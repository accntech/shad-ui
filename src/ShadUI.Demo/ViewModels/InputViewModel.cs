using System;
using System.IO;
using System.Timers;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI.Demo.ViewModels.Examples.Input;

namespace ShadUI.Demo.ViewModels;

[Page("input")]
public sealed partial class InputViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;
    private readonly Timer? _searchTimer;

    public InputViewModel(PageManager pageManager, FormInputViewModel inputForm)
    {
        _pageManager = pageManager;
        InputForm = inputForm;

        _searchTimer = new Timer(500); // 500ms debounce
        _searchTimer.Elapsed += SearchTimerElapsed;
        _searchTimer.AutoReset = false;

        var path = Path.Combine(AppContext.BaseDirectory, "views", "InputPage.axaml");
        DefaultInputCode = path.ExtractByLineRange(58, 60).CleanIndentation();
        DisabledCode = path.ExtractByLineRange(63, 66).CleanIndentation();
        WithLabelCode = path.ExtractByLineRange(69, 73).CleanIndentation();
        WithCustomLabelCode = path.ExtractByLineRange(76, 80).CleanIndentation();
        SearchBoxCode = path.ExtractByLineRange(83, 95).CleanIndentation();
        AutoCompleteBoxCode = path.ExtractByLineRange(98, 113).CleanIndentation();
        TextAreaCode = path.ExtractByLineRange(116, 121).CleanIndentation();
        FormValidationCode = path.ExtractByLineRange(124, 159).CleanIndentation();
    }

    [RelayCommand]
    private void BackPage()
    {
        _pageManager.Navigate<DialogViewModel>();
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<MenuViewModel>();
    }

    private void SearchTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            IsSearching = false;
            _searchTimer?.Stop();
        });
    }

    [ObservableProperty]
    private string _defaultInputCode = string.Empty;

    [ObservableProperty]
    private string _disabledCode = string.Empty;

    [ObservableProperty]
    private string _withLabelCode = string.Empty;

    [ObservableProperty]
    private string _withCustomLabelCode = string.Empty;

    [ObservableProperty]
    private string _formValidationCode = string.Empty;

    [ObservableProperty]
    private string _searchString = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private string _searchBoxCode = string.Empty;

    [ObservableProperty]
    private string[] _webFrameworks =
    [
        "Angular",
        "Astro",
        "Lit",
        "Next.js",
        "Nuxt.js",
        "Preact",
        "Qwik",
        "React",
        "Remix",
        "SolidJS",
        "Svelte",
        "SvelteKit",
        "Vue.js"
    ];

    [ObservableProperty]
    private string _autoCompleteBoxCode = string.Empty;

    [ObservableProperty]
    private string _textAreaCode = string.Empty;

    [ObservableProperty]
    private FormInputViewModel _inputForm;

    public void Initialize()
    {
        InputForm.Initialize();
    }

    public override void Dispose()
    {
        if (_searchTimer != null)
        {
            _searchTimer.Stop();
            _searchTimer.Elapsed -= SearchTimerElapsed;
            _searchTimer.Dispose();
        }

        base.Dispose();
    }
}