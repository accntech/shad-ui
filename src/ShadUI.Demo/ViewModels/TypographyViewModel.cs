using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI.Demo.ViewModels.Examples.Typography;

namespace ShadUI.Demo.ViewModels;

[Page("typography")]
public sealed partial class TypographyViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;
    private readonly TextBlockViewModel _textBlock;
    private readonly SelectableTextBlockViewModel _selectableTextBlock;
    private readonly LabelViewModel _label;

    public TypographyViewModel(
        PageManager pageManager,
        TextBlockViewModel textBlock,
        SelectableTextBlockViewModel selectableTextBlock,
        LabelViewModel label)
    {
        _pageManager = pageManager;
        _textBlock = textBlock;
        _selectableTextBlock = selectableTextBlock;
        _label = label;
        PropertyChanged += OnPropertyChanged;

        Content = _textBlock;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedControl))
        {
            switch (SelectedControl)
            {
                case "Text Block":
                    Content = _textBlock;
                    break;
                case "Selectable Text Block":
                    Content = _selectableTextBlock;
                    break;
                case "Label":
                    Content = _label;
                    break;
            }
        }
    }

    [ObservableProperty]
    private string _selectedControl = "Text Block";

    [ObservableProperty]
    private string[] _controls =
    [
        "Text Block",
        "Selectable Text Block",
        "Label"
    ];

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<ThemeViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<SmoothScrollViewModel>();
    }

    [ObservableProperty]
    private object _content = null!;
}