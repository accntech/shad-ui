using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI.Dialogs;
using ShadUI.Themes;
using ShadUI.Toasts;

namespace ShadUI.Demo.ViewModels;

public sealed partial class MainWindowViewModel(
    DialogManager dialogManager,
    ToastManager toastManager,
    ThemeWatcher themeWatcher,
    AboutViewModel aboutViewModel,
    DashboardViewModel dashboardViewModel,
    ThemeViewModel themeViewModel,
    TypographyViewModel typographyViewModel,
    AvatarsViewModel avatarsViewModel,
    ButtonsViewModel buttonsViewModel,
    CardsViewModel cardsViewModel,
    DataGridViewModel dataGridViewModel,
    DateViewModel dateViewModel,
    ExpandersViewModel expandersViewModel,
    CheckBoxesViewModel checkBoxesViewModel,
    DialogsViewModel dialogsViewModel,
    TimeViewModel timeViewModel,
    InputViewModel inputViewModel,
    MenuViewModel menuViewModel,
    TabsViewModel tabsViewModel,
    ComboBoxesViewModel comboBoxesViewModel,
    SlidersViewModel slidersViewModel,
    SwitchViewModel switchViewModel,
    ToastsViewModel toastsViewModel,
    TogglesViewModel togglesViewModel,
    ToolTipViewModel toolTipViewModel,
    MiscellaneousViewModel miscellaneousViewModel)
    : ViewModelBase
{
    [ObservableProperty]
    private DialogManager _dialogManager = dialogManager;

    [ObservableProperty]
    private ToastManager _toastManager = toastManager;

    [ObservableProperty]
    private object? _selectedPage;

    [ObservableProperty]
    private bool _isBusy;

    private async Task<bool> SwitchPageAsync(object page)
    {
        IsBusy = true;
        try
        {
            await Task.Delay(200); // prevent flickering

            if (SelectedPage == page) return false;

            SelectedPage = page;
            return true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenDashboard()
    {
        await SwitchPageAsync(dashboardViewModel);
        dashboardViewModel.Initialize();
    }

    [RelayCommand]
    private async Task OpenTheme()
    {
        await SwitchPageAsync(themeViewModel);
    }

    [RelayCommand]
    private async Task OpenTypography()
    {
        await SwitchPageAsync(typographyViewModel);
    }

    [RelayCommand]
    private async Task OpenButtons()
    {
        await SwitchPageAsync(buttonsViewModel);
    }

    [RelayCommand]
    private async Task OpenAvatar()
    {
        await SwitchPageAsync(avatarsViewModel);
    }

    [RelayCommand]
    private async Task OpenCards()
    {
        await SwitchPageAsync(cardsViewModel);
    }

    [RelayCommand]
    private async Task OpenDataGrid()
    {
        await SwitchPageAsync(dataGridViewModel);
    }

    [RelayCommand]
    private async Task OpenDate()
    {
        await SwitchPageAsync(dateViewModel);
    }

    [RelayCommand]
    private async Task OpenCheckBoxes()
    {
        await SwitchPageAsync(checkBoxesViewModel);
    }

    [RelayCommand]
    private async Task OpenDialogs()
    {
        await SwitchPageAsync(dialogsViewModel);
    }
    
    [RelayCommand]
    private async Task OpenExpanders()
    {
        await SwitchPageAsync(expandersViewModel);
    }

    [RelayCommand]
    private async Task OpenInputs()
    {
        if (await SwitchPageAsync(inputViewModel))
        {
            inputViewModel.Initialize();
        }
    }

    [RelayCommand]
    private async Task OpenMenus()
    {
        await SwitchPageAsync(menuViewModel);
    }

    [RelayCommand]
    private async Task OpenTabs()
    {
        await SwitchPageAsync(tabsViewModel);
    }

    [RelayCommand]
    private async Task OpenComboBoxes()
    {
        await SwitchPageAsync(comboBoxesViewModel);
    }

    [RelayCommand]
    private async Task OpenSliders()
    {
        await SwitchPageAsync(slidersViewModel);
    }

    [RelayCommand]
    private async Task OpenSwitch()
    {
        await SwitchPageAsync(switchViewModel);
    }

    [RelayCommand]
    private async Task OpenTime()
    {
        await SwitchPageAsync(timeViewModel);
    }

    [RelayCommand]
    private async Task OpenToast()
    {
        await SwitchPageAsync(toastsViewModel);
    }

    [RelayCommand]
    private async Task OpenToggle()
    {
        await SwitchPageAsync(togglesViewModel);
    }

    [RelayCommand]
    private async Task OpenToolTip()
    {
        await SwitchPageAsync(toolTipViewModel);
    }

    [RelayCommand]
    private async Task OpenMiscellaneous()
    {
        await SwitchPageAsync(miscellaneousViewModel);
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
        SelectedPage = dashboardViewModel;
        dashboardViewModel.Initialize();
    }

    [RelayCommand]
    private void ShowAbout()
    {
        DialogManager.CreateDialog(aboutViewModel)
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

        themeWatcher.SwitchTheme(CurrentTheme);
    }
}