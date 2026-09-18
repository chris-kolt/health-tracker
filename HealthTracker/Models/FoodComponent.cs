// Calliope - One row per INGREDIENT inside a food entry. Your log is "ingredient-up":
// the entry is "lunch", the components are "chicken 150g", "rice 200g", each with
// its own calories and macros. Day totals are just sums of components.
using SQLite;

namespace HealthTracker.Models;

[Table("food_components")]
public class FoodComponent
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Calliope - The glue between the tables: the Id of the parent FoodEntry.
    // Components with EntryId = 7 are the ingredients of entry 7.
    [Indexed]
    public int EntryId { get; set; }

    public string Ingredient { get; set; } = string.Empty;
    public double? Grams { get; set; }
    public string? AmountText { get; set; }
    public double? Calories { get; set; }
    public double? ProteinG { get; set; }
    public double? CarbsG { get; set; }
    public double? FatG { get; set; }
    public double? FiberG { get; set; }
    public string? Basis { get; set; }
}
