using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI.Dialogs;
using ShadUI.Toasts;

namespace ShadUI.Demo.ViewModels;

public sealed partial class MainWindowViewModel(
    DialogManager dialogManager,
    ToastManager toastManager,
    DashboardViewModel dashboardViewModel,
    TypographyViewModel typographyViewModel,
    AvatarsViewModel avatarsViewModel,
    ButtonsViewModel buttonsViewModel,
    CardsViewModel cardsViewModel,
    DialogsViewModel dialogsViewModel,
    InputViewModel inputViewModel,
    TabsViewModel tabsViewModel,
    ComboBoxesViewModel comboBoxesViewModel,
    SlidersViewModel slidersViewModel,
    TimeControlViewModel timeControlViewModel,
    ToastsViewModel toastsViewModel,
    ToggleSwitchViewModel toggleSwitchViewModel,
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
    private async Task OpenDialogs()
    {
        await SwitchPageAsync(dialogsViewModel);
    }

    [RelayCommand]
    private async Task OpenInputs()
    {
        if (await SwitchPageAsync(inputViewModel))
            inputViewModel.Initialize();
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
    private async Task OpenTimePicker()
    {
        await SwitchPageAsync(timeControlViewModel);
    }
    
    [RelayCommand]
    private async Task OpenToggleSwitch()
    {
        await SwitchPageAsync(toggleSwitchViewModel);
    }

    [RelayCommand]
    private async Task OpenToast()
    {
        await SwitchPageAsync(toastsViewModel);
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
            Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Process.Start("xdg-open", url);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Process.Start("open", url);
    }

    public void Initialize()
    {
        SelectedPage = dashboardViewModel;
        dashboardViewModel.Initialize();
    }
}