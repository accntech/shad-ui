using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Reactive;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Dialog host control that manages the display and lifecycle of dialogs within a window.
/// </summary>
[TemplatePart("PART_DialogBackground", typeof(Border))]
[TemplatePart("PART_TitleBar", typeof(Border))]
[TemplatePart("PART_CloseButton", typeof(Button))]
public class DialogHost : TemplatedControl
{
    /// <summary>
    ///     Defines the <see cref="Owner" /> property.
    /// </summary>
    public static readonly StyledProperty<Window?> OwnerProperty =
        AvaloniaProperty.Register<DialogHost, Window?>(nameof(Owner));

    /// <summary>
    ///     Gets or sets the owner window of the dialog host.
    /// </summary>
    public Window? Owner
    {
        get => GetValue(OwnerProperty);
        set => SetValue(OwnerProperty, value);
    }

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
    ///     Called when the control template is applied to set up event handlers and animations.
    /// </summary>
    /// <param name="e">The template applied event arguments.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (e.NameScope.Find<Border>("PART_DialogBackground") is { } background)
        {
            background.PointerPressed += (_, _) =>
            {
                if (CanDismissWithBackgroundClick) CloseDialog();
            };
        }

        if (e.NameScope.Find<Border>("PART_TitleBar") is { } titleBar)
        {
            titleBar.PointerPressed += OnTitleBarPointerPressed;
            titleBar.DoubleTapped += OnMaximizeButtonClicked;
        }

        if (e.NameScope.Find<Button>("PART_CloseButton") is { } closeButton)
        {
            closeButton.Click += (_, _) => CloseDialog();
        }
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
            {
                MainWindow: not null
            } desktop)
        {
            desktop.MainWindow.BeginMoveDrag(e);
        }
    }

    private void OnMaximizeButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (Owner is not null && Owner.CanMaximize)
        {
            Owner.WindowState = Owner.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }

    private void CloseDialog()
    {
        if (!Dismissible) return;

        IsDialogOpen = false;

        Manager?.RemoveLast();
        Manager?.OpenLast();

        if (Owner is not null) Owner.HasOpenDialog = false;
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

        if (propChanged.NewValue is DialogManager newManager)
        {
            host.AttachManagerEvents(newManager);
        }
    }

    private void AttachManagerEvents(DialogManager manager)
    {
        manager.OnDialogShown += ManagerOnDialogShown;
        manager.OnDialogClosed += ManagerOnDialogClosed;
        manager.AllowDismissChanged += AllowDismissChanged;
    }

    private void DetachManagerEvents(DialogManager manager)
    {
        manager.OnDialogShown -= ManagerOnDialogShown;
        manager.OnDialogClosed -= ManagerOnDialogClosed;
        manager.AllowDismissChanged -= AllowDismissChanged;
    }

    private void ManagerOnDialogShown(object sender, DialogShownEventArgs e)
    {
        if (Manager is null) return;

        Dialog = e.Control;
        Dismissible = e.Options.Dismissible;

        if (e.Options.MaxWidth > 0) DialogMaxWidth = e.Options.MaxWidth;
        if (e.Options.MinWidth > 0) DialogMinWidth = e.Options.MinWidth;

        IsDialogOpen = true;
        HasOpenDialog = true;
        if (Owner is { })
            Owner.HasOpenDialog = true;
    }

    private async void ManagerOnDialogClosed(object sender, DialogClosedEventArgs e)
    {
        try
        {
            if (Manager is null) return;
            if (e.Control != Dialog) return;

            IsDialogOpen = false;
            if (e.ReplaceExisting) return;

            HasOpenDialog = Manager.Dialogs.Count > 0;
            if (Owner is { })
                Owner.HasOpenDialog = Manager.Dialogs.Count > 0;

            await Task.Delay(200); // Allow animations to complete
            if (!HasOpenDialog) Dialog = null;
        }
        catch (Exception)
        {
            //ignore
        }
    }

    private void AllowDismissChanged(object sender, bool e)
    {
        if (Manager is null || Manager.Dialogs.Count == 0) return;

        var firstDialog = Manager.Dialogs.First();
        Dismissible = firstDialog.Value.Dismissible || e;
    }
}