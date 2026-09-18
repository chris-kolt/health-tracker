// Calliope - File-scoped namespace: everything below lives in "HealthTracker".
namespace HealthTracker;

// Calliope - "partial" is the load-bearing keyword. This class is completed by
// generated code from MainPage.xaml (that's the x:Class handshake). The XAML
// declares the controls; this file declares the behavior; together ONE class.
public partial class MainPage : ContentPage
{
    // Calliope - The constructor runs once, when the page is created.
    public MainPage()
    {
        // Calliope - REQUIRED first line. Builds all the XAML controls so the
        // x:Name variables (like DateLabel) actually exist. Always call it first,
        // before touching any named control.
        InitializeComponent();

        // Calliope - DateLabel comes from x:Name="DateLabel" in the XAML.
        // "dddd, MMMM d" renders as e.g. "Friday, September 18". Try changing
        // the format string and re-running to see what happens.
        DateLabel.Text = DateTime.Now.ToString("dddd, MMMM d");
    }

    // Calliope - Event handler: runs every time the checkbox is toggled.
    // 'sender' is the CheckBox itself; e.Value is the new state (true/false).
    private void OnWorkoutChecked(object sender, CheckedChangedEventArgs e)
    {
        // Calliope - Ternary operator: condition ? value-if-true : value-if-false.
        // Compact if/else. Reads as: "checked? show done : show not-done".
        WorkoutLabel.Text = e.Value
            ? "Daily routine — done!"
            : "Daily routine — not done yet";
    }
}
