using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;

namespace ShadUI.Demo.ViewModels;

[Page("progressBar")]
public sealed partial class ProgressBarViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public ProgressBarViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;

        var xamlPath = Path.Combine(AppContext.BaseDirectory, "views", "ProgressBarPage.axaml");
        DefaultProgressBar = xamlPath.ExtractByLineRange(58, 66).CleanIndentation();
        SuccessProgressBar = xamlPath.ExtractByLineRange(72, 81).CleanIndentation();
        InfoProgressBar = xamlPath.ExtractByLineRange(87, 96).CleanIndentation();
        WarningProgressBar = xamlPath.ExtractByLineRange(102, 111).CleanIndentation();
        DangerProgressBar = xamlPath.ExtractByLineRange(117, 126).CleanIndentation();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<SidebarViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<SwitchViewModel>();
    }

    [ObservableProperty]
    private string _defaultProgressBar = string.Empty;

    [ObservableProperty]
    private string _successProgressBar = string.Empty;

    [ObservableProperty]
    private string _infoProgressBar = string.Empty;

    [ObservableProperty]
    private string _warningProgressBar = string.Empty;

    [ObservableProperty]
    private string _dangerProgressBar = string.Empty;
}