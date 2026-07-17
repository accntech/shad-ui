using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI.Demo.ViewModels;

namespace ShadUI.Demo;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly PageManager _pageManager;
    private readonly ThemeWatcher _themeWatcher;
    private readonly AboutViewModel _aboutViewModel;
    private bool _disposed;

    public MainWindowViewModel(
        PageManager pageManager,
        DialogManager dialogManager,
        ToastManager toastManager,
        ThemeWatcher themeWatcher,
        AboutViewModel aboutViewModel)
    {
        _pageManager = pageManager;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _themeWatcher = themeWatcher;
        _aboutViewModel = aboutViewModel;

        pageManager.RegisterNavigationHandler(SwitchPage);
    }

    [ObservableProperty]
    private string _currentRoute = "dashboard";

    [ObservableProperty]
    private DialogManager _dialogManager;

    [ObservableProperty]
    private ToastManager _toastManager;

    [ObservableProperty]
    private object? _selectedPage;

    private void SwitchPage(INavigable page, string route)
    {
        if (SelectedPage == page) return;

        SelectedPage = page;
        CurrentRoute = route;
        page.Initialize();
    }

    [RelayCommand]
    private void OpenDashboard()
    {
        _pageManager.Navigate<DashboardViewModel>();
    }

    [RelayCommand]
    private void OpenTheme()
    {
        _pageManager.Navigate<ThemeViewModel>();
    }

    [RelayCommand]
    private void OpenTypography()
    {
        _pageManager.Navigate<TypographyViewModel>();
    }

    [RelayCommand]
    private void OpenSmoothScroll()
    {
        _pageManager.Navigate<SmoothScrollViewModel>();
    }

    [RelayCommand]
    private void OpenButtons()
    {
        _pageManager.Navigate<ButtonViewModel>();
    }

    [RelayCommand]
    private void OpenAvatar()
    {
        _pageManager.Navigate<AvatarViewModel>();
    }

    [RelayCommand]
    private void OpenBadge()
    {
        _pageManager.Navigate<BadgeViewModel>();
    }

    [RelayCommand]
    private void OpenCards()
    {
        _pageManager.Navigate<CardViewModel>();
    }

    [RelayCommand]
    private void OpenDataGrid()
    {
        _pageManager.Navigate<DataTableViewModel>();
    }

    [RelayCommand]
    private void OpenDate()
    {
        _pageManager.Navigate<DateViewModel>();
    }

    [RelayCommand]
    private void OpenCheckBoxes()
    {
        _pageManager.Navigate<CheckBoxViewModel>();
    }

    [RelayCommand]
    private void OpenDialogs()
    {
        _pageManager.Navigate<DialogViewModel>();
    }

    [RelayCommand]
    private void OpenInputs()
    {
        _pageManager.Navigate<InputViewModel>();
    }

    [RelayCommand]
    private void OpenNumerics()
    {
        _pageManager.Navigate<NumericViewModel>();
    }

    [RelayCommand]
    private void OpenMenus()
    {
        _pageManager.Navigate<MenuViewModel>();
    }

    [RelayCommand]
    private void OpenTabs()
    {
        _pageManager.Navigate<TabControlViewModel>();
    }

    [RelayCommand]
    private void OpenComboBoxes()
    {
        _pageManager.Navigate<ComboBoxViewModel>();
    }

    [RelayCommand]
    private void OpenColors()
    {
        _pageManager.Navigate<ColorViewModel>();
    }

    [RelayCommand]
    private void OpenSidebar()
    {
        _pageManager.Navigate<SidebarViewModel>();
    }

    [RelayCommand]
    private void OpenSliders()
    {
        _pageManager.Navigate<SliderViewModel>();
    }

    [RelayCommand]
    private void OpenSwitch()
    {
        _pageManager.Navigate<SwitchViewModel>();
    }

    [RelayCommand]
    private void OpenTime()
    {
        _pageManager.Navigate<TimeViewModel>();
    }

    [RelayCommand]
    private void OpenToast()
    {
        _pageManager.Navigate<ToastViewModel>();
    }

    [RelayCommand]
    private void OpenToggle()
    {
        _pageManager.Navigate<ToggleViewModel>();
    }

    [RelayCommand]
    private void OpenToolTip()
    {
        _pageManager.Navigate<ToolTipViewModel>();
    }

    [RelayCommand]
    private void OpenMiscellaneous()
    {
        _pageManager.Navigate<MiscellaneousViewModel>();
    }

    [RelayCommand]
    private void OpenUrl(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
    }

    public void Initialize()
    {
        _pageManager.Navigate<DashboardViewModel>();
    }

    [RelayCommand]
    private void ShowAbout()
    {
        DialogManager.CreateDialog(_aboutViewModel)
            .WithMinWidth(300)
            .Dismissible()
            .Show();
    }

    [RelayCommand]
    private void TryClose()
    {
        DialogManager.CreateDialog("Close", "Do you really want to exit?")
            .WithPrimaryButton("Yes", OnAcceptExit)
            .WithCancelButton("No")
            .WithMinWidth(300)
            .Show();
    }

    private void OnAcceptExit()
    {
        Environment.Exit(0);
    }

    private ThemeMode _currentTheme;

    public ThemeMode CurrentTheme
    {
        get => _currentTheme;
        private set => SetProperty(ref _currentTheme, value);
    }

    [RelayCommand]
    private void SwitchTheme()
    {
        CurrentTheme = CurrentTheme switch
        {
            ThemeMode.System => ThemeMode.Light,
            ThemeMode.Light => ThemeMode.Dark,
            _ => ThemeMode.System
        };

        _themeWatcher.SwitchTheme(CurrentTheme);
    }

    /// <summary>
    ///     Disposes the MainWindowViewModel and cleans up all pages and resources.
    /// </summary>
    public override void Dispose()
    {
        if (_disposed) return;

        _pageManager.UnregisterNavigationHandler(SwitchPage);
        _pageManager.Dispose();
        DialogManager.Dispose();

        _disposed = true;
        base.Dispose();
    }
}
