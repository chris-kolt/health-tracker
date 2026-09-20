using HealthTracker.Models;
using HealthTracker.Services;

namespace HealthTracker;

public partial class GoalsPage : ContentPage
{
    private readonly HealthDatabase _db;

    // Calliope - Same constructor injection as the other pages.
    public GoalsPage(HealthDatabase db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var target = await _db.GetOrCreateCurrentTargetAsync(today);

        CaloriesEntry.Text = (target.CalorieGoal ?? 0).ToString();
        ProteinEntry.Text = (target.ProteinGoalG ?? 0).ToString();
        CarbsEntry.Text = (target.CarbsGoalG ?? 0).ToString();
        FatEntry.Text = (target.FatGoalG ?? 0).ToString();
        FiberEntry.Text = (target.FiberGoalG ?? 0).ToString();
        SavedLabel.IsVisible = false;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Calliope - Parse everything BEFORE saving anything: one bad field
        // must not silently zero out a goal.
        if (!int.TryParse(CaloriesEntry.Text, out var calories) ||
            !int.TryParse(ProteinEntry.Text, out var protein) ||
            !int.TryParse(CarbsEntry.Text, out var carbs) ||
            !int.TryParse(FatEntry.Text, out var fat) ||
            !int.TryParse(FiberEntry.Text, out var fiber))
        {
            SavedLabel.Text = "Enter whole numbers in every field.";
            SavedLabel.TextColor = Colors.Red;
            SavedLabel.IsVisible = true;
            return;
        }

        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var target = await _db.GetOrCreateCurrentTargetAsync(today);

        // Calliope - History-safe: reuse today's row if there is one,
        // otherwise start a new row effective today.
        if (target.EffectiveFrom != today)
            target = new Target { EffectiveFrom = today };

        target.CalorieGoal = calories;
        target.ProteinGoalG = protein;
        target.CarbsGoalG = carbs;
        target.FatGoalG = fat;
        target.FiberGoalG = fiber;

        await _db.SaveTargetAsync(target);

        SavedLabel.Text = $"Saved ✓ (effective {today})";
        SavedLabel.TextColor = Colors.Green;
        SavedLabel.IsVisible = true;
    }
}
