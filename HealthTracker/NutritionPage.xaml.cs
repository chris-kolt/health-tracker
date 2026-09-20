using HealthTracker.Services;

namespace HealthTracker;

public partial class NutritionPage : ContentPage
{
    private readonly HealthDatabase _db;

    // Calliope - The day being viewed. Starts at today; the arrows walk it
    // backward/forward and every change reloads the table.
    private DateTime _currentDate = DateTime.Today;

    public NutritionPage(HealthDatabase db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDayAsync();
    }

    private async void OnPrevDayClicked(object sender, EventArgs e)
    {
        _currentDate = _currentDate.AddDays(-1);
        await LoadDayAsync();
    }

    private async void OnNextDayClicked(object sender, EventArgs e)
    {
        // Calliope - No peeking into the future: tomorrow has no entries yet.
        if (_currentDate < DateTime.Today)
        {
            _currentDate = _currentDate.AddDays(1);
            await LoadDayAsync();
        }
    }

    private async Task LoadDayAsync()
    {
        var dateStr = _currentDate.ToString("yyyy-MM-dd");
        DateLabel.Text = _currentDate.ToString("dddd, MMM. d");

        // Calliope - Flatten entries + components into table rows: one bold row per
        // entry (summing its components), then one indented row per ingredient.
        var rows = new List<EntryRow>();
        var entries = await _db.GetFoodEntriesAsync(dateStr);
        foreach (var entry in entries)
        {
            var components = await _db.GetComponentsAsync(entry.Id);

            // Calliope - "?? 0": component numbers are nullable ("might be missing"),
            // so fall back to 0 inside the sums.
            rows.Add(new EntryRow
            {
                Name = entry.Description,
                Calories = $"{components.Sum(c => c.Calories ?? 0):0}",
                Protein = $"{components.Sum(c => c.ProteinG ?? 0):0}",
                Carbs = $"{components.Sum(c => c.CarbsG ?? 0):0}",
                Fat = $"{components.Sum(c => c.FatG ?? 0):0}",
                Fiber = $"{components.Sum(c => c.FiberG ?? 0):0}",
                NameFont = FontAttributes.Bold
            });

            foreach (var c in components)
            {
                rows.Add(new EntryRow
                {
                    Name = "- " + c.Ingredient,
                    NameMargin = new Thickness(14, 0, 0, 0),
                    Calories = $"{c.Calories ?? 0:0}",
                    Protein = $"{c.ProteinG ?? 0:0}",
                    Carbs = $"{c.CarbsG ?? 0:0}",
                    Fat = $"{c.FatG ?? 0:0}",
                    Fiber = $"{c.FiberG ?? 0:0}",
                    IsIngredient = true,
                    IsAlternate = rows.Count % 2 == 1
                });
            }
        }
        EntriesView.ItemsSource = rows;

        // Calliope - Pinned totals row: the day's sums, always visible under the list.
        var totals = await _db.GetDailyTotalsAsync(dateStr);
        TotalCaloriesLabel.Text = $"{totals.Calories:0}";
        TotalProteinLabel.Text = $"{totals.ProteinG:0}";
        TotalCarbsLabel.Text = $"{totals.CarbsG:0}";
        TotalFatLabel.Text = $"{totals.FatG:0}";
        TotalFiberLabel.Text = $"{totals.FiberG:0}";
    }
}
