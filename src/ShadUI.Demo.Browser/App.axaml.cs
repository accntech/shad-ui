using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ShadUI.Demo;
using ShadUI.Demo.Browser.Views;

namespace ShadUI.Demo.Browser;

public sealed class App : Application
{
    private BrowserNavigationCoordinator? _navigation;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ISingleViewApplicationLifetime browser)
        {
            var provider = new ShadUI.Demo.ServiceProvider().RegisterDialogs();
            var themeWatcher = provider.GetService<ThemeWatcher>();
            themeWatcher.Initialize();

            var viewModel = provider.GetService<ShadUI.Demo.MainWindowViewModel>();
            _navigation = new BrowserNavigationCoordinator(viewModel);
            _navigation.Initialize();
            browser.MainView = new MainView { DataContext = viewModel };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
