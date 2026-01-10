using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Timers;
using Avalonia.Threading;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels.Examples.DataTable;

public sealed partial class BasicDataTableViewModel : ViewModelBase
{
    private readonly List<DataGridItem> _originalItems =
    [
        new() { Status = Status.Success, Email = "abe45@example.com", Amount = 242 },
        new() { Status = Status.Processing, Email = "monserrat44@example.com", Amount = 837 },
        new() { Status = Status.Success, Email = "silas22@example.com", Amount = 874 },
        new() { Status = Status.Failed, Email = "carmella@example.com", Amount = 721 },
        new() { Status = Status.Success, Email = "ken99@example.com", Amount = 316 }
    ];

    private readonly Timer? _searchTimer;

    public BasicDataTableViewModel()
    {
        var xamlPath = Path.Combine(AppContext.BaseDirectory, "views", "Examples", "DataTable",
            "BasicDataTableContent.axaml");
        XamlCode = xamlPath.ExtractByLineRange(1, 163).CleanIndentation();

        var csharpPath = Path.Combine(AppContext.BaseDirectory, "viewModels", "Examples", "DataTable",
            "BasicDataTableViewModel.cs");
        CSharpCode = csharpPath.ExtractWithSkipRanges((31, 37), (48, 54)).CleanIndentation();

        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        PropertyChanged += OnPropertyChanged;

        foreach (var i in _originalItems) i.PropertyChanged += OnItemsChanged;
        Items = new ObservableCollection<DataGridItem>(_originalItems);
    }

    [ObservableProperty]
    private string _xamlCode = string.Empty;

    [ObservableProperty]
    private string _cSharpCode = string.Empty;

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SearchString))
        {
            if (SearchString.Length > 0)
            {
                IsSearching = true;
                _searchTimer?.Stop();
                _searchTimer?.Start();
            }
            else
            {
                _searchTimer?.Stop();
                IsSearching = false;
                Items.Clear();
                Items.AddRange(_originalItems);
                UpdateTotal();
            }
        }
    }

    private void SearchTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var filteredItems = _originalItems
                .Where(item => item.Email.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Items.Clear();
            Items.AddRange(filteredItems);

            IsSearching = false;
            _searchTimer?.Stop();
            UpdateTotal();
        });
    }

    private void OnItemsChanged(object? sender, PropertyChangedEventArgs e)
    {
        var selectedAll = Items.All(item => item.IsSelected);
        var notSelectedCount = Items.Count(item => !item.IsSelected);

        if (selectedAll)
        {
            SelectAll = true;
        }
        else if (notSelectedCount == Items.Count)
        {
            SelectAll = false;
        }
        else
        {
            SelectAll = null;
        }

        UpdateTotal();
    }

    private void UpdateTotal()
    {
        TotalCount = Items.Count;
        SelectedCount = Items.Count(item => item.IsSelected);
    }

    [ObservableProperty]
    private string _searchString = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private bool? _selectAll = false;

    [RelayCommand]
    private void ToggleSelection(bool? selectAll)
    {
        foreach (var item in Items)
        {
            item.IsSelected = selectAll ?? false;
        }
    }

    [ObservableProperty]
    private int _selectedCount;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private ObservableCollection<DataGridItem> _items;

    [ObservableProperty]
    private bool _showStatusColumn = true;

    [RelayCommand]
    private void ToggleStatusColumn()
    {
        ShowStatusColumn = !ShowStatusColumn;
    }

    [ObservableProperty]
    private bool _showEmailColumn = true;

    [RelayCommand]
    private void ToggleEmailColumn()
    {
        ShowEmailColumn = !ShowEmailColumn;
    }

    [ObservableProperty]
    private bool _showAmountColumn = true;

    [RelayCommand]
    private void ToggleAmountColumn()
    {
        ShowAmountColumn = !ShowAmountColumn;
    }

    public override void Dispose()
    {
        if (_searchTimer != null)
        {
            _searchTimer.Stop();
            _searchTimer.Elapsed -= SearchTimerElapsed;
            _searchTimer.Dispose();
        }

        PropertyChanged -= OnPropertyChanged;

        foreach (var item in _originalItems)
            item.PropertyChanged -= OnItemsChanged;

        base.Dispose();
    }
}

public sealed partial class DataGridItem : ObservableValidator
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private Status _status;

    private string _email = string.Empty;

    [EmailAddress]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value, true);
    }

    [ObservableProperty]
    private decimal _amount;
}

public enum Status
{
    [Display(Name = "Success")]
    Success,

    [Display(Name = "Processing")]
    Processing,

    [Display(Name = "Failed")]
    Failed
}