using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;

// ReSharper disable once CheckNamespace
namespace ShadUI;

[TemplatePart("PART_ToastCard", typeof(Border))]
[TemplatePart("PART_ActionButton", typeof(Button))]
[TemplatePart("PART_CloseButton", typeof(Button))]
internal class Toast : ContentControl
{
    /// <summary>
    ///     Delay in seconds before the toast is dismissed.
    /// </summary>
    public double Delay { get; set; }

    public ToastPosition? Position { get; set; }

    public Toast()
    {
        PointerEntered += (_, _) => _timer?.Stop();
        PointerExited += (_, _) => _timer?.Start();
    }

    private DispatcherTimer? _timer;
    private double _timeLapsed;

    private void StartCounter()
    {
        if (Delay <= 0) return;

        if (_timer != null) return;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnTimeLapse;

        _timer.Start();
    }

    private void OnTimeLapse(object sender, EventArgs e)
    {
        _timeLapsed += 1;
        if (_timeLapsed < Delay) return;
        _timer?.Stop();
        _manager?.Dismiss(this);
    }

    private readonly ToastManager? _manager;

    public Toast(ToastManager manager)
    {
        _manager = manager;
        PointerEntered += (_, _) => _timer?.Stop();
        PointerExited += (_, _) => _timer?.Start();
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

    public static readonly StyledProperty<bool> ShowProgressBarProperty =
    AvaloniaProperty.Register<Toast, bool>(nameof(ShowProgressBar));

    public bool ShowProgressBar
    {
        get => GetValue(ShowProgressBarProperty);
        set => SetValue(ShowProgressBarProperty, value);
    }

    public static readonly StyledProperty<bool> IsProgressIndeterminateProperty =
        AvaloniaProperty.Register<Toast, bool>(nameof(IsProgressIndeterminate), true);

    public bool IsProgressIndeterminate
    {
        get => GetValue(IsProgressIndeterminateProperty);
        set => SetValue(IsProgressIndeterminateProperty, value);
    }

    public static readonly StyledProperty<double> ProgressValueProperty =
        AvaloniaProperty.Register<Toast, double>(nameof(ProgressValue), 0);

    public double ProgressValue
    {
        get => GetValue(ProgressValueProperty);
        set => SetValue(ProgressValueProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        e.NameScope.Get<Border>("PART_ToastCard").PointerPressed += ToastCardClickedHandler;
        e.NameScope.Get<Button>("PART_ActionButton").Click += (_, _) =>
        {
            Action?.Invoke();
            Task.Delay(500).ContinueWith(_ => _manager?.Dismiss(this),
                TaskScheduler.FromCurrentSynchronizationContext());
        };
        e.NameScope.Get<Button>("PART_CloseButton").Click += (_, _) => _manager?.Dismiss(this);
    }

    private void ToastCardClickedHandler(object sender, PointerPressedEventArgs e)
    {
        if (!CanDismissByClicking) return;
        _manager?.Dismiss(this);
    }

    public void AnimateShow()
    {
        this.Animate(OpacityProperty)
            .From(0d)
            .To(1d)
            .WithDuration(TimeSpan.FromMilliseconds(500))
            .WithEasing(new CubicEaseInOut())
            .Start();

        this.Animate(MaxHeightProperty)
            .From(0)
            .To(500)
            .WithDuration(TimeSpan.FromMilliseconds(500))
            .WithEasing(new CubicEaseInOut())
            .Start();

        this.Animate(MarginProperty)
            .From(new Thickness(0, 10, 0, -10))
            .To(new Thickness())
            .WithDuration(TimeSpan.FromMilliseconds(500))
            .WithEasing(new CubicEaseInOut())
            .Start();

        StartCounter();
    }

    public void AnimateDismiss()
    {
        this.Animate(OpacityProperty)
            .From(1d)
            .To(0d)
            .WithDuration(TimeSpan.FromMilliseconds(500))
            .WithEasing(new CubicEaseInOut())
            .Start();

        this.Animate(MarginProperty)
            .From(new Thickness())
            .To(new Thickness(0, 0, 0, -100))
            .WithDuration(TimeSpan.FromMilliseconds(500))
            .WithEasing(new CubicEaseInOut())
            .Start();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty) IsEmptyContent = Content == null;
    }
}