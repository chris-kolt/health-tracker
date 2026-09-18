// Calliope - Your exercise targets (squats 2x12, plank 2x30sec...), also with
// history via EffectiveFrom, same idea as Target.
using SQLite;

namespace HealthTracker.Models;

[Table("exercise_targets")]
public class ExerciseTarget
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string EffectiveFrom { get; set; } = string.Empty;
    public string Exercise { get; set; } = string.Empty;
    public int? Sets { get; set; }
    public int? Reps { get; set; }
    public double? HoldSec { get; set; }
    public string? Notes { get; set; }
}
