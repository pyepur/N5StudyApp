using Microsoft.Extensions.Logging;
using N5StudyApp.Services;
using Microsoft.Maui;
using N5StudyApp.ViewModels;
namespace N5StudyApp;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddSingleton<IDataService, JsonDataService>();
		builder.Services.AddTransient<FlashcardViewModel>();
		return builder.Build();
	}
}
