using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

[Page("checkbox")]
public partial class CheckBoxViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;

    public CheckBoxViewModel(PageManager pageManager)
    {
        _pageManager = pageManager;
        var path = Path.Combine(AppContext.BaseDirectory, "views", "CheckBoxPage.axaml");
        DefaultCode = path.ExtractByLineRange(58, 60).CleanIndentation();
        DisabledCode = path.ExtractByLineRange(63, 65).CleanIndentation();
        IndeterminateCode = path.ExtractByLineRange(68, 70).CleanIndentation();
        MultipleCode = path.ExtractByLineRange(73, 92).CleanIndentation();

        Items.CollectionChanged += OnItemsCollectionChanged;
        foreach (var item in Items) item.PropertyChanged += OnItemPropertyChanged;
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (CheckBoxItem item in e.NewItems)
                item.PropertyChanged += OnItemPropertyChanged;
        }

        if (e.OldItems != null)
        {
            foreach (CheckBoxItem item in e.OldItems)
                item.PropertyChanged -= OnItemPropertyChanged;
        }

        UpdateSelectAllState();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        UpdateSelectAllState();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<CardViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<ColorViewModel>();
    }

    [ObservableProperty]
    private string _defaultCode = string.Empty;

    [ObservableProperty]
    private string _disabledCode = string.Empty;

    [ObservableProperty]
    private string _indeterminateCode = string.Empty;

    [ObservableProperty]
    private string _multipleCode = string.Empty;

    partial void OnIsCheckedChanged(bool? value)
    {
        if (!value.HasValue) return;

        foreach (var item in Items)
        {
            item.IsChecked = value.Value;
        }
    }

    private void UpdateSelectAllState()
    {
        IsChecked = Items.All(i => i.IsChecked == true) ? true :
            Items.All(i => i.IsChecked == false) ? false : null;
    }

    [ObservableProperty]
    private bool? _isChecked = false;

    [ObservableProperty]
    private ObservableCollection<CheckBoxItem> _items =
    [
        new() { IsChecked = false, Text = "Recents" },
        new() { IsChecked = false, Text = "Home" },
        new() { IsChecked = false, Text = "Applications" },
        new() { IsChecked = false, Text = "Desktop" },
        new() { IsChecked = false, Text = "Downloads" },
        new() { IsChecked = false, Text = "Documents" }
    ];

    public override void Dispose()
    {
        Items.CollectionChanged -= OnItemsCollectionChanged;

        foreach (var item in Items)
            item.PropertyChanged -= OnItemPropertyChanged;

        base.Dispose();
    }
}

public partial class CheckBoxItem : ObservableObject
{
    [ObservableProperty]
    private bool? _isChecked;

    [ObservableProperty]
    private string _text = string.Empty;
}