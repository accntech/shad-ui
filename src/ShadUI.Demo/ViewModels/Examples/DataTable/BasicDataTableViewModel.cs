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
        new() { Status = Status.Success, Email = "ken99@example.com", Amount = 316 },
        new() { Status = Status.Processing, Email = "alice.johnson@example.com", Amount = 542 },
        new() { Status = Status.Failed, Email = "bob.smith@example.com", Amount = 189 },
        new() { Status = Status.Success, Email = "charlie.brown@example.com", Amount = 736 },
        new() { Status = Status.Processing, Email = "diana.prince@example.com", Amount = 423 },
        new() { Status = Status.Success, Email = "ethan.hunt@example.com", Amount = 891 },
        new() { Status = Status.Failed, Email = "fiona.apple@example.com", Amount = 267 },
        new() { Status = Status.Success, Email = "george.washington@example.com", Amount = 654 },
        new() { Status = Status.Processing, Email = "helen.keller@example.com", Amount = 338 },
        new() { Status = Status.Failed, Email = "ivan.drago@example.com", Amount = 712 },
        new() { Status = Status.Success, Email = "jane.doe@example.com", Amount = 495 },
        new() { Status = Status.Processing, Email = "kevin.bacon@example.com", Amount = 823 },
        new() { Status = Status.Success, Email = "laura.croft@example.com", Amount = 156 },
        new() { Status = Status.Failed, Email = "mike.tyson@example.com", Amount = 674 },
        new() { Status = Status.Processing, Email = "nancy.drew@example.com", Amount = 381 },
        new() { Status = Status.Success, Email = "oscar.wilde@example.com", Amount = 529 },
        new() { Status = Status.Failed, Email = "peter.parker@example.com", Amount = 745 },
        new() { Status = Status.Success, Email = "quinn.reynolds@example.com", Amount = 298 },
        new() { Status = Status.Processing, Email = "rachel.green@example.com", Amount = 612 },
        new() { Status = Status.Failed, Email = "steve.jobs@example.com", Amount = 867 },
        new() { Status = Status.Success, Email = "tina.turner@example.com", Amount = 433 },
        new() { Status = Status.Processing, Email = "ulysses.grant@example.com", Amount = 756 },
        new() { Status = Status.Failed, Email = "victoria.secret@example.com", Amount = 221 },
        new() { Status = Status.Success, Email = "walter.white@example.com", Amount = 584 },
        new() { Status = Status.Processing, Email = "xena.warrior@example.com", Amount = 349 },
        new() { Status = Status.Failed, Email = "yoda.master@example.com", Amount = 793 },
        new() { Status = Status.Success, Email = "zoe.saldana@example.com", Amount = 416 },
        new() { Status = Status.Processing, Email = "adam.sandler@example.com", Amount = 678 },
        new() { Status = Status.Failed, Email = "betty.white@example.com", Amount = 205 },
        new() { Status = Status.Success, Email = "carl.sagan@example.com", Amount = 841 },
        new() { Status = Status.Processing, Email = "debbie.harry@example.com", Amount = 362 },
        new() { Status = Status.Failed, Email = "elvis.presley@example.com", Amount = 725 },
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

    [ObservableProperty] private string _xamlCode = string.Empty;

    [ObservableProperty] private string _cSharpCode = string.Empty;

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

    [ObservableProperty] private string _searchString = string.Empty;

    [ObservableProperty] private bool _isSearching;

    [ObservableProperty] private bool? _selectAll = false;

    [RelayCommand]
    private void ToggleSelection(bool? selectAll)
    {
        foreach (var item in Items)
        {
            item.IsSelected = selectAll ?? false;
        }
    }

    [ObservableProperty] private int _selectedCount;

    [ObservableProperty] private int _totalCount;

    [ObservableProperty] private ObservableCollection<DataGridItem> _items;

    [ObservableProperty] private bool _showStatusColumn = true;

    [RelayCommand]
    private void ToggleStatusColumn()
    {
        ShowStatusColumn = !ShowStatusColumn;
    }

    [ObservableProperty] private bool _showEmailColumn = true;

    [RelayCommand]
    private void ToggleEmailColumn()
    {
        ShowEmailColumn = !ShowEmailColumn;
    }

    [ObservableProperty] private bool _showAmountColumn = true;

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
    [ObservableProperty] private bool _isSelected;

    [ObservableProperty] private Status _status;

    private string _email = string.Empty;

    [EmailAddress]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value, true);
    }

    [ObservableProperty] private decimal _amount;
}

public enum Status
{
    [Display(Name = "Success")] Success,

    [Display(Name = "Processing")] Processing,

    [Display(Name = "Failed")] Failed
}