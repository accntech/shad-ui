using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

// ReSharper disable once CheckNamespace
namespace ShadUI;

[TemplatePart("PART_PrimaryButton", typeof(Button))]
[TemplatePart("PART_SecondaryButton", typeof(Button))]
[TemplatePart("PART_TertiaryButton", typeof(Button))]
[TemplatePart("PART_CancelButton", typeof(Button))]
internal class SimpleDialog : TemplatedControl, IDisposable
{
    private readonly DialogManager? _manager;
    private Button? _primaryButton;
    private Button? _secondaryButton;
    private Button? _tertiaryButton;
    private Button? _cancelButton;
    private bool _disposed;

    public SimpleDialog()
    {
    }

    public string Id { get; set; } = string.Empty;

    public SimpleDialog(DialogManager manager)
    {
        _manager = manager;
    }

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SimpleDialog, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<SimpleDialog, string>(nameof(Message));

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly StyledProperty<string> PrimaryButtonTextProperty =
        AvaloniaProperty.Register<SimpleDialog, string>(nameof(PrimaryButtonText));

    public string PrimaryButtonText
    {
        get => GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    public static readonly StyledProperty<DialogButtonStyle> PrimaryButtonStyleProperty =
        AvaloniaProperty.Register<SimpleDialog, DialogButtonStyle>(nameof(PrimaryButtonStyle));

    public DialogButtonStyle PrimaryButtonStyle
    {
        get => GetValue(PrimaryButtonStyleProperty);
        set => SetValue(PrimaryButtonStyleProperty, value);
    }

    public Action? PrimaryCallback { get; set; }

    public Func<Task>? PrimaryCallbackAsync { get; set; }

    public static readonly StyledProperty<string> SecondaryButtonTextProperty =
        AvaloniaProperty.Register<SimpleDialog, string>(nameof(SecondaryButtonText));

    public string SecondaryButtonText
    {
        get => GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    public static readonly StyledProperty<DialogButtonStyle> SecondaryButtonStyleProperty =
        AvaloniaProperty.Register<SimpleDialog, DialogButtonStyle>(nameof(SecondaryButtonStyle),
            DialogButtonStyle.Secondary);

    public DialogButtonStyle SecondaryButtonStyle
    {
        get => GetValue(SecondaryButtonStyleProperty);
        set => SetValue(SecondaryButtonStyleProperty, value);
    }

    public Action? SecondaryCallback { get; set; }

    public Func<Task>? SecondaryCallbackAsync { get; set; }

    public static readonly StyledProperty<string> TertiaryButtonTextProperty =
        AvaloniaProperty.Register<SimpleDialog, string>(nameof(TertiaryButtonText));

    public string TertiaryButtonText
    {
        get => GetValue(TertiaryButtonTextProperty);
        set => SetValue(TertiaryButtonTextProperty, value);
    }

    public static readonly StyledProperty<DialogButtonStyle> TertiaryButtonStyleProperty =
        AvaloniaProperty.Register<SimpleDialog, DialogButtonStyle>(nameof(TertiaryButtonStyle),
            DialogButtonStyle.Outline);

    public DialogButtonStyle TertiaryButtonStyle
    {
        get => GetValue(TertiaryButtonStyleProperty);
        set => SetValue(TertiaryButtonStyleProperty, value);
    }

    public Action? TertiaryCallback { get; set; }

    public Func<Task>? TertiaryCallbackAsync { get; set; }

    public static readonly StyledProperty<string> CancelButtonTextProperty =
        AvaloniaProperty.Register<SimpleDialog, string>(nameof(CancelButtonText));

    public string CancelButtonText
    {
        get => GetValue(CancelButtonTextProperty);
        set => SetValue(CancelButtonTextProperty, value);
    }

    public Action? CancelCallback { get; set; }

    public Func<Task>? CancelCallbackAsync { get; set; }

    public static readonly StyledProperty<DialogButtonStyle> CancelButtonStyleProperty =
        AvaloniaProperty.Register<SimpleDialog, DialogButtonStyle>(nameof(CancelButtonStyle),
            DialogButtonStyle.Outline);

    public DialogButtonStyle CancelButtonStyle
    {
        get => GetValue(CancelButtonStyleProperty);
        set => SetValue(CancelButtonStyleProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachButtonHandlers();

        _primaryButton = e.NameScope.Get<Button>("PART_PrimaryButton");
        _secondaryButton = e.NameScope.Get<Button>("PART_SecondaryButton");
        _tertiaryButton = e.NameScope.Get<Button>("PART_TertiaryButton");
        _cancelButton = e.NameScope.Get<Button>("PART_CancelButton");

        if (_primaryButton is not null)
            _primaryButton.Click += OnPrimaryButtonClick;
        if (_secondaryButton is not null)
            _secondaryButton.Click += OnSecondaryButtonClick;
        if (_tertiaryButton is not null)
            _tertiaryButton.Click += OnTertiaryButtonClick;
        if (_cancelButton is not null)
            _cancelButton.Click += OnCancelButtonClick;
    }

    private async void OnPrimaryButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            PrimaryCallback?.Invoke();
            if (PrimaryCallbackAsync is not null)
                await PrimaryCallbackAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            _manager?.CloseDialog(this);
            _manager?.OpenLast();
        }
    }

    private async void OnSecondaryButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            SecondaryCallback?.Invoke();
            if (SecondaryCallbackAsync is not null)
                await SecondaryCallbackAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            _manager?.CloseDialog(this);
            _manager?.OpenLast();
        }
    }

    private async void OnTertiaryButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            TertiaryCallback?.Invoke();
            if (TertiaryCallbackAsync is not null)
                await TertiaryCallbackAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            _manager?.CloseDialog(this);
            _manager?.OpenLast();
        }
    }

    private async void OnCancelButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            CancelCallback?.Invoke();
            if (CancelCallbackAsync is not null)
                await CancelCallbackAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            _manager?.CloseDialog(this);
            _manager?.OpenLast();
        }
    }

    private void DetachButtonHandlers()
    {
        if (_primaryButton is not null)
            _primaryButton.Click -= OnPrimaryButtonClick;
        if (_secondaryButton is not null)
            _secondaryButton.Click -= OnSecondaryButtonClick;
        if (_tertiaryButton is not null)
            _tertiaryButton.Click -= OnTertiaryButtonClick;
        if (_cancelButton is not null)
            _cancelButton.Click -= OnCancelButtonClick;
    }

    /// <summary>
    ///     Sets the ID of the dialog based on its properties and options.
    /// </summary>
    /// <param name="options"></param>
    internal void SetId(DialogOptions options)
    {
        // Use a pooled array writer which is more memory efficient than StringBuilder
        using var hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        if (!string.IsNullOrEmpty(Title))
        {
            hasher.AppendData(Encoding.UTF8.GetBytes(Title));
        }

        hasher.AppendData(new[] { (byte)'|' });

        if (!string.IsNullOrEmpty(Message))
        {
            hasher.AppendData(Encoding.UTF8.GetBytes(Message));
        }

        hasher.AppendData(new[] { (byte)'|' });

        if (!string.IsNullOrEmpty(PrimaryButtonText))
        {
            hasher.AppendData(Encoding.UTF8.GetBytes(PrimaryButtonText));
        }

        hasher.AppendData(new[] { (byte)'|' });
        hasher.AppendData(BitConverter.GetBytes((int)PrimaryButtonStyle));
        hasher.AppendData(new[] { (byte)'|' });

        if (!string.IsNullOrEmpty(SecondaryButtonText))
        {
            hasher.AppendData(Encoding.UTF8.GetBytes(SecondaryButtonText));
        }

        hasher.AppendData(new[] { (byte)'|' });
        hasher.AppendData(BitConverter.GetBytes((int)SecondaryButtonStyle));
        hasher.AppendData(new[] { (byte)'|' });

        if (!string.IsNullOrEmpty(TertiaryButtonText))
        {
            hasher.AppendData(Encoding.UTF8.GetBytes(TertiaryButtonText));
        }

        hasher.AppendData(new[] { (byte)'|' });
        hasher.AppendData(BitConverter.GetBytes((int)TertiaryButtonStyle));
        hasher.AppendData(new[] { (byte)'|' });

        if (!string.IsNullOrEmpty(CancelButtonText))
        {
            hasher.AppendData(Encoding.UTF8.GetBytes(CancelButtonText));
        }

        hasher.AppendData(new[] { (byte)'|' });
        hasher.AppendData(BitConverter.GetBytes((int)CancelButtonStyle));
        hasher.AppendData(new[] { (byte)'|' });

        hasher.AppendData(BitConverter.GetBytes(options.Dismissible));
        hasher.AppendData(new[] { (byte)'|' });
        hasher.AppendData(BitConverter.GetBytes(options.MinWidth));
        hasher.AppendData(new[] { (byte)'|' });
        hasher.AppendData(BitConverter.GetBytes(options.MaxWidth));

        var hash = hasher.GetHashAndReset();
        var hexChars = new char[16];
        for (var i = 0; i < 8; i++)
        {
            hexChars[i * 2] = GetHexChar(hash[i] >> 4);
            hexChars[i * 2 + 1] = GetHexChar(hash[i] & 0xF);
        }

        Id = new string(hexChars);
    }

    private static char GetHexChar(int value)
    {
        return (char)(value < 10 ? '0' + value : 'A' + (value - 10));
    }

    /// <summary>
    ///     Disposes the dialog and cleans up event subscriptions and callbacks.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        // Detach button event handlers
        DetachButtonHandlers();

        // Clear callback references to allow garbage collection
        PrimaryCallback = null;
        PrimaryCallbackAsync = null;
        SecondaryCallback = null;
        SecondaryCallbackAsync = null;
        TertiaryCallback = null;
        TertiaryCallbackAsync = null;
        CancelCallback = null;
        CancelCallbackAsync = null;

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Finalizer to ensure cleanup if Dispose is not called.
    /// </summary>
    ~SimpleDialog()
    {
        Dispose();
    }
}