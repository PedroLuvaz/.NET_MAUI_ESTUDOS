using GerenciadorContas.Maui.Services;
using GerenciadorContas.Maui.ViewModels;
using GerenciadorContas.Maui.Views;

namespace GerenciadorContas.Maui
{
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

			// Registamos o ApiService como "Singleton" (uma única instância para toda a app)
			// Quando alguém pedir um "IApiService", entregue um "ApiService".
			builder.Services.AddSingleton<IApiService, ApiService>();

			// Registamos as Views e ViewModels como "Transient" (uma nova instância
			// é criada sempre que a página é pedida).

			// Quando a AppShell pedir a "MainPage"...
			builder.Services.AddTransient<MainPage>();

			// ...ela vai precisar de uma "MainPageViewModel".
			builder.Services.AddTransient<MainPageViewModel>();

			return builder.Build();
		}
	}
}