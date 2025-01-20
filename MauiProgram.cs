using MauiCrud.Services;
using MauiCrud.Pages;
using Microsoft.Extensions.Logging;

namespace MauiCrud
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

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<DashBoard>();
            builder.Services.AddTransient<ExpensePage>();
            builder.Services.AddTransient<ExpenseCategoryPage>();
            builder.Services.AddTransient<RevenuePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
