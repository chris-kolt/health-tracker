namespace HealthTracker;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        // Calliope - Route registration: teaches Shell the name "AddFoodPage" so any
        // page can navigate to it with GoToAsync. Registered once, usable everywhere.
        // nameof() = "the name as the compiler knows it", so renaming the class can't
        // silently break a magic string.
        Routing.RegisterRoute(nameof(AddFoodPage), typeof(AddFoodPage));

    }
}
