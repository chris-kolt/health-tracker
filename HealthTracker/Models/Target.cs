// Calliope - Your nutrition targets, WITH HISTORY. Targets change over time, so
// each row says "these numbers apply from this date onward." The current target
// is the row with the latest EffectiveFrom on or before today.
using SQLite;

namespace HealthTracker.Models;

[Table("targets")]
public class Target
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string EffectiveFrom { get; set; } = string.Empty;
    public int? CalorieGoal { get; set; }
    public int? ProteinGoalG { get; set; }
    public int? CarbsGoalG { get; set; }
    public int? FatGoalG { get; set; }

    // Calliope - Your log's targets table doesn't have this column, but YOUR actual
    // targets include 25g+ fiber — so the app's table gets it from day one.
    public int? FiberGoalG { get; set; }

    public double? WaistGoalIn { get; set; }
    public string? Notes { get; set; }
}
