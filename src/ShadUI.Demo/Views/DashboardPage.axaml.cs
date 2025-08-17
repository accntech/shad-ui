using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LiveChartsCore.Kernel;
using ShadUI.Demo.ViewModels;

namespace ShadUI.Demo.Views;

public partial class DashboardPage : UserControl
{
    public DashboardPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        _viewModel.ThemeWatcher.ThemeChanged -= OnThemeChanged;
    }

    private DashboardViewModel _viewModel = null!;

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DashboardViewModel vm) return;

        _viewModel = vm;
        _viewModel.ThemeWatcher.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeColors e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            CartesianChart1.CoreChart.Update(new ChartUpdateParams 
                { IsAutomaticUpdate = false, Throttling = false });
            CartesianChart2.CoreChart.Update(new ChartUpdateParams 
                { IsAutomaticUpdate = false, Throttling = false });
        });
    }
}