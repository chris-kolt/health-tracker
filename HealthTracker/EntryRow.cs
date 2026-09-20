namespace HealthTracker;

// Calliope - One row in the Nutrition table. Pre-formatted strings keep the XAML
// bindings dead simple; margin/font carry the indent-and-bold styling as data,
// so the XAML needs no styling tricks.
public class EntryRow
{
    public string Name { get; set; } = string.Empty;
    public string Calories { get; set; } = string.Empty;
    public string Protein { get; set; } = string.Empty;
    public string Carbs { get; set; } = string.Empty;
    public string Fat { get; set; } = string.Empty;
    public string Fiber { get; set; } = string.Empty;
    public Thickness NameMargin { get; set; } = new Thickness(0);
    public FontAttributes NameFont { get; set; } = FontAttributes.None;

    // Calliope - Flags ingredient sub-rows so the XAML can style them
    // differently (smaller, gray) via a DataTrigger below.
    public bool IsIngredient { get; set; }

    // Calliope - Zebra striping: odd-indexed rows get the alternate background
    // via a DataTrigger, since the table has no row borders.
    public bool IsAlternate { get; set; }


}
