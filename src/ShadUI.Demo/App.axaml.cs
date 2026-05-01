using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

namespace ShadUI.Demo;

public class App : Application
{
    // ReSharper disable once NotAccessedField.Local
    private static Mutex? _appMutex;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        _appMutex = new Mutex(true, "ShadUISingleInstanceMutex", out var createdNew);
        if (!createdNew)
        {
            var instanceDialog = new InstanceDialog();
            instanceDialog.Show();
            return;
        }

        var provider = new ServiceProvider().RegisterDialogs();

        var themeWatcher = provider.GetService<ThemeWatcher>();
        themeWatcher.Initialize();
        var viewModel = provider.GetService<MainWindowViewModel>();
        viewModel.Initialize();

        var mainWindow = new MainWindow { DataContext = viewModel };
        this.RegisterTrayIconsEvents(mainWindow, viewModel);

        desktop.MainWindow = mainWindow;
        base.OnFrameworkInitializationCompleted();
    }
}