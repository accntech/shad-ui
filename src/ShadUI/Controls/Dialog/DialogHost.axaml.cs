using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.VisualTree;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Dialog host control that manages the display and lifecycle of dialogs within a window.
/// </summary>
[TemplatePart("PART_DialogBackground", typeof(Border))]
[TemplatePart("PART_TitleBar", typeof(Border))]
[TemplatePart("PART_CloseButton", typeof(Button))]
public class DialogHost : TemplatedControl, IDisposable
{
    private bool _disposed;
    private Avalonia.Controls.Window? _ancestorWindow;
    private Border? _overlayBackground;
    private Border? _observedContainer;
    private Border? _titleBar;
    private Button? _closeButton;
    private DialogManager? _subscribedManager;
    private bool _isAttached;

    /// <summary>
    ///     Defines the <see cref="Manager" /> property.
    /// </summary>
    public static readonly StyledProperty<DialogManager?> ManagerProperty =
        AvaloniaProperty.Register<DialogHost, DialogManager?>(nameof(Manager));

    /// <summary>
    ///     Gets or sets the dialog manager responsible for handling dialog operations.
    /// </summary>
    public DialogManager? Manager
    {
        get => GetValue(ManagerProperty);
        set => SetValue(ManagerProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="OverlayBrush" /> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> OverlayBrushProperty =
        AvaloniaProperty.Register<DialogHost, IBrush?>(nameof(OverlayBrush));

    /// <summary>
    ///     Gets or sets the brush used to dim the host when a dialog is open.
    ///     Defaults to the <c>DialogOverlayColor</c> theme resource.
    /// </summary>
    public IBrush? OverlayBrush
    {
        get => GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="OverlayOpacity" /> property.
    /// </summary>
    public static readonly StyledProperty<double> OverlayOpacityProperty =
        AvaloniaProperty.Register<DialogHost, double>(nameof(OverlayOpacity), 0.60);

    /// <summary>
    ///     Gets or sets the opacity of the dim overlay when a dialog is open. Range 0-1; default 0.60.
    /// </summary>
    public double OverlayOpacity
    {
        get => GetValue(OverlayOpacityProperty);
        set => SetValue(OverlayOpacityProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Dialog" /> property.
    /// </summary>
    internal static readonly StyledProperty<object?> DialogProperty =
        AvaloniaProperty.Register<DialogHost, object?>(nameof(Dialog));

    /// <summary>
    ///     Gets or sets the current dialog content.
    /// </summary>
    internal object? Dialog
    {
        get => GetValue(DialogProperty);
        set => SetValue(DialogProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="IsDialogOpen" /> property.
    /// </summary>
    internal static readonly StyledProperty<bool> IsDialogOpenProperty =
        AvaloniaProperty.Register<DialogHost, bool>(nameof(IsDialogOpen));

    /// <summary>
    ///     Gets or sets whether a dialog is currently open.
    /// </summary>
    internal bool IsDialogOpen
    {
        get => GetValue(IsDialogOpenProperty);
        set => SetValue(IsDialogOpenProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="DialogMaxWidth" /> property.
    /// </summary>
    internal static readonly StyledProperty<double> DialogMaxWidthProperty =
        AvaloniaProperty.Register<DialogHost, double>(nameof(DialogMaxWidth), double.MaxValue);

    /// <summary>
    ///     Gets or sets the maximum width of the dialog.
    /// </summary>
    internal double DialogMaxWidth
    {
        get => GetValue(DialogMaxWidthProperty);
        set => SetValue(DialogMaxWidthProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="DialogMinWidth" /> property.
    /// </summary>
    internal static readonly StyledProperty<double> DialogMinWidthProperty =
        AvaloniaProperty.Register<DialogHost, double>(nameof(DialogMinWidth), double.MinValue);

    /// <summary>
    ///     Gets or sets the minimum width of the dialog.
    /// </summary>
    internal double DialogMinWidth
    {
        get => GetValue(DialogMinWidthProperty);
        set => SetValue(DialogMinWidthProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Dismissible" /> property.
    /// </summary>
    internal static readonly StyledProperty<bool> DismissibleProperty =
        AvaloniaProperty.Register<DialogHost, bool>(nameof(Dismissible), true);

    /// <summary>
    ///     Gets or sets whether the dialog can be dismissed.
    /// </summary>
    internal bool Dismissible
    {
        get => GetValue(DismissibleProperty);
        set => SetValue(DismissibleProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="HasOpenDialog" /> property.
    /// </summary>
    internal static readonly StyledProperty<bool> HasOpenDialogProperty =
        AvaloniaProperty.Register<DialogHost, bool>(nameof(HasOpenDialog));

    /// <summary>
    ///     Gets or sets whether the dialog can be dismissed.
    /// </summary>
    internal bool HasOpenDialog
    {
        get => GetValue(HasOpenDialogProperty);
        set => SetValue(HasOpenDialogProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="CanDismissWithBackgroundClick" /> property.
    /// </summary>
    internal static readonly StyledProperty<bool> CanDismissWithBackgroundClickProperty =
        AvaloniaProperty.Register<DialogHost, bool>(nameof(CanDismissWithBackgroundClick), true);

    /// <summary>
    ///     Gets or sets whether the dialog can be dismissed by clicking the background.
    /// </summary>
    internal bool CanDismissWithBackgroundClick
    {
        get => GetValue(CanDismissWithBackgroundClickProperty);
        set => SetValue(CanDismissWithBackgroundClickProperty, value);
    }

    /// <summary>
    ///     Caches the nearest ancestor Window so drag and maximize work without an explicit Owner.
    /// </summary>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_disposed) return;

        _isAttached = true;
        _ancestorWindow = this.FindAncestorOfType<Avalonia.Controls.Window>();
        AttachContainerObserver();
        if (Manager is { } manager) AttachManagerEvents(manager);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DetachManagerEvents();
        _isAttached = false;
        _ancestorWindow = null;
        DetachContainerObserver();
        Dialog = null;
        IsDialogOpen = false;
        HasOpenDialog = false;
        base.OnDetachedFromVisualTree(e);
    }

    private Avalonia.Controls.Window? ResolveOwnerWindow()
        => _ancestorWindow ??= this.FindAncestorOfType<Avalonia.Controls.Window>();

    private void AttachContainerObserver()
    {
        DetachContainerObserver();
        _observedContainer = FindRoundedAncestorBorder();
        if (_observedContainer is not null)
        {
            _observedContainer.PropertyChanged += OnContainerPropertyChanged;
        }
        ApplyContainerCornerRadius();
    }

    private Border? FindRoundedAncestorBorder()
    {
        var current = this.GetVisualParent();
        while (current is not null)
        {
            if (current is Border border && border.ClipToBounds)
            {
                return border;
            }
            current = current.GetVisualParent();
        }
        return null;
    }

    private void DetachContainerObserver()
    {
        if (_observedContainer is null) return;
        _observedContainer.PropertyChanged -= OnContainerPropertyChanged;
        _observedContainer = null;
    }

    private void OnContainerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Border.CornerRadiusProperty)
        {
            ApplyContainerCornerRadius();
        }
    }

    private void ApplyContainerCornerRadius()
    {
        _overlayBackground?.CornerRadius = _observedContainer?.CornerRadius ?? default;
    }

    /// <summary>
    ///     Called when the control template is applied to set up event handlers and animations.
    /// </summary>
    /// <param name="e">The template applied event arguments.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DetachTemplateHandlers();

        if (e.NameScope.Find<Border>("PART_DialogBackground") is { } background)
        {
            _overlayBackground = background;
            background.PointerPressed += OnDialogBackgroundPointerPressed;
            ApplyContainerCornerRadius();
        }

        if (e.NameScope.Find<Border>("PART_TitleBar") is { } titleBar)
        {
            _titleBar = titleBar;
            titleBar.PointerPressed += OnTitleBarPointerPressed;
            titleBar.DoubleTapped += OnMaximizeButtonClicked;
        }

        if (e.NameScope.Find<Button>("PART_CloseButton") is { } closeButton)
        {
            _closeButton = closeButton;
            closeButton.Click += OnCloseButtonClick;
        }
    }

    private void OnDialogBackgroundPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (CanDismissWithBackgroundClick) CloseDialog();
    }

    private void OnCloseButtonClick(object? sender, RoutedEventArgs e) => CloseDialog();

    private void DetachTemplateHandlers()
    {
        if (_overlayBackground is not null)
        {
            _overlayBackground.PointerPressed -= OnDialogBackgroundPointerPressed;
            _overlayBackground = null;
        }

        if (_titleBar is not null)
        {
            _titleBar.PointerPressed -= OnTitleBarPointerPressed;
            _titleBar.DoubleTapped -= OnMaximizeButtonClicked;
            _titleBar = null;
        }

        if (_closeButton is not null)
        {
            _closeButton.Click -= OnCloseButtonClick;
            _closeButton = null;
        }
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        ResolveOwnerWindow()?.BeginMoveDrag(e);
    }

    private void OnMaximizeButtonClicked(object? sender, RoutedEventArgs args)
    {
        var window = ResolveOwnerWindow();
        if (window is null || !window.CanMaximize) return;

        window.WindowState = window.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void CloseDialog()
    {
        if (!Dismissible) return;

        IsDialogOpen = false;

        Manager?.RemoveLast();
        Manager?.OpenLast();
    }

    static DialogHost()
    {
        ManagerProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<DialogManager?>>(x =>
                OnManagerPropertyChanged(x.Sender, x)));
    }

    private static void OnManagerPropertyChanged(AvaloniaObject sender,
        AvaloniaPropertyChangedEventArgs propChanged)
    {
        if (sender is not DialogHost host)
        {
            throw new NullReferenceException("Dependency object is not of valid type " + nameof(DialogHost));
        }

        if (propChanged.OldValue is DialogManager oldManager)
        {
            host.DetachManagerEvents(oldManager);
        }

        if (host._isAttached && !host._disposed && propChanged.NewValue is DialogManager newManager)
        {
            host.AttachManagerEvents(newManager);
        }
    }

    private void AttachManagerEvents(DialogManager manager)
    {
        if (ReferenceEquals(_subscribedManager, manager)) return;

        DetachManagerEvents();
        manager.OnDialogShown += ManagerOnDialogShown;
        manager.OnDialogClosed += ManagerOnDialogClosed;
        manager.AllowDismissChanged += AllowDismissChanged;
        _subscribedManager = manager;

        if (manager.Dialogs.Count > 0)
        {
            var lastDialog = manager.Dialogs.Last();
            ManagerOnDialogShown(manager,
                new DialogShownEventArgs { Control = lastDialog.Key, Options = lastDialog.Value });
        }
    }

    private void DetachManagerEvents(DialogManager manager)
    {
        if (!ReferenceEquals(_subscribedManager, manager)) return;

        manager.OnDialogShown -= ManagerOnDialogShown;
        manager.OnDialogClosed -= ManagerOnDialogClosed;
        manager.AllowDismissChanged -= AllowDismissChanged;
        _subscribedManager = null;
    }

    private void DetachManagerEvents()
    {
        if (_subscribedManager is { } manager) DetachManagerEvents(manager);
    }

    private void ManagerOnDialogShown(object? sender, DialogShownEventArgs e)
    {
        if (Manager is null) return;

        Dialog = e.Control;
        Dismissible = e.Options.Dismissible;

        if (e.Options.MaxWidth > 0) DialogMaxWidth = e.Options.MaxWidth;
        if (e.Options.MinWidth > 0) DialogMinWidth = e.Options.MinWidth;

        IsDialogOpen = true;
        HasOpenDialog = true;
    }

    private async void ManagerOnDialogClosed(object? sender, DialogClosedEventArgs e)
    {
        try
        {
            if (Manager is null) return;
            if (e.Control != Dialog) return;

            IsDialogOpen = false;
            if (e.ReplaceExisting) return;

            HasOpenDialog = Manager.Dialogs.Count > 0;

            await Task.Delay(200); // Allow animations to complete
            if (!HasOpenDialog) Dialog = null;
        }
        catch (Exception)
        {
            //ignore
        }
    }

    private void AllowDismissChanged(object? sender, bool e)
    {
        if (Manager is null || Manager.Dialogs.Count == 0) return;

        var firstDialog = Manager.Dialogs.First();
        Dismissible = firstDialog.Value.Dismissible || e;
    }

    /// <summary>
    ///     Disposes the dialog host and cleans up event subscriptions to prevent memory leaks.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        DetachManagerEvents();
        DetachContainerObserver();
        DetachTemplateHandlers();
        Dialog = null;

        _disposed = true;
    }
}
