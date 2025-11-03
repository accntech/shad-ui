using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace ShadUI.Demo.ViewModels;

[Page("toast")]
public sealed partial class ToastViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;
    private readonly ToastManager _toastManager;

    public ToastViewModel(PageManager pageManager, ToastManager toastManager)
    {
        _pageManager = pageManager;
        _toastManager = toastManager;
        var path = Path.Combine(AppContext.BaseDirectory, "viewModels", "ToastViewModel.cs");
        SimpleToastCode = WrapCode(path.ExtractByLineRange(54, 58).CleanIndentation());
        WithTitleToastCode = WrapCode(path.ExtractByLineRange(63, 69).CleanIndentation());
        WithActionToastCode = WrapCode(path.ExtractByLineRange(74, 81).CleanIndentation());
        WithDelayToastCode = WrapCode(path.ExtractByLineRange(86, 93).CleanIndentation());
        InfoToastCode = WrapCode(path.ExtractByLineRange(98, 105).CleanIndentation());
        SuccessToastCode = WrapCode(path.ExtractByLineRange(110, 117).CleanIndentation());
        WarningToastCode = WrapCode(path.ExtractByLineRange(122, 129).CleanIndentation());
        ErrorToastCode = WrapCode(path.ExtractByLineRange(134, 141).CleanIndentation());
        DynamicToastCode = WrapCode(path.ExtractByLineRange(148, 173).CleanIndentation());
        DynamicToastCodeWithProgressBar = WrapCode(path.ExtractByLineRange(175, 197).CleanIndentation());
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<TimeViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<ToggleViewModel>();
    }

    private string WrapCode(string code)
    {
        return $"""
                using CommunityToolkit.Mvvm.Input;

                //..other code

                {code}

                //..rest of the code
                """;
    }

    [RelayCommand]
    private void ShowSimpleToast()
    {
        _toastManager.CreateToast("Your message has been sent.").Show();
    }

    [ObservableProperty]
    private string _simpleToastCode = string.Empty;

    [RelayCommand]
    private void ShowToastWithTitle()
    {
        _toastManager.CreateToast("Uh oh! Something went wrong.")
            .WithContent("There was a problem with your request.")
            .Show();
    }

    [ObservableProperty]
    private string _withTitleToastCode = string.Empty;

    [RelayCommand]
    private void ShowToastWithAction()
    {
        _toastManager.CreateToast("Uh oh! Something went wrong.")
            .WithContent("There was a problem with your request.")
            .WithAction("Try again", () => _toastManager.CreateToast("Retry clicked").Show())
            .Show();
    }

    [ObservableProperty]
    private string _withActionToastCode = string.Empty;

    [RelayCommand]
    private void ShowToastWithDelay()
    {
        _toastManager.CreateToast("This toast will disappear in 5 seconds.")
            .WithContent("You can hover over it to prevent it from disappearing.")
            .WithDelay(5)
            .Show();
    }

    [ObservableProperty]
    private string _withDelayToastCode = string.Empty;

    [RelayCommand]
    private void ShowInfoToast()
    {
        _toastManager.CreateToast("Event has been created")
            .WithContent($"{DateTime.Now:dddd, MMMM d 'at' h:mm tt}")
            .DismissOnClick()
            .ShowInfo();
    }

    [ObservableProperty]
    private string _infoToastCode = string.Empty;

    [RelayCommand]
    private void ShowSuccessToast()
    {
        _toastManager.CreateToast("Event has been created successfully")
            .WithContent($"{DateTime.Now:dddd, MMMM d 'at' h:mm tt}")
            .DismissOnClick()
            .ShowSuccess();
    }

    [ObservableProperty]
    private string _successToastCode = string.Empty;

    [RelayCommand]
    private void ShowWarningToast()
    {
        _toastManager.CreateToast("Event has been created with warnings")
            .WithContent($"{DateTime.Now:dddd, MMMM d 'at' h:mm tt}")
            .DismissOnClick()
            .ShowWarning();
    }

    [ObservableProperty]
    private string _warningToastCode = string.Empty;

    [RelayCommand]
    private void ShowErrorToast()
    {
        _toastManager.CreateToast("Failed to create event")
            .WithContent("Unable to connect to the server.")
            .WithAction("Retry", () => _toastManager.CreateToast("Retry clicked").Show())
            .ShowError();
    }

    [ObservableProperty]
    private string _errorToastCode = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Notification> _notificationsCollection =
    [
        Notification.Basic,
        Notification.Info,
        Notification.Success,
        Notification.Warning,
        Notification.Error
    ];

    [ObservableProperty]
    private Notification _selectedNotification;

    [ObservableProperty]
    private string _dynamicToastCode = string.Empty;

    [RelayCommand]
    private void ShowToastWithType()
    {
        _toastManager.CreateToast("Your message has been sent.")
            .WithContent($"{DateTime.Now:dddd, MMMM d 'at' h:mm tt}")
            .WithAction("Retry", () => _toastManager.CreateToast("Retry clicked").Show(Notification.Success))
            .DismissOnClick()
            .Show(SelectedNotification);
    }

    [ObservableProperty]
    private Notification _selectedNotificationWithProgressBar;

    [ObservableProperty]
    private bool _isProgressIndeterminate = true;

    [ObservableProperty]
    private int _progressValue = 30;

    [ObservableProperty]
    private string _dynamicToastCodeWithProgressBar = string.Empty;

    [RelayCommand]
    private async Task ShowToastWithProgressBar()
    {
        _toastManager.CreateToast("Your message has been sent.")
            .WithContent($"{DateTime.Now:dddd, MMMM d 'at' h:mm tt}")
            .WithAction("Retry", () => _toastManager.CreateToast("Retry clicked").Show(Notification.Success))
            .DismissOnClick()
            .WithProgressBar(IsProgressIndeterminate, ProgressValue)
            .Show(SelectedNotificationWithProgressBar);
    }
}