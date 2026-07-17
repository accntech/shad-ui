using System;
using System.ComponentModel;
using Avalonia.Threading;
using ShadUI.Demo;

namespace ShadUI.Demo.Browser;

internal sealed class BrowserNavigationCoordinator : IDisposable
{
    private readonly MainWindowViewModel _viewModel;
    private readonly DispatcherTimer _historyTimer;
    private bool _applyingBrowserRoute;
    private bool _disposed;

    public BrowserNavigationCoordinator(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        _historyTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, OnHistoryTick);
    }

    public void Initialize()
    {
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        var initialRoute = BrowserHistory.GetRoute();
        _applyingBrowserRoute = true;
        try
        {
            if (!_viewModel.NavigateToRoute(initialRoute))
            {
                _viewModel.NavigateToRoute("dashboard");
            }

            BrowserHistory.SetRoute(_viewModel.CurrentRoute, replace: true);
        }
        finally
        {
            _applyingBrowserRoute = false;
        }

        _historyTimer.Start();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_applyingBrowserRoute || e.PropertyName != nameof(MainWindowViewModel.CurrentRoute)) return;

        if (!RoutesMatch(BrowserHistory.GetRoute(), _viewModel.CurrentRoute))
        {
            BrowserHistory.SetRoute(_viewModel.CurrentRoute, replace: false);
        }
    }

    private void OnHistoryTick(object? sender, EventArgs e)
    {
        var browserRoute = BrowserHistory.GetRoute();
        if (RoutesMatch(browserRoute, _viewModel.CurrentRoute)) return;

        _applyingBrowserRoute = true;
        try
        {
            if (!_viewModel.NavigateToRoute(browserRoute))
            {
                BrowserHistory.SetRoute(_viewModel.CurrentRoute, replace: true);
                return;
            }

            if (!string.Equals(browserRoute, _viewModel.CurrentRoute, StringComparison.Ordinal))
            {
                BrowserHistory.SetRoute(_viewModel.CurrentRoute, replace: true);
            }
        }
        finally
        {
            _applyingBrowserRoute = false;
        }
    }

    private static bool RoutesMatch(string left, string right)
    {
        return string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string route)
    {
        return route.Trim().Trim('/').Replace("-", string.Empty, StringComparison.Ordinal);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _historyTimer.Stop();
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _disposed = true;
    }
}
