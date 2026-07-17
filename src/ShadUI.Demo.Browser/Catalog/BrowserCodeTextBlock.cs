using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;

namespace ShadUI.Demo.Controls;

/// <summary>
/// Lightweight source-code viewer used by the WebAssembly catalog.
/// AvaloniaEdit and TextMate are intentionally omitted because their initialization
/// can monopolize the browser's single UI thread.
/// </summary>
public sealed class CodeTextBlock : TemplatedControl, IDisposable
{
    private static readonly IBrush PlainBrush = new SolidColorBrush(Color.Parse("#D4D4D4"));
    private static readonly IBrush KeywordBrush = new SolidColorBrush(Color.Parse("#569CD6"));
    private static readonly IBrush TypeBrush = new SolidColorBrush(Color.Parse("#4EC9B0"));
    private static readonly IBrush AttributeBrush = new SolidColorBrush(Color.Parse("#9CDCFE"));
    private static readonly IBrush StringBrush = new SolidColorBrush(Color.Parse("#CE9178"));
    private static readonly IBrush CommentBrush = new SolidColorBrush(Color.Parse("#6A9955"));
    private static readonly IBrush NumberBrush = new SolidColorBrush(Color.Parse("#B5CEA8"));
    private static readonly IBrush PunctuationBrush = new SolidColorBrush(Color.Parse("#808080"));

    private readonly DispatcherTimer _timer;
    private InlineCollection? _inlines = new();
    private Button? _clipboardButton;
    private PathIcon? _clipboardIcon;
    private SelectableTextBlock? _codeText;
    private Geometry? _originalIconData;
    private bool _disposed;

    public CodeTextBlock()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _timer.Tick += OnTimerTick;
    }

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<CodeTextBlock, string?>(nameof(Text));

    public static readonly DirectProperty<CodeTextBlock, InlineCollection?> InlinesProperty =
        AvaloniaProperty.RegisterDirect<CodeTextBlock, InlineCollection?>(
            nameof(Inlines), control => control.Inlines, (control, value) => control.Inlines = value);

    public static readonly StyledProperty<string> LanguageProperty =
        AvaloniaProperty.Register<CodeTextBlock, string>(nameof(Language), "xaml");

    public static readonly StyledProperty<string?> DisplayTextProperty =
        AvaloniaProperty.Register<CodeTextBlock, string?>(nameof(DisplayText));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    [Content]
    public InlineCollection? Inlines
    {
        get => _inlines;
        set
        {
            SetAndRaise(InlinesProperty, ref _inlines, value);
            RefreshDisplayText();
        }
    }

    public string Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public string? DisplayText
    {
        get => GetValue(DisplayTextProperty);
        private set => SetValue(DisplayTextProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_clipboardButton is not null) _clipboardButton.Click -= OnClipboardButtonClick;

        _clipboardButton = e.NameScope.Find<Button>("PART_ClipBoardButton");
        _clipboardIcon = e.NameScope.Find<PathIcon>("PART_ClipBoardIcon");
        _codeText = e.NameScope.Find<SelectableTextBlock>("PART_CodeText");
        _originalIconData = _clipboardIcon?.Data;

        RefreshDisplayText();

        if (_clipboardButton is not null) _clipboardButton.Click += OnClipboardButtonClick;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty || change.Property == LanguageProperty) RefreshDisplayText();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_clipboardButton is not null) _clipboardButton.Click -= OnClipboardButtonClick;
        _timer.Stop();
    }

    private void RefreshDisplayText()
    {
        DisplayText = Inlines is { Count: > 0 } ? Inlines.Text : Text;
        ApplySyntaxHighlighting();
    }

    private void ApplySyntaxHighlighting()
    {
        if (_codeText is null) return;

        _codeText.Text = null;
        _codeText.Inlines ??= new InlineCollection();
        _codeText.Inlines.Clear();

        foreach (var token in BrowserSyntaxHighlighter.Tokenize(DisplayText, Language))
        {
            _codeText.Inlines.Add(new Run(token.Text)
            {
                Foreground = GetBrush(token.Kind)
            });
        }
    }

    private static IBrush GetBrush(BrowserSyntaxKind kind)
    {
        return kind switch
        {
            BrowserSyntaxKind.Keyword => KeywordBrush,
            BrowserSyntaxKind.Type => TypeBrush,
            BrowserSyntaxKind.Attribute => AttributeBrush,
            BrowserSyntaxKind.String => StringBrush,
            BrowserSyntaxKind.Comment => CommentBrush,
            BrowserSyntaxKind.Number => NumberBrush,
            BrowserSyntaxKind.Punctuation => PunctuationBrush,
            _ => PlainBrush
        };
    }

    private async void OnClipboardButtonClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null || string.IsNullOrEmpty(DisplayText)) return;

        try
        {
            await clipboard.SetTextAsync(DisplayText);
            if (_clipboardIcon is null) return;

            _clipboardIcon.Data = Icons.Check;
            _timer.Stop();
            _timer.Start();
        }
        catch
        {
            // Clipboard access can be denied by browser permissions.
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_clipboardIcon is not null) _clipboardIcon.Data = _originalIconData;
        _timer.Stop();
    }

    public void Dispose()
    {
        if (_disposed) return;
        if (_clipboardButton is not null) _clipboardButton.Click -= OnClipboardButtonClick;
        _codeText = null;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _disposed = true;
    }
}
