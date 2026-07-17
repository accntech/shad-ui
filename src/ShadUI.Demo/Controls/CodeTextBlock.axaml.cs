using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace ShadUI.Demo.Controls;

public class CodeTextBlock : TemplatedControl, IDisposable
{
    private static readonly Lazy<RegistryOptions> SharedRegistryOptions =
        new(() => new RegistryOptions(ThemeName.DarkPlus), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly DispatcherTimer _timer;
    private InlineCollection? _inlines = new();
    private Button? _clipboardButton;
    private PathIcon? _clipboardIcon;
    private Geometry? _originalIconData;
    private TextEditor? _editor;
    private TextMate.Installation? _textMateInstallation;
    private int _templateGeneration;
    private bool _disposed;

    public CodeTextBlock()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _timer.Tick += OnTimerTick;
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_clipboardIcon is not null) _clipboardIcon.Data = _originalIconData;
        _timer.Stop();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_disposed || _clipboardButton is null) return;

        _clipboardButton.Click -= OnClipboardButtonClick;
        _clipboardButton.Click += OnClipboardButtonClick;
    }

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<CodeTextBlock, string?>(nameof(Text));

    public static readonly DirectProperty<CodeTextBlock, InlineCollection?> InlinesProperty =
        AvaloniaProperty.RegisterDirect<CodeTextBlock, InlineCollection?>(
            nameof(Inlines), t => t.Inlines, (t, v) => t.Inlines = v);

    public static readonly StyledProperty<string> LanguageProperty =
        AvaloniaProperty.Register<CodeTextBlock, string>(nameof(Language), "xaml");

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    [Content]
    public InlineCollection? Inlines
    {
        get => _inlines;
        set => SetAndRaise(InlinesProperty, ref _inlines, value);
    }

    public string Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _templateGeneration++;
        _textMateInstallation?.Dispose();
        _textMateInstallation = null;

        if (_clipboardButton != null) _clipboardButton.Click -= OnClipboardButtonClick;

        _clipboardButton = e.NameScope.Find<Button>("PART_ClipBoardButton");
        _clipboardIcon = e.NameScope.Find<PathIcon>("PART_ClipBoardIcon");

        _editor = e.NameScope.Find<TextEditor>("PART_Editor");
        if (_editor != null)
        {
            _editor.IsReadOnly = true;
            _editor.ShowLineNumbers = true;
            _editor.TextArea.SelectionBrush = new SolidColorBrush(Color.FromRgb(51, 153, 255));
            _editor.TextArea.SelectionForeground = Brushes.White;
            _editor.Options.EnableHyperlinks = false;
            _editor.Options.EnableEmailHyperlinks = false;

            if (Inlines?.Count > 0)
            {
                _editor.Text = Inlines.Text;
            }
            else if (!string.IsNullOrEmpty(Text)) _editor.Text = Text;

            var editor = _editor;
            var language = Language;
            var templateGeneration = _templateGeneration;
            Dispatcher.UIThread.Post(() =>
            {
                if (_disposed || templateGeneration != _templateGeneration) return;
                _textMateInstallation = InstallSyntaxHighlighting(editor, language);
            }, DispatcherPriority.Background);
        }

        if (_clipboardIcon != null) _originalIconData = _clipboardIcon.Data;

        if (_clipboardButton != null) _clipboardButton.Click += OnClipboardButtonClick;
    }

    private static TextMate.Installation InstallSyntaxHighlighting(TextEditor editor, string language)
    {
        var opts = SharedRegistryOptions.Value;
        var installation = editor.InstallTextMate(opts);
        var lang = opts.GetLanguageByExtension($".{language.ToLowerInvariant()}");
        if (lang is not null) installation.SetGrammar(opts.GetScopeByLanguageId(lang.Id));
        return installation;
    }

    private async void OnClipboardButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard == null) return;
            string? textToCopy;
            if (!string.IsNullOrEmpty(Text))
            {
                textToCopy = Text;
            }
            else if (Inlines is { Count: > 0 })
            {
                textToCopy = Inlines.Text;
            }
            else
            {
                return;
            }

            await clipboard.SetTextAsync(textToCopy);

            if (_clipboardIcon == null || _originalIconData == null) return;

            // Show feedback that text was copied
            ShowCopyNotification();
            _clipboardIcon.Data = Icons.Check;

            _timer.Stop();
            _timer.Start();
        }
        catch (Exception)
        {
            //ignore
        }
    }

    private void ShowCopyNotification()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        var window = desktop.MainWindow as Window;
        var viewModel = window?.DataContext as MainWindowViewModel;

        viewModel?.ToastManager
            .CreateToast("Code copied successfully!")
            .WithContent("The code snippet has been copied to your clipboard and is ready to paste.")
            .DismissOnClick()
            .Show();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property != TextProperty) return;

        Inlines?.Clear();
        _editor?.Clear();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Cleanup(detachTimer: false);
    }

    private void Cleanup(bool detachTimer)
    {
        if (_clipboardButton != null)
            _clipboardButton.Click -= OnClipboardButtonClick;

        _timer.Stop();
        if (detachTimer) _timer.Tick -= OnTimerTick;
    }

    public void Dispose()
    {
        if (_disposed) return;

        _templateGeneration++;
        _textMateInstallation?.Dispose();
        _textMateInstallation = null;
        Cleanup(detachTimer: true);

        _disposed = true;
    }
}
