using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("smoothScroll")]
public sealed partial class SmoothScrollViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public SmoothScrollViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "SmoothScrollPage.axaml");
        UsageCode = path.ExtractByLineRange(86, 114).CleanIndentation();
        
        for (int i = 1; i <= 300; i++)
        {
            ScrollItems.Add($"#{i}");
        }
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<TypographyViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<AvatarViewModel>();
    }

    [ObservableProperty]
    private string _usageCode = string.Empty;
    
    
    public ObservableCollection<string> ScrollItems { get; } = [];

    [ObservableProperty]
    private bool _isEnabled = true;

    [ObservableProperty]
    private double? _baseStepSize = SmoothScrollAssist.BaseStepSizeProperty.GetDefaultValue(typeof(ScrollViewer));

    [ObservableProperty]
    private double? _smoothingFactor = SmoothScrollAssist.SmoothingFactorProperty.GetDefaultValue(typeof(ScrollViewer));
}