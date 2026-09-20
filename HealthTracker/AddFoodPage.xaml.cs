using HealthTracker.Models;
using HealthTracker.Services;

namespace HealthTracker;

public partial class AddFoodPage : ContentPage
{
    private readonly HealthDatabase _db;

    // Calliope - Same DI pattern as MainPage: declare the need, MAUI hands over the
    // shared database. Because this page now takes a constructor parameter, it MUST
    // be registered in MauiProgram — otherwise Shell can't build it.
    public AddFoodPage(HealthDatabase db)
    {
        InitializeComponent();
        _db = db;

        MealPicker.ItemsSource = new List<string> { "Breakfast", "Lunch", "Dinner", "Snack" };
        MealPicker.SelectedIndex = 0;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Calliope - Guard clause: DisplayAlert is MAUI's built-in popup dialog.
        if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
        {
            await DisplayAlertAsync("Missing description", "Tell me what you ate first.", "OK");
            return;
        }

        // Calliope - TryParse converts text to numbers WITHOUT ever crashing: bad
        // input just becomes 0.
        double.TryParse(CaloriesEntry.Text, out var calories);
        double.TryParse(ProteinEntry.Text, out var protein);
        double.TryParse(CarbsEntry.Text, out var carbs);
        double.TryParse(FatEntry.Text, out var fat);
        double.TryParse(FiberEntry.Text, out var fiber);

        var today = DateTime.Now.ToString("yyyy-MM-dd");

        // Calliope - Quick-log: one entry + one component carrying the totals.
        var entry = new FoodEntry
        {
            Date = today,
            Time = DateTime.Now.ToString("HH:mm"),
            Meal = MealPicker.SelectedItem as string ?? "Snack",
            Description = DescriptionEntry.Text.Trim()
        };
        await _db.SaveFoodEntryAsync(entry); // entry.Id is populated on insert

        await _db.SaveFoodComponentAsync(new FoodComponent
        {
            EntryId = entry.Id,
            Ingredient = entry.Description,
            Calories = calories,
            ProteinG = protein,
            CarbsG = carbs,
            FatG = fat,
            FiberG = fiber,
            Basis = "quick log"
        });

        // Calliope - This page is a TAB now, not a pushed page: there's nowhere to go
        // "back" to. So clear the form for the next item and confirm inline — which
        // suits batch logging: save, save, save, no navigation in between.
        var what = entry.Description;
        DescriptionEntry.Text = string.Empty;
        CaloriesEntry.Text = string.Empty;
        ProteinEntry.Text = string.Empty;
        CarbsEntry.Text = string.Empty;
        FatEntry.Text = string.Empty;
        FiberEntry.Text = string.Empty;
        MealPicker.SelectedIndex = 0;

        SavedLabel.Text = $"Saved {what} ✓";
        SavedLabel.IsVisible = true;
    }
}
