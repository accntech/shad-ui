using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("avatar")]
public sealed partial class AvatarViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public AvatarViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "AvatarPage.axaml");
        UsageCode = path.ExtractByLineRange(59, 63).CleanIndentation();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<SmoothScrollViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<ButtonViewModel>();
    }

    [ObservableProperty]
    private string _usageCode = string.Empty;
}