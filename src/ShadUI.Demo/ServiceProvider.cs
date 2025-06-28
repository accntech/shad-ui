using System;
using System.IO;
using Avalonia;
using Jab;
using Serilog;
using ShadUI.Demo.ViewModels;
using ShadUI.Themes;

namespace ShadUI.Demo;

[ServiceProvider]
[Transient<AboutViewModel>]
[Transient<AvatarViewModel>]
[Transient<ButtonsViewModel>]
[Transient<CardsViewModel>]
[Transient<DataGridViewModel>]
[Transient<DateViewModel>]
[Transient<CheckBoxesViewModel>]
[Transient<ComboBoxesViewModel>]
[Transient<ColorsViewModel>]
[Transient<DashboardViewModel>]
[Transient<DialogsViewModel>]
[Transient<AccordionViewModel>]
[Transient<InputViewModel>]
[Transient<NumericViewModel>]
[Transient<LoginViewModel>]
[Transient<MenuViewModel>]
[Transient<MiscellaneousViewModel>]
[Transient<SlidersViewModel>]
[Transient<SwitchViewModel>]
[Transient<TabsViewModel>]
[Transient<TimeViewModel>]
[Transient<ThemeViewModel>]
[Transient<ToastsViewModel>]
[Transient<ToggleViewModel>]
[Transient<ToolTipViewModel>]
[Transient<TypographyViewModel>]
[Transient<MainWindowViewModel>]
[Import<IUtilitiesModule>]
[Singleton(typeof(ThemeWatcher), Factory = nameof(ThemeWatcherFactory))]
[Singleton(typeof(ILogger), Factory = nameof(LoggerFactory))]
public partial class ServiceProvider
{
    public ILogger LoggerFactory()
    {
        var currentFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ShadUI\\logs");

        Directory.CreateDirectory(currentFolder); //ensure the directory exists

        var file = Path.Combine(currentFolder, "log.txt");

        var config = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(file, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Logger = config; //set the global logger

        return config;
    }

    public ThemeWatcher ThemeWatcherFactory()
    {
        return new ThemeWatcher(Application.Current!);
    }
}