using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI.Demo.ViewModels.Examples.Numeric;

namespace ShadUI.Demo.ViewModels;

[Page("numeric")]
public sealed partial class NumericViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public NumericViewModel(PageManager pageManager, FormNumericViewModel form)
    {
        _pageManager = pageManager;
        Form = form;

        var path = Path.Combine(AppContext.BaseDirectory, "views", "NumericPage.axaml");
        DefaultNumericCode = path.ExtractByLineRange(58, 60).CleanIndentation();
        DisabledCode = path.ExtractByLineRange(63, 65).CleanIndentation();
        LeftAlignedCode = path.ExtractByLineRange(68, 70).CleanIndentation();
        WithLabelCode = path.ExtractByLineRange(73, 76).CleanIndentation();
        WithCustomLabelCode = path.ExtractByLineRange(79, 90).CleanIndentation();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<MenuViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<ProgressViewModel>();
    }

    [ObservableProperty]
    private string _defaultNumericCode = string.Empty;

    [ObservableProperty]
    private string _disabledCode = string.Empty;

    [ObservableProperty]
    private string _leftAlignedCode = string.Empty;

    [ObservableProperty]
    private string _withLabelCode = string.Empty;

    [ObservableProperty]
    private string _withCustomLabelCode = string.Empty;

    [ObservableProperty]
    private FormNumericViewModel _form;

    public void Initialize()
    {
        Form.Initialize();
    }
}
