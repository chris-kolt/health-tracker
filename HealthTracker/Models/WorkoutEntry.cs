// Calliope - Same model pattern as FoodEntry: attributes describe the table,
// properties describe the columns. Mirrors your log's workout_entries.
using SQLite;

namespace HealthTracker.Models;

[Table("workout_entries")]
public class WorkoutEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Date { get; set; } = string.Empty;
    public string? Time { get; set; }

    // Calliope - Session groups exercises: "morning routine". Exercise names one:
    // "Squats". Reps stays TEXT like your log ("12", "2x12", "30 sec") — flexible.
    public string? Session { get; set; }
    public string Exercise { get; set; } = string.Empty;
    public int? Sets { get; set; }
    public string? Reps { get; set; }
    public double? WeightLb { get; set; }
    public int? Rpe { get; set; }
    public string? Notes { get; set; }
}
