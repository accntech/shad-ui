using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("loading")]
public sealed partial class LoadingViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public LoadingViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "LoadingPage.axaml");
        DefaultCode = path.ExtractByLineRange(39, 43).CleanIndentation();
        DeterminateCode = path.ExtractByLineRange(46, 59).CleanIndentation();
        ControlledCode = path.ExtractByLineRange(62, 73).CleanIndentation();
        RangeCode = path.ExtractByLineRange(76, 81).CleanIndentation();
    }

    [ObservableProperty]
    private double _value = 56;

    [ObservableProperty]
    private bool _isIndeterminate;

    public string DefaultCode { get; }

    public string DeterminateCode { get; }

    public string ControlledCode { get; }

    public string RangeCode { get; }

    [RelayCommand]
    private void BackPage() => _pageManager.Navigate<InputViewModel>();

    [RelayCommand]
    private void NextPage() => _pageManager.Navigate<MenuViewModel>();
}
