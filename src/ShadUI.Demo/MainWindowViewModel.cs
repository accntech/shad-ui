using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI.Demo.ViewModels;

namespace ShadUI.Demo;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly ThemeWatcher _themeWatcher;
    private readonly AboutViewModel _aboutViewModel;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly ThemeViewModel _themeViewModel;
    private readonly TypographyViewModel _typographyViewModel;
    private readonly AvatarViewModel _avatarViewModel;
    private readonly BadgeViewModel _badgeViewModel;
    private readonly ButtonViewModel _buttonViewModel;
    private readonly CardViewModel _cardViewModel;
    private readonly DataTableViewModel _dataTableViewModel;
    private readonly DateViewModel _dateViewModel;
    private readonly CheckBoxViewModel _checkBoxViewModel;
    private readonly DialogViewModel _dialogViewModel;
    private readonly TimeViewModel _timeViewModel;
    private readonly InputViewModel _inputViewModel;
    private readonly NumericViewModel _numericViewModel;
    private readonly MenuViewModel _menuViewModel;
    private readonly TabControlViewModel _tabControlViewModel;
    private readonly ColorViewModel _colorViewModel;
    private readonly ComboBoxViewModel _comboBoxViewModel;
    private readonly SidebarViewModel _sidebarViewModel;
    private readonly SliderViewModel _sliderViewModel;
    private readonly SwitchViewModel _switchViewModel;
    private readonly ToastViewModel _toastViewModel;
    private readonly ToggleViewModel _toggleViewModel;
    private readonly ToolTipViewModel _toolTipViewModel;
    private readonly MiscellaneousViewModel _miscellaneousViewModel;

    public MainWindowViewModel(
        PageManager pageManager,
        DialogManager dialogManager,
        ToastManager toastManager,
        ThemeWatcher themeWatcher,
        AboutViewModel aboutViewModel,
        DashboardViewModel dashboardViewModel,
        ThemeViewModel themeViewModel,
        TypographyViewModel typographyViewModel,
        AvatarViewModel avatarViewModel,
        BadgeViewModel badgeViewModel,
        ButtonViewModel buttonViewModel,
        CardViewModel cardViewModel,
        DataTableViewModel dataTableViewModel,
        DateViewModel dateViewModel,
        CheckBoxViewModel checkBoxViewModel,
        DialogViewModel dialogViewModel,
        InputViewModel inputViewModel,
        NumericViewModel numericViewModel,
        MenuViewModel menuViewModel,
        TabControlViewModel tabControlViewModel,
        ColorViewModel colorViewModel,
        ComboBoxViewModel comboBoxViewModel,
        SidebarViewModel sidebarViewModel,
        SliderViewModel sliderViewModel,
        SwitchViewModel switchViewModel,
        TimeViewModel timeViewModel,
        ToastViewModel toastViewModel,
        ToggleViewModel toggleViewModel,
        ToolTipViewModel toolTipViewModel,
        MiscellaneousViewModel miscellaneousViewModel)
    {
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _themeWatcher = themeWatcher;
        _aboutViewModel = aboutViewModel;
        _dashboardViewModel = dashboardViewModel;
        _themeViewModel = themeViewModel;
        _typographyViewModel = typographyViewModel;
        _avatarViewModel = avatarViewModel;
        _badgeViewModel = badgeViewModel;
        _buttonViewModel = buttonViewModel;
        _cardViewModel = cardViewModel;
        _dataTableViewModel = dataTableViewModel;
        _dateViewModel = dateViewModel;
        _checkBoxViewModel = checkBoxViewModel;
        _dialogViewModel = dialogViewModel;
        _inputViewModel = inputViewModel;
        _numericViewModel = numericViewModel;
        _menuViewModel = menuViewModel;
        _tabControlViewModel = tabControlViewModel;
        _colorViewModel = colorViewModel;
        _comboBoxViewModel = comboBoxViewModel;
        _sidebarViewModel = sidebarViewModel;
        _sliderViewModel = sliderViewModel;
        _switchViewModel = switchViewModel;
        _timeViewModel = timeViewModel;
        _toastViewModel = toastViewModel;
        _toggleViewModel = toggleViewModel;
        _toolTipViewModel = toolTipViewModel;
        _miscellaneousViewModel = miscellaneousViewModel;

        pageManager.OnNavigate = SwitchPage;
    }

    [ObservableProperty]
    private string _currentRoute = "dashboard";

    [ObservableProperty]
    private DialogManager _dialogManager;

    [ObservableProperty]
    private ToastManager _toastManager;

    [ObservableProperty]
    private object? _selectedPage;

    private void SwitchPage(INavigable page, string route = "")
    {
        var pageType = page.GetType();
        if (string.IsNullOrEmpty(route)) route = pageType.GetCustomAttribute<PageAttribute>()?.Route ?? "dashboard";
        CurrentRoute = route;

        if (SelectedPage == page) return;
        SelectedPage = page;
        CurrentRoute = route;
        page.Initialize();
    }

    [RelayCommand]
    private void OpenDashboard()
    {
        SwitchPage(_dashboardViewModel);
    }

    [RelayCommand]
    private void OpenTheme()
    {
        SwitchPage(_themeViewModel);
    }

    [RelayCommand]
    private void OpenTypography()
    {
        SwitchPage(_typographyViewModel);
    }

    [RelayCommand]
    private void OpenButtons()
    {
        SwitchPage(_buttonViewModel);
    }

    [RelayCommand]
    private void OpenAvatar()
    {
        SwitchPage(_avatarViewModel);
    }

    [RelayCommand]
    private void OpenBadge()
    {
        SwitchPage(_badgeViewModel);
    }

    [RelayCommand]
    private void OpenCards()
    {
        SwitchPage(_cardViewModel);
    }

    [RelayCommand]
    private void OpenDataGrid()
    {
        SwitchPage(_dataTableViewModel);
    }

    [RelayCommand]
    private void OpenDate()
    {
        SwitchPage(_dateViewModel);
    }

    [RelayCommand]
    private void OpenCheckBoxes()
    {
        SwitchPage(_checkBoxViewModel);
    }

    [RelayCommand]
    private void OpenDialogs()
    {
        SwitchPage(_dialogViewModel);
    }

    [RelayCommand]
    private void OpenInputs()
    {
        SwitchPage(_inputViewModel);
    }

    [RelayCommand]
    private void OpenNumerics()
    {
        SwitchPage(_numericViewModel);
    }

    [RelayCommand]
    private void OpenMenus()
    {
        SwitchPage(_menuViewModel);
    }

    [RelayCommand]
    private void OpenTabs()
    {
        SwitchPage(_tabControlViewModel);
    }

    [RelayCommand]
    private void OpenComboBoxes()
    {
        SwitchPage(_comboBoxViewModel);
    }

    [RelayCommand]
    private void OpenColors()
    {
        SwitchPage(_colorViewModel);
    }

    [RelayCommand]
    private void OpenSidebar()
    {
        SwitchPage(_sidebarViewModel);
    }

    [RelayCommand]
    private void OpenSliders()
    {
        SwitchPage(_sliderViewModel);
    }

    [RelayCommand]
    private void OpenSwitch()
    {
        SwitchPage(_switchViewModel);
    }

    [RelayCommand]
    private void OpenTime()
    {
        SwitchPage(_timeViewModel);
    }

    [RelayCommand]
    private void OpenToast()
    {
        SwitchPage(_toastViewModel);
    }

    [RelayCommand]
    private void OpenToggle()
    {
        SwitchPage(_toggleViewModel);
    }

    [RelayCommand]
    private void OpenToolTip()
    {
        SwitchPage(_toolTipViewModel);
    }

    [RelayCommand]
    private void OpenMiscellaneous()
    {
        SwitchPage(_miscellaneousViewModel);
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
        SwitchPage(_dashboardViewModel);
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
}