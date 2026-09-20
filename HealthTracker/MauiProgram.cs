using HealthTracker.Services;
using Microsoft.Extensions.Logging;

namespace HealthTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Calliope - SERVICE REGISTRATION: the chain growing, like I promised on the
		// tour. AddSingleton = "create ONE HealthDatabase and share it with everyone
		// who asks." Any page can now declare "I need a HealthDatabase" in its
		// constructor and MAUI hands it over. That's dependency injection: the
		// opposite of pages creating their own database connections.
		builder.Services.AddSingleton<HealthDatabase>();

		// Calliope - MainPage registered explicitly so Shell builds it through the
		// same injection system — when we give MainPage a HealthDatabase parameter,
		// it will just work. Transient = a fresh page each time it's navigated to.
		builder.Services.AddTransient<MainPage>();

        // Calliope - AddFoodPage is a tab with a HealthDatabase constructor parameter,
        // so it needs the same transient registration as MainPage.
        builder.Services.AddTransient<AddFoodPage>();

        // Calliope - NutritionPage is a tab with a HealthDatabase constructor parameter,
        // so it needs the same transient registration as the other pages.
        builder.Services.AddTransient<NutritionPage>();



#if DEBUG
        builder.Logging.AddDebug();
		#endif

		return builder.Build();
	}
}
