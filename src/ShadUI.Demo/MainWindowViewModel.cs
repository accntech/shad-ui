using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI.Demo.ViewModels;
using ShadUI.Dialogs;
using ShadUI.Themes;
using ShadUI.Toasts;

namespace ShadUI.Demo;

public sealed partial class MainWindowViewModel(
    DialogManager dialogManager,
    ToastManager toastManager,
    ThemeWatcher themeWatcher,
    AboutViewModel aboutViewModel,
    DashboardViewModel dashboardViewModel,
    ThemeViewModel themeViewModel,
    TypographyViewModel typographyViewModel,
    AvatarViewModel avatarViewModel,
    ButtonsViewModel buttonsViewModel,
    CardsViewModel cardsViewModel,
    DataGridViewModel dataGridViewModel,
    DateViewModel dateViewModel,
    AccordionViewModel accordionViewModel,
    CheckBoxesViewModel checkBoxesViewModel,
    DialogsViewModel dialogsViewModel,
    TimeViewModel timeViewModel,
    InputViewModel inputViewModel,
    NumericViewModel numericViewModel,
    MenuViewModel menuViewModel,
    TabsViewModel tabsViewModel,
    ColorsViewModel colorsViewModel,
    ComboBoxesViewModel comboBoxesViewModel,
    SlidersViewModel slidersViewModel,
    SwitchViewModel switchViewModel,
    ToastsViewModel toastsViewModel,
    ToggleViewModel toggleViewModel,
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

    private async Task<bool> SwitchPageAsync(object page)
    {
        if (SelectedPage == page) return false;

        await Task.Delay(200); // prevent animate delay
        SelectedPage = page;
        return true;
    }

    [RelayCommand]
    private async Task OpenDashboard()
    {
        if (await SwitchPageAsync(dashboardViewModel))
        {
            dashboardViewModel.Initialize();
        }
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
        await SwitchPageAsync(avatarViewModel);
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
    private async Task OpenAccordion()
    {
        await SwitchPageAsync(accordionViewModel);
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
    private async Task OpenNumerics()
    {
        if (await SwitchPageAsync(numericViewModel))
        {
            numericViewModel.Initialize();
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
    private async Task OpenColors()
    {
        await SwitchPageAsync(colorsViewModel);
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
        await SwitchPageAsync(toggleViewModel);
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