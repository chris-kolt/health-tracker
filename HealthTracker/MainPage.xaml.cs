using HealthTracker.Models;
using HealthTracker.Services;

// Calliope - File-scoped namespace: everything below lives in "HealthTracker".
namespace HealthTracker;

// Calliope - "partial" is the load-bearing keyword. This class is completed by
// generated code from MainPage.xaml (that's the x:Class handshake). The XAML
// declares the controls; this file declares the behavior; together ONE class.
public partial class MainPage : ContentPage
{
    
    // Calliope - The shared database, handed to us by MAUI. Because we registered
    // HealthDatabase as a singleton in MauiProgram, MAUI sees this constructor
    // parameter and passes the one shared instance in. This is dependency injection
    // paying off: the page never creates a database, it just declares what it needs.
    private readonly HealthDatabase _db;
    
    // Calliope - The constructor runs once, when the page is created.
    public MainPage(HealthDatabase db)
    {
        InitializeComponent();
        _db = db;
        DateLabel.Text = DateTime.Now.ToString("dddd, MMMM d");
    }

    // Calliope - Event handler: runs every time the checkbox is toggled.
    // 'sender' is the CheckBox itself; e.Value is the new state (true/false).
    private void OnWorkoutChecked(object sender, CheckedChangedEventArgs e)
    {
        // Calliope - Ternary operator: condition ? value-if-true : value-if-false.
        // Compact if/else. Reads as: "checked? show done : show not-done".
        WorkoutLabel.Text = e.Value
            ? "Daily routine — done!"
            : "Daily routine — not done yet";
    }

    // Calliope - OnAppearing runs EVERY time this page shows on screen (including
    // coming back to it later). The constructor runs once. Data loading goes here
    // so the numbers refresh on every visit. (async void is normally a smell, but
    // for event-style overrides like this one it's the accepted pattern.)
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTodayAsync();
    }

    private async Task LoadTodayAsync()
    {
        // Calliope - "yyyy-MM-dd": the database's date shape, matching your log.
        var today = DateTime.Now.ToString("yyyy-MM-dd");

        // Calliope - Seed your real targets on first launch, so the screen shows
        // truth instead of zeros. Runs once ever — after that a target exists and
        // this block is skipped.
        var target = await _db.GetCurrentTargetAsync(today);
        if (target == null)
        {
            target = new Target
            {
                EffectiveFrom = "2026-09-16",
                CalorieGoal = 1750,
                ProteinGoalG = 140,
                CarbsGoalG = 150,
                FatGoalG = 65,
                FiberGoalG = 25,
                Notes = "Initial targets"
            };
            await _db.SaveTargetAsync(target);
        }

        var totals = await _db.GetDailyTotalsAsync(today);

        // Calliope - ?? 0 means "if the goal is missing, treat it as 0" (never crash
        // on a null). Math.Min/Max keep the bar and the "remaining" text sane when
        // someone eats over target.
        var goal = target.CalorieGoal ?? 0;
        CaloriesLabel.Text = $"{totals.Calories:0} / {goal}";
        RemainingLabel.Text = $"{Math.Max(0, goal - totals.Calories):0} remaining";
        CaloriesBar.Progress = goal > 0 ? Math.Min(1, totals.Calories / goal) : 0;
    }
}
