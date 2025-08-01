using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("badge")]
public sealed partial class BadgeViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public BadgeViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "BadgePage.axaml");
        BadgeDemoCode = path.ExtractByLineRange(58, 101).CleanIndentation();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<AvatarViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<ButtonViewModel>();
    }

    [ObservableProperty]
    private string _badgeDemoCode = string.Empty;
}