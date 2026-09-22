using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("progress")]
public sealed partial class ProgressViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public ProgressViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "ProgressPage.axaml");
        DefaultCode = path.ExtractByLineRange(44, 44).CleanIndentation();
        LabelCode = path.ExtractByLineRange(47, 61).CleanIndentation();
        IndeterminateCode = path.ExtractByLineRange(64, 64).CleanIndentation();
        RangeCode = path.ExtractByLineRange(67, 69).CleanIndentation();
        EndpointsCode = path.ExtractByLineRange(72, 77).CleanIndentation();
        OrientationCode = path.ExtractByLineRange(80, 88).CleanIndentation();
        DisabledCode = path.ExtractByLineRange(91, 91).CleanIndentation();
    }

    [ObservableProperty]
    private double _value = 56;

    [ObservableProperty]
    private bool _isIndeterminate;

    public string DefaultCode { get; }

    public string LabelCode { get; }

    public string IndeterminateCode { get; }

    public string RangeCode { get; }

    public string EndpointsCode { get; }

    public string OrientationCode { get; }

    public string DisabledCode { get; }

    [RelayCommand]
    private void BackPage() => _pageManager.Navigate<NumericViewModel>();

    [RelayCommand]
    private void NextPage() => _pageManager.Navigate<SidebarViewModel>();
}
