using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("sidebar")]
public sealed partial class SidebarViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public SidebarViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "SidebarPage.axaml");
        DefaultCode = path.ExtractByLineRange(60, 474).CleanIndentation();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<ProgressViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<SliderViewModel>();
    }

    [ObservableProperty]
    private string _defaultCode = string.Empty;
}
