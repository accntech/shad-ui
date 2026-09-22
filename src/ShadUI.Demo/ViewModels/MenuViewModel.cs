using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("menu")]
public sealed partial class MenuViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public MenuViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "MenuPage.axaml");
        SimpleDropdownCode = path.ExtractByLineRange(58, 189).CleanIndentation();
        MenuBarCode = path.ExtractByLineRange(192, 267).CleanIndentation();
        DropDownCode = path.ExtractByLineRange(271, 434).CleanIndentation();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<LoadingViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<NumericViewModel>();
    }

    [ObservableProperty]
    private string _simpleDropdownCode = string.Empty;

    [ObservableProperty]
    private string _menuBarCode = string.Empty;

    [ObservableProperty]
    private string _dropDownCode = string.Empty;
}
