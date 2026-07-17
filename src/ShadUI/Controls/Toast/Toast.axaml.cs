using System;
using System.Numerics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;

// ReSharper disable once CheckNamespace
namespace ShadUI;

[TemplatePart("PART_ToastCard", typeof(Border))]
[TemplatePart("PART_ActionButton", typeof(Button))]
[TemplatePart("PART_CloseButton", typeof(Button))]
internal class Toast : ContentControl, IDisposable
{
    /// <summary>
    ///     Delay in seconds before the toast is dismissed.
    /// </summary>
    public double Delay { get; set; }

    public ToastPosition? Position { get; set; }

    private DispatcherTimer? _timer;
    private double _timeLapsed;
    private readonly ToastManager? _manager;
    private bool _eventsAttached;
    private bool _disposed;
    private Border? _toastCard;
    private Button? _actionButton;
    private Button? _closeButton;

    public Toast()
    {
    }

    public Toast(ToastManager manager) : this()
    {
        _manager = manager;
        AttachPointerEvents();
    }

    private void AttachPointerEvents()
    {
        if (_eventsAttached) return;

        PointerEntered += OnPointerEntered;
        PointerExited += OnPointerExited;
        _eventsAttached = true;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_disposed) return;

        AttachPointerEvents();
        AttachTemplateEvents();
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        _timer?.Stop();
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        _timer?.Start();
    }

    private void StartCounter()
    {
        if (Delay <= 0) return;

        if (_timer != null) return;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnTimeLapse;

        _timer.Start();
    }

    private void OnTimeLapse(object? sender, EventArgs e)
    {
        _timeLapsed += 1;
        if (_timeLapsed < Delay) return;
        _timer?.Stop();
        _manager?.Dismiss(this);
    }

    public static readonly StyledProperty<Notification> NotificationProperty =
        AvaloniaProperty.Register<Toast, Notification>(nameof(Notification));

    public Notification Notification
    {
        get => GetValue(NotificationProperty);
        set => SetValue(NotificationProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<Toast, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<bool> IsEmptyContentProperty =
        AvaloniaProperty.Register<Toast, bool>(nameof(IsEmptyContent), true);

    public bool IsEmptyContent
    {
        get => Content == null;
        private set => SetValue(IsEmptyContentProperty, value);
    }

    public static readonly StyledProperty<Action?> ActionProperty =
        AvaloniaProperty.Register<Toast, Action?>(nameof(Action));

    public Action? Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }

    public static readonly StyledProperty<string> ActionLabelProperty =
        AvaloniaProperty.Register<Toast, string>(nameof(ActionLabel));

    public string ActionLabel
    {
        get => GetValue(ActionLabelProperty);
        set => SetValue(ActionLabelProperty, value);
    }

    public static readonly StyledProperty<bool> CanDismissByClickingProperty =
        AvaloniaProperty.Register<Toast, bool>(nameof(CanDismissByClicking));

    public bool CanDismissByClicking
    {
        get => GetValue(CanDismissByClickingProperty);
        set => SetValue(CanDismissByClickingProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_toastCard != null)
            _toastCard.PointerPressed -= ToastCardClickedHandler;
        if (_actionButton != null)
            _actionButton.Click -= OnActionButtonClick;
        if (_closeButton != null)
            _closeButton.Click -= OnCloseButtonClick;

        _toastCard = e.NameScope.Get<Border>("PART_ToastCard");
        _actionButton = e.NameScope.Get<Button>("PART_ActionButton");
        _closeButton = e.NameScope.Get<Button>("PART_CloseButton");

        AttachTemplateEvents();
    }

    private void AttachTemplateEvents()
    {
        if (_toastCard is null || _actionButton is null || _closeButton is null) return;

        _toastCard.PointerPressed -= ToastCardClickedHandler;
        _actionButton.Click -= OnActionButtonClick;
        _closeButton.Click -= OnCloseButtonClick;
        _toastCard.PointerPressed += ToastCardClickedHandler;
        _actionButton.Click += OnActionButtonClick;
        _closeButton.Click += OnCloseButtonClick;
    }

    private void OnActionButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Action?.Invoke();
        Task.Delay(500).ContinueWith(_ => _manager?.Dismiss(this),
            TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _manager?.Dismiss(this);
    }

    private void ToastCardClickedHandler(object? sender, PointerPressedEventArgs e)
    {
        if (!CanDismissByClicking) return;
        _manager?.Dismiss(this);
    }

    public void AnimateShow()
    {
        var duration = TimeSpan.FromMilliseconds(360);
        AnimateVisual(0, 1, 12, 0, duration);

        StartCounter();
    }

    public void AnimateDismiss()
    {
        var duration = TimeSpan.FromMilliseconds(220);
        AnimateVisual(1, 0, 0, -24, duration);
    }

    private void AnimateVisual(float fromOpacity, float toOpacity, double fromY, double toY, TimeSpan duration,
        bool retryIfUnavailable = true)
    {
        var visual = ElementComposition.GetElementVisual(this);
        if (visual is null)
        {
            if (retryIfUnavailable)
            {
                Dispatcher.UIThread.Post(
                    () => AnimateVisual(fromOpacity, toOpacity, fromY, toY, duration, false),
                    DispatcherPriority.Render);
            }

            return;
        }

        var compositor = visual.Compositor;
        var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.Duration = duration;
        opacityAnimation.InsertKeyFrame(0, fromOpacity);
        opacityAnimation.InsertKeyFrame(1, toOpacity);

        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Duration = duration;
        offsetAnimation.InsertKeyFrame(0, new Vector3(0, (float)fromY, 0));
        offsetAnimation.InsertKeyFrame(1, new Vector3(0, (float)toY, 0));

        visual.StartAnimation("Opacity", opacityAnimation);
        visual.StartAnimation("Offset", offsetAnimation);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty) IsEmptyContent = Content == null;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        var isNoLongerQueued = _manager?.IsDismissed(this) ?? true;
        Cleanup(releaseTemplateParts: false, stopTimer: isNoLongerQueued);
    }

    private void Cleanup(bool releaseTemplateParts, bool stopTimer)
    {
        if (stopTimer && _timer != null)
        {
            _timer.Stop();
            _timer.Tick -= OnTimeLapse;
            _timer = null;
        }

        if (_eventsAttached)
        {
            PointerEntered -= OnPointerEntered;
            PointerExited -= OnPointerExited;
            _eventsAttached = false;
        }

        if (_toastCard != null)
        {
            _toastCard.PointerPressed -= ToastCardClickedHandler;
            if (releaseTemplateParts) _toastCard = null;
        }

        if (_actionButton != null)
        {
            _actionButton.Click -= OnActionButtonClick;
            if (releaseTemplateParts) _actionButton = null;
        }

        if (_closeButton != null)
        {
            _closeButton.Click -= OnCloseButtonClick;
            if (releaseTemplateParts) _closeButton = null;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        Cleanup(releaseTemplateParts: true, stopTimer: true);

        _disposed = true;
    }
}
