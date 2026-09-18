// Calliope - A "model" is a plain C# class that mirrors one database table.
// sqlite-net-pcl reads these attributes and creates the matching table for us —
// we never write CREATE TABLE by hand. This mirrors your log's food_entries
// table, column for column, so data could move between the two someday.
using SQLite;

namespace HealthTracker.Models;

[Table("food_entries")]
public class FoodEntry
{
    // Calliope - Primary key: the unique id of each row. AutoIncrement means
    // SQLite assigns it (1, 2, 3...) — we never set it ourselves.
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Calliope - Dates are TEXT like "2026-09-18", exactly like your log. SQLite
    // has no real date type; this format sorts and compares correctly as plain
    // text. Keep every date in the app in this shape and life stays simple.
    // [Indexed] = "we filter by this column constantly, keep a fast lookup."
    [Indexed]
    public string Date { get; set; } = string.Empty;

    public string Time { get; set; } = string.Empty;

    // Calliope - "Breakfast", "Lunch", "Dinner", "Snack"... free text, no enum,
    // same as your log. You can invent "Brunch" without touching code.
    public string Meal { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Calliope - string? (with the ?) means "allowed to be missing." Your project
    // has Nullable enabled, so the compiler makes us declare which fields can be
    // empty. Anything without ? must always have a value.
    public string? Amount { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
}
