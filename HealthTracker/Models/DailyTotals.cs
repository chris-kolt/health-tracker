// Calliope - NOT a database table: no [Table] attribute, never passed to
// CreateTableAsync. Just a bucket for "the day's summed macros", computed by
// HealthDatabase from the components. Keeps the summing logic in one place.
namespace HealthTracker.Models;

public class DailyTotals
{
    public double Calories { get; set; }
    public double ProteinG { get; set; }
    public double CarbsG { get; set; }
    public double FatG { get; set; }
    public double FiberG { get; set; }
}
