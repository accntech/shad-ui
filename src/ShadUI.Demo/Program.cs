using System;
using Avalonia;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using SkiaSharp;

namespace ShadUI.Demo;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            LiveCharts.Configure(config =>
                config.HasTextSettings(new TextSettings
                {
                    DefaultTypeface = SKTypeface.FromFamilyName("Segoe UI") ?? SKTypeface.Default
                }));

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            var serviceProvider = new ServiceProvider();

            var logger = serviceProvider.GetService<ILogger>();
            logger.Fatal(e, "An unhandled exception occurred during bootstrapping the application.");
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseHarfBuzz()
            .LogToTrace();
    }
}