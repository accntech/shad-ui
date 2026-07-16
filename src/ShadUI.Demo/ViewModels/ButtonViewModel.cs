using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("button")]
public sealed partial class ButtonViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public ButtonViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "ButtonPage.axaml");
        PrimaryCode = path.ExtractByLineRange(58, 83).CleanIndentation();
        SecondaryCode = path.ExtractByLineRange(86, 111).CleanIndentation();
        DestructiveCode = path.ExtractByLineRange(114, 139).CleanIndentation();
        OutlineCode = path.ExtractByLineRange(142, 167).CleanIndentation();
        GhostCode = path.ExtractByLineRange(170, 195).CleanIndentation();
        IconCode = path.ExtractByLineRange(198, 225).CleanIndentation();
        DestructiveIconCode = path.ExtractByLineRange(228, 251).CleanIndentation();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<BadgeViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<CardViewModel>();
    }

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    private async Task SubmitAsync()
    {
        IsBusy = true;
        await Task.Delay(5000);
        IsBusy = false;
    }

    [ObservableProperty]
    private string _primaryCode = string.Empty;

    [ObservableProperty]
    private string _secondaryCode = string.Empty;

    [ObservableProperty]
    private string _destructiveCode = string.Empty;

    [ObservableProperty]
    private string _outlineCode = string.Empty;

    [ObservableProperty]
    private string _ghostCode = string.Empty;

    [ObservableProperty]
    private string _iconCode = string.Empty;

    [ObservableProperty]
    private string _destructiveIconCode = string.Empty;
}
