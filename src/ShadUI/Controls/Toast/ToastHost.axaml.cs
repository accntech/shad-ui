using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Reactive;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     The host for toast notifications.
/// </summary>
public class ToastHost : ItemsControl
{
    /// <summary>
    ///     Defines the toast manager.
    /// </summary>
    public static readonly StyledProperty<ToastManager> ManagerProperty =
        AvaloniaProperty.Register<ToastHost, ToastManager>(nameof(Manager));

    /// <summary>
    ///     Gets or sets the toast manager.
    /// </summary>
    public ToastManager Manager
    {
        get => GetValue(ManagerProperty);
        set => SetValue(ManagerProperty, value);
    }

    /// <summary>
    ///     Maximum number of toasts to be displayed at a time.
    /// </summary>
    public static readonly StyledProperty<byte>
        MaxToastsProperty = AvaloniaProperty.Register<ToastHost, byte>(nameof(MaxToasts), 5);

    /// <summary>
    ///     Gets or sets the maximum number of toasts to be displayed at a time.
    /// </summary>
    public byte MaxToasts
    {
        get => GetValue(MaxToastsProperty);
        set => SetValue(MaxToastsProperty, value);
    }

    /// <summary>
    ///     Defines the position of the toast host relative to its parent.
    /// </summary>
    public static readonly StyledProperty<ToastPosition> PositionProperty =
        AvaloniaProperty.Register<ToastHost, ToastPosition>(nameof(Position));

    /// <summary>
    ///     Gets or sets the position of the toast host.
    /// </summary>
    public ToastPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether only a single toast should be displayed at a time.
    ///     If true, any new toast will clear all existing toasts before being shown.
    /// </summary>
    public static readonly StyledProperty<bool> SingleToastProperty = AvaloniaProperty.Register<ToastHost, bool>(
        nameof(SingleToast));

    /// <summary>
    ///     Gets or sets a value indicating whether only a single toast should be displayed at a time.
    ///     If true, any new toast will clear all existing toasts before being shown.
    /// </summary>
    public bool SingleToast
    {
        get => GetValue(SingleToastProperty);
        set => SetValue(SingleToastProperty, value);
    }

    private ToastPosition _originalPosition;

    /// <summary>
    ///     Called when the template is applied.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        OnPositionChanged(Position);
        _originalPosition = Position; // use this as the default position if none is specified in the toast
    }

    /// <summary>
    ///     Called when a property is changed.
    /// </summary>
    /// <param name="change"></param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PositionProperty && change.NewValue is ToastPosition loc)
        {
            OnPositionChanged(loc);
        }
    }

    private void OnPositionChanged(ToastPosition position)
    {
        HorizontalAlignment = position switch
        {
            ToastPosition.BottomRight => HorizontalAlignment.Right,
            ToastPosition.BottomCenter => HorizontalAlignment.Center,
            ToastPosition.BottomLeft => HorizontalAlignment.Left,
            ToastPosition.TopRight => HorizontalAlignment.Right,
            ToastPosition.TopCenter => HorizontalAlignment.Center,
            ToastPosition.TopLeft => HorizontalAlignment.Left,
            _ => throw new ArgumentOutOfRangeException()
        };
        VerticalAlignment = position switch
        {
            ToastPosition.BottomRight => VerticalAlignment.Bottom,
            ToastPosition.BottomCenter => VerticalAlignment.Bottom,
            ToastPosition.BottomLeft => VerticalAlignment.Bottom,
            ToastPosition.TopRight => VerticalAlignment.Top,
            ToastPosition.TopCenter => VerticalAlignment.Top,
            ToastPosition.TopLeft => VerticalAlignment.Top,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static void OnManagerPropertyChanged(AvaloniaObject sender,
        AvaloniaPropertyChangedEventArgs propChanged)
    {
        if (sender is not ToastHost host)
        {
            throw new NullReferenceException("Dependency object is not of valid type " + nameof(ToastHost));
        }

        if (propChanged.OldValue is ToastManager oldManager)
        {
            host.DetachManagerEvents(oldManager);
        }

        if (propChanged.NewValue is ToastManager newManager)
        {
            host.AttachManagerEvents(newManager);
        }
    }

    private void AttachManagerEvents(ToastManager manager)
    {
        manager.OnToastQueued += ManagerOnToastQueued;
        manager.OnToastDismissed += ManagerOnToastDismissed;
        manager.OnAllToastsDismissed += ManagerOnAllToastsDismissed;
    }

    private void DetachManagerEvents(ToastManager manager)
    {
        manager.OnToastQueued -= ManagerOnToastQueued;
        manager.OnToastDismissed -= ManagerOnToastDismissed;
        manager.OnAllToastsDismissed -= ManagerOnAllToastsDismissed;
    }

    private void ManagerOnToastDismissed(object? sender, Toast toast)
    {
        ClearToast(toast);
    }

    private void ManagerOnAllToastsDismissed(object? sender, EventArgs e)
    {
        foreach (var toast in Items)
        {
            ClearToast((Toast)toast!);
        }
    }

    private void ManagerOnToastQueued(object? sender, Toast toast)
    {
        if (MaxToasts <= 0) return;

        if (SingleToast)
        {
            foreach (var t in Items)
            {
                ClearToast((Toast)t!);
            }
        }

        var position = toast.Position ?? _originalPosition;
        if (Position == position)
        {
            ShowToast(toast);
            return;
        }

        var count = Items.Count;
        foreach (var t in Items) ClearToast((Toast)t!);

        if (count > 0)
        {
            Task.Delay(300).ContinueWith(_ => ShowToast(toast), TaskScheduler.FromCurrentSynchronizationContext());
        }
        else
        {
            ShowToast(toast);
        }
    }

    private void ShowToast(Toast toast)
    {
        Items.Add(toast);
        Manager.EnsureMaximum(MaxToasts);

        Position = toast.Position ?? _originalPosition;
        toast.AnimateShow();
    }

    private void ClearToast(Toast toast)
    {
        if (Manager.IsDismissed(toast)) return;
        toast.AnimateDismiss();
        Task.Delay(300).ContinueWith(_ => { Items.Remove(toast); }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    static ToastHost()
    {
        ManagerProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<ToastManager>>(x =>
                OnManagerPropertyChanged(x.Sender, x)));
    }
}