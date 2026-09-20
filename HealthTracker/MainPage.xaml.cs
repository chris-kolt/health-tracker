using HealthTracker.Models;
using HealthTracker.Services;

// Calliope - File-scoped namespace: everything below lives in "HealthTracker".
namespace HealthTracker;

// Calliope - "partial": completed by generated code from MainPage.xaml (the x:Class
// handshake). XAML declares the controls; this file declares the behavior.
public partial class MainPage : ContentPage
{
    // Calliope - The shared database, handed over by MAUI (dependency injection).
    private readonly HealthDatabase _db;

    public MainPage(HealthDatabase db)
    {
        InitializeComponent();
        _db = db;
        // Calliope - "dddd, MMM. d" -> "Sunday, Sep. 20", matching your sketch.
        DateLabel.Text = DateTime.Now.ToString("dddd, MMM. d");

        // Macro Rings
        ProteinRing.Drawable = _proteinRing;
        CarbsRing.Drawable = _carbsRing;
        FatRing.Drawable = _fatRing;
        FiberRing.Drawable = _fiberRing;

    }

    // Calliope - One drawable per ring. They hold the progress value; the
    // GraphicsView in the XAML is just the frame they paint into.
    private readonly RingDrawable _proteinRing = new() { FillColor = Colors.DodgerBlue };
    private readonly RingDrawable _carbsRing = new() { FillColor = Colors.DarkOrange };
    private readonly RingDrawable _fatRing = new() { FillColor = Colors.Gold };
    private readonly RingDrawable _fiberRing = new() { FillColor = Colors.SeaGreen };

    // Calliope - Runs EVERY time the page appears, so the numbers refresh whenever
    // you switch back to this tab.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTodayAsync();
    }

    private async Task LoadTodayAsync()
    {
        var today = DateTime.Now.ToString("yyyy-MM-dd");

        // Calliope - Seed your real targets on first launch, so the screen shows
        // truth instead of zeros. Runs once ever.
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

        // Calliope - Calories card: eaten / goal, remaining, and the bar.
        var calorieGoal = target.CalorieGoal ?? 0;
        CaloriesLabel.Text = $"{totals.Calories:0} / {calorieGoal}";
        RemainingLabel.Text = $"{Math.Max(0, calorieGoal - totals.Calories):0} remaining";
        CaloriesBar.Progress = calorieGoal > 0 ? Math.Min(1, totals.Calories / calorieGoal) : 0;

        // Calliope - Macros card: the same eaten-vs-goal pattern four times. A touch
        // repetitive on purpose — clarity beats cleverness while you're learning.
        var proteinGoal = target.ProteinGoalG ?? 0;
        ProteinLabel.Text = $"{totals.ProteinG:0} / {proteinGoal}g";
        _proteinRing.Progress = proteinGoal > 0 ? Math.Min(1, totals.ProteinG / proteinGoal) : 0;
        ProteinRing.Invalidate(); // "repaint yourself with the new value"


        var carbsGoal = target.CarbsGoalG ?? 0;
        CarbsLabel.Text = $"{totals.CarbsG:0} / {carbsGoal}g";
        _carbsRing.Progress = carbsGoal > 0 ? Math.Min(1, totals.CarbsG / carbsGoal) : 0;
        CarbsRing.Invalidate(); // "repaint yourself with the new value"


        var fatGoal = target.FatGoalG ?? 0;
        FatLabel.Text = $"{totals.FatG:0} / {fatGoal}g";
        _fatRing.Progress = fatGoal > 0 ? Math.Min(1, totals.FatG / fatGoal) : 0;
        FatRing.Invalidate(); // "repaint yourself with the new value"

        var fiberGoal = target.FiberGoalG ?? 0;
        FiberLabel.Text = $"{totals.FiberG:0} / {fiberGoal}g";
        _fiberRing.Progress = fiberGoal > 0 ? Math.Min(1, totals.FiberG / fiberGoal) : 0;
        FiberRing.Invalidate(); // "repaint yourself with the new value"
    }

    // Calliope - The expand buttons: for now both jump to the Nutrition tab, where
    // the meals behind these numbers will live. Dedicated detail pages come later.
    private async void OnShowDetailsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//NutritionPage");
    }
}
