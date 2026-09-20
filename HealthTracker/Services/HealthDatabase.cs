// Calliope - THE database service: the app's single front door to SQLite. Pages
// never touch SQLite directly; they call these friendly methods. One class owns
// every query, so when the schema changes there's exactly one place to update.
using SQLite;
using HealthTracker.Models;

namespace HealthTracker.Services;

public class HealthDatabase
{
    // Calliope - The async connection. "Async" matters on a phone: database work
    // never blocks the UI thread, so the app can't freeze mid-scroll.
    private readonly SQLiteAsyncConnection _db;

    // Calliope - Guards EnsureInitAsync so tables are created exactly once,
    // no matter how many methods get called.
    private bool _initialized;

    public HealthDatabase()
    {
        // Calliope - FileSystem.AppDataDirectory is MAUI's private folder for YOUR
        // app on the phone. Nothing else can read it; uninstalling the app deletes
        // it. This is the "data stays on the device" part of your plan.
        var path = Path.Combine(FileSystem.AppDataDirectory, "healthtracker.db3");
        _db = new SQLiteAsyncConnection(path);
    }

    // Calliope - Creates every table from the model classes — but ONLY if missing.
    // Existing data is never touched. Every method below calls this first, so no
    // page has to remember to initialize the database. Laziness as a feature.
    private async Task EnsureInitAsync()
    {
        if (_initialized)
            return;

        await _db.CreateTableAsync<FoodEntry>();
        await _db.CreateTableAsync<FoodComponent>();
        await _db.CreateTableAsync<WorkoutEntry>();
        await _db.CreateTableAsync<BodyMetric>();
        await _db.CreateTableAsync<Target>();
        await _db.CreateTableAsync<ExerciseTarget>();

        _initialized = true;
    }

    // ---- Food ----

    // Calliope - All entries for one day, newest first. The Today screen will call
    // this with today's date string ("2026-09-18").
    public async Task<List<FoodEntry>> GetFoodEntriesAsync(string date)
    {
        await EnsureInitAsync();
        return await _db.Table<FoodEntry>()
            .Where(e => e.Date == date)
            .OrderByDescending(e => e.Time)
            .ToListAsync();
    }

    public async Task<List<FoodComponent>> GetComponentsAsync(int entryId)
    {
        await EnsureInitAsync();
        return await _db.Table<FoodComponent>()
            .Where(c => c.EntryId == entryId)
            .ToListAsync();
    }

    // Calliope - Insert-or-update in one method: Id 0 means "new row" (SQLite assigns
    // the id); anything else means "update the row with this id". Callers don't
    // think about which — they just Save.
    public async Task SaveFoodEntryAsync(FoodEntry entry)
    {
        await EnsureInitAsync();
        if (entry.Id == 0)
            await _db.InsertAsync(entry);
        else
            await _db.UpdateAsync(entry);
    }

    public async Task SaveFoodComponentAsync(FoodComponent component)
    {
        await EnsureInitAsync();
        if (component.Id == 0)
            await _db.InsertAsync(component);
        else
            await _db.UpdateAsync(component);
    }

    // Calliope - Deleting an entry also deletes its components: no orphaned
    // ingredient rows left pointing at a meal that no longer exists.
    public async Task DeleteFoodEntryAsync(FoodEntry entry)
    {
        await EnsureInitAsync();
        var components = await GetComponentsAsync(entry.Id);
        foreach (var c in components)
            await _db.DeleteAsync(c);
        await _db.DeleteAsync(entry);
    }

    // Calliope - The day's totals: every entry for the date, sum every component's
    // macros in plain C#. For a personal app this is instant — no raw SQL needed,
    // and it reads like the math it is.
    public async Task<DailyTotals> GetDailyTotalsAsync(string date)
    {
        await EnsureInitAsync();
        var entries = await GetFoodEntriesAsync(date);
        var totals = new DailyTotals();
        foreach (var entry in entries)
        {
            var components = await GetComponentsAsync(entry.Id);
            foreach (var c in components)
            {
                totals.Calories += c.Calories ?? 0;
                totals.ProteinG += c.ProteinG ?? 0;
                totals.CarbsG += c.CarbsG ?? 0;
                totals.FatG += c.FatG ?? 0;
                totals.FiberG += c.FiberG ?? 0;
            }
        }
        return totals;
    }

    // ---- Workouts ----

    public async Task<List<WorkoutEntry>> GetWorkoutsAsync(string date)
    {
        await EnsureInitAsync();
        return await _db.Table<WorkoutEntry>()
            .Where(w => w.Date == date)
            .ToListAsync();
    }

    public async Task SaveWorkoutAsync(WorkoutEntry workout)
    {
        await EnsureInitAsync();
        if (workout.Id == 0)
            await _db.InsertAsync(workout);
        else
            await _db.UpdateAsync(workout);
    }

    // ---- Body metrics & targets ----

    // Calliope - The most recent weigh-in, whatever day it was on. Your morning
    // summary rule — "latest weight plus its date" — is exactly this query.
    public async Task<BodyMetric?> GetLatestWeightAsync()
    {
        await EnsureInitAsync();
        return await _db.Table<BodyMetric>()
            .Where(m => m.WeightLb != null)
            .OrderByDescending(m => m.Date)
            .FirstOrDefaultAsync();
    }

    public async Task SaveBodyMetricAsync(BodyMetric metric)
    {
        await EnsureInitAsync();
        if (metric.Id == 0)
            await _db.InsertAsync(metric);
        else
            await _db.UpdateAsync(metric);
    }

    // Calliope - App invariant: a current target ALWAYS exists. Pages call this,
    // never the nullable getter above, so nobody null-checks or seeds. First
    // ever call inserts your real starting numbers, editable later in Goals.
    public async Task<Target> GetOrCreateCurrentTargetAsync(string date)
    {
        var target = await GetCurrentTargetAsync(date);
        if (target != null)
            return target;

        target = new Target
        {
            EffectiveFrom = date,
            CalorieGoal = 1750,
            ProteinGoalG = 140,
            CarbsGoalG = 150,
            FatGoalG = 65,
            FiberGoalG = 25,
            Notes = "Initial targets"
        };
        await SaveTargetAsync(target);
        return target;
    }


    // Calliope - "Current target" = the row with the latest EffectiveFrom on or
    // before today. Pulled into C# for the comparison to keep it obvious.
    private async Task<Target?> GetCurrentTargetAsync(string date)
    {
        await EnsureInitAsync();
        var all = await _db.Table<Target>()
            .OrderByDescending(t => t.EffectiveFrom)
            .ToListAsync();
        return all.FirstOrDefault(t =>
            string.Compare(t.EffectiveFrom, date, StringComparison.Ordinal) <= 0);
    }

    public async Task SaveTargetAsync(Target target)
    {
        await EnsureInitAsync();
        if (target.Id == 0)
            await _db.InsertAsync(target);
        else
            await _db.UpdateAsync(target);
    }
}
