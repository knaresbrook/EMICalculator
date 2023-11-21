using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using CommunityToolkit.Maui.Core.Platform;
using EMICalculator.View;
using EMICalculator.ViewModel;

namespace EMICalculator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("ShowSoftInputOnFocus", (handler, view) =>
            {
                if (view is Entry entry)
                {
                    handler.PlatformView.ShowSoftInputOnFocus = true;
                }
            });
#endif

            builder.ConfigureMauiHandlers(builder =>
            {
#if ANDROID
                builder.AddHandler(typeof(Shell), typeof(EMICalculator.Platforms.Android.Renderers.CustomShellRenderer));
#endif
            });


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}