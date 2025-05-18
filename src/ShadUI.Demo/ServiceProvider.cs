using Avalonia;
using Jab;
using ShadUI.Demo.ViewModels;
using ShadUI.Themes;

namespace ShadUI.Demo;

[ServiceProvider]
[Transient<AboutViewModel>]
[Transient<AvatarsViewModel>]
[Transient<ButtonsViewModel>]
[Transient<CardsViewModel>]
[Transient<DataGridViewModel>]
[Transient<DateViewModel>]
[Transient<CheckBoxesViewModel>]
[Transient<ComboBoxesViewModel>]
[Transient<DashboardViewModel>]
[Transient<DialogsViewModel>]
[Transient<ExpandersViewModel>]
[Transient<InputViewModel>]
[Transient<LoginViewModel>]
[Transient<MenuViewModel>]
[Transient<MiscellaneousViewModel>]
[Transient<SlidersViewModel>]
[Transient<SwitchViewModel>]
[Transient<TabsViewModel>]
[Transient<TimeViewModel>]
[Transient<ThemeViewModel>]
[Transient<ToastsViewModel>]
[Transient<TogglesViewModel>]
[Transient<ToolTipViewModel>]
[Transient<TypographyViewModel>]
[Transient<MainWindowViewModel>]
[Import<IUtilitiesModule>]
[Singleton(typeof(ThemeWatcher), Factory = nameof(ThemeWatcherFactory))]
public partial class ServiceProvider
{
    public ThemeWatcher ThemeWatcherFactory() => new(Application.Current!);
}