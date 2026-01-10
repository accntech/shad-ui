using System;
using System.IO;
using Avalonia;
using CommunityToolkit.Mvvm.Messaging;
using Jab;
using Serilog;
using ShadUI.Demo.ViewModels;
using ShadUI.Demo.ViewModels.Examples.ComboBox;
using ShadUI.Demo.ViewModels.Examples.DataTable;
using ShadUI.Demo.ViewModels.Examples.Date;
using ShadUI.Demo.ViewModels.Examples.Input;
using ShadUI.Demo.ViewModels.Examples.ListBox;
using ShadUI.Demo.ViewModels.Examples.Numeric;
using ShadUI.Demo.ViewModels.Examples.Time;
using ShadUI.Demo.ViewModels.Examples.Typography;

namespace ShadUI.Demo;

[ServiceProvider]
[Transient<AboutViewModel>]
[Transient<AvatarViewModel>]
[Transient<BadgeViewModel>]
[Transient<ButtonViewModel>]
[Transient<CardViewModel>]
[Transient<DataTableViewModel>]
[Transient<BasicDataTableViewModel>]
[Transient<GroupedDataTableViewModel>]
[Transient<DateViewModel>]
[Transient<FormDateInputViewModel>]
[Transient<FormDatePickerViewModel>]
[Transient<CheckBoxViewModel>]
[Transient<ComboBoxViewModel>]
[Transient<FormComboBoxViewModel>]
[Transient<ColorViewModel>]
[Transient<DashboardViewModel>]
[Transient<DialogViewModel>]
[Transient<InputViewModel>]
[Transient<FormInputViewModel>]
[Transient<NumericViewModel>]
[Transient<FormNumericViewModel>]
[Transient<LoginViewModel>]
[Transient<MenuViewModel>]
[Transient<MiscellaneousViewModel>]
[Transient<ListBoxViewModel>]
[Transient<SidebarViewModel>]
[Transient<SliderViewModel>]
[Transient<SwitchViewModel>]
[Transient<TabControlViewModel>]
[Transient<TimeViewModel>]
[Transient<FormTimeInputViewModel>]
[Transient<FormTimePickerViewModel>]
[Transient<ThemeViewModel>]
[Transient<ToastViewModel>]
[Transient<ToggleViewModel>]
[Transient<ToolTipViewModel>]
[Transient<TypographyViewModel>]
[Transient<SmoothScrollViewModel>]
[Transient<TextBlockViewModel>]
[Transient<SelectableTextBlockViewModel>]
[Transient<LabelViewModel>]
[Transient<MainWindowViewModel>]
[Import<IUtilitiesModule>]
[Singleton<IMessenger, WeakReferenceMessenger>]
[Singleton(typeof(ThemeWatcher), Factory = nameof(ThemeWatcherFactory))]
[Singleton(typeof(ILogger), Factory = nameof(LoggerFactory))]
[Singleton(typeof(PageManager), Factory = nameof(PageManagerFactory))]
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

    public PageManager PageManagerFactory()
    {
        return new PageManager(this);
    }
}