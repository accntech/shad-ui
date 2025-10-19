using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HotAvalonia;

namespace ShadUI.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Closing += OnClosing;
        Initialize();
    }

    [AvaloniaHotReload]
    private void Initialize()
    {
        ToolTip.SetTip(FullscreenButton, "Fullscreen");
        FullscreenButton.Click -= OnFullScreen;
        FullscreenButton.Click += OnFullScreen;
    }

    private void OnFullScreen(object? sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.FullScreen)
        {
            ExitFullScreen();
            ToolTip.SetTip(FullscreenButton, "Fullscreen");
        }
        else
        {
            WindowState = WindowState.FullScreen;
            ToolTip.SetTip(FullscreenButton, "Exit Fullscreen");
        }
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.TryCloseCommand.Execute(null);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Dispose DialogHost
        if (this.FindControl<DialogHost>("PART_DialogHost") is { } dialogHost)
        {
            dialogHost.Dispose();
        }

        // Dispose ViewModel
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }

        // Clear DataContext
        DataContext = null;
    }
}