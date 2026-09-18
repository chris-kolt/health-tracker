// Calliope - Weigh-ins and measurements. One row per check-in day.
using SQLite;

namespace HealthTracker.Models;

[Table("body_metrics")]
public class BodyMetric
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Date { get; set; } = string.Empty;
    public double? WeightLb { get; set; }
    public double? WaistIn { get; set; }
    public double? ThighIn { get; set; }
    public string? Notes { get; set; }
}
