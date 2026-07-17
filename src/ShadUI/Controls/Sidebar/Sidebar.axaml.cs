using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents a sidebar control that can be expanded or collapsed.
/// </summary>
public class Sidebar : ContentControl
{
    private CancellationTokenSource? _animationCancellation;
    private double? _expandedWidth;
    private bool _isAttached;
    private bool _settingWidth;
    private bool _templateApplied;

    /// <summary>
    ///     Defines the <see cref="Expanded" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ExpandedProperty = AvaloniaProperty.Register<Sidebar, bool>(
        nameof(Expanded), true);

    /// <summary>
    ///     Gets or sets a value indicating whether the sidebar is expanded.
    /// </summary>
    public bool Expanded
    {
        get => GetValue(ExpandedProperty);
        set => SetValue(ExpandedProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Header" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<Sidebar, object?>(
        nameof(Header));

    /// <summary>
    ///     Gets or sets the header content of the sidebar.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Footer" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> FooterProperty = AvaloniaProperty.Register<Sidebar, object?>(
        nameof(Footer));

    /// <summary>
    ///     Gets or sets the footer content of the sidebar.
    /// </summary>
    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="ItemIconContentSpacing" /> property.
    /// </summary>
    public static readonly StyledProperty<double> ItemIconContentSpacingProperty =
        AvaloniaProperty.Register<Sidebar, double>(
            nameof(ItemIconContentSpacing));

    /// <summary>
    ///     Gets or sets the spacing between icon and content in sidebar items.
    /// </summary>
    public double ItemIconContentSpacing
    {
        get => GetValue(ItemIconContentSpacingProperty);
        set => SetValue(ItemIconContentSpacingProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="DefaultItemsSharedSizeGroup" /> property.
    /// </summary>
    public static readonly StyledProperty<string> DefaultItemsSharedSizeGroupProperty =
        AvaloniaProperty.Register<Sidebar, string>(
            nameof(DefaultItemsSharedSizeGroup));

    /// <summary>
    ///     Gets the default item SharedSizeGroup name for the sidebar.
    /// </summary>
    public string DefaultItemsSharedSizeGroup
    {
        get => GetValue(DefaultItemsSharedSizeGroupProperty);
        private set => SetValue(DefaultItemsSharedSizeGroupProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="DefaultItemsGroup" /> property.
    /// </summary>
    public static readonly StyledProperty<string> DefaultItemsGroupProperty =
        AvaloniaProperty.Register<Sidebar, string>(
            nameof(DefaultItemsGroup));

    /// <summary>
    ///     Gets the default item group name for the sidebar.
    /// </summary>
    public string DefaultItemsGroup
    {
        get => GetValue(DefaultItemsGroupProperty);
        private set => SetValue(DefaultItemsGroupProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="ExpandEasing" /> property.
    /// </summary>
    public static readonly StyledProperty<Easing> ExpandEasingProperty = AvaloniaProperty.Register<Sidebar, Easing>(
        nameof(ExpandEasing), new EaseInOut());

    /// <summary>
    ///     Gets or sets the easing function used for the expand animation of the sidebar.
    /// </summary>
    public Easing ExpandEasing
    {
        get => GetValue(ExpandEasingProperty);
        set => SetValue(ExpandEasingProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="ExpandAnimationDuration" /> property.
    /// </summary>
    public static readonly StyledProperty<double> ExpandAnimationDurationProperty =
        AvaloniaProperty.Register<Sidebar, double>(
            nameof(ExpandAnimationDuration), 300);

    /// <summary>
    ///     Gets or sets the duration of the expand animation in milliseconds.
    /// </summary>
    public double ExpandAnimationDuration
    {
        get => GetValue(ExpandAnimationDurationProperty);
        set => SetValue(ExpandAnimationDurationProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="CollapseEasing" /> property.
    /// </summary>
    public static readonly StyledProperty<Easing> CollapseEasingProperty = AvaloniaProperty.Register<Sidebar, Easing>(
        nameof(CollapseEasing), new EaseOut());

    /// <summary>
    ///     Gets or sets the easing function used for the collapse animation of the sidebar.
    /// </summary>
    public Easing CollapseEasing
    {
        get => GetValue(CollapseEasingProperty);
        set => SetValue(CollapseEasingProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="CollapseAnimationDuration" /> property.
    /// </summary>
    public static readonly StyledProperty<double> CollapseAnimationDurationProperty =
        AvaloniaProperty.Register<Sidebar, double>(
            nameof(CollapseAnimationDuration), 200);

    /// <summary>
    ///     Gets or sets the duration of the collapse animation in milliseconds.
    /// </summary>
    public double CollapseAnimationDuration
    {
        get => GetValue(CollapseAnimationDurationProperty);
        set => SetValue(CollapseAnimationDurationProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="CurrentRoute" /> property.
    /// </summary>
    public static readonly StyledProperty<string> CurrentRouteProperty = AvaloniaProperty.Register<Sidebar, string>(
        nameof(CurrentRoute));

    /// <summary>
    ///     Gets or sets the current route or navigation path for the sidebar.
    ///     This property enables navigation-aware sidebar behavior by allowing the control
    ///     to track the current application route. It's particularly useful for:
    ///     <list type="bullet">
    ///         <item><description>Highlighting the currently active navigation item</description></item>
    ///         <item><description>Implementing route-based sidebar state management</description></item>
    ///         <item><description>Enabling automatic sidebar item selection based on the current page</description></item>
    ///         <item><description>Supporting deep linking scenarios where the sidebar should reflect the current route</description></item>
    ///     </list>
    /// </summary>
    public string CurrentRoute
    {
        get => GetValue(CurrentRouteProperty);
        set => SetValue(CurrentRouteProperty, value);
    }
    
    /// <summary>
    ///     Called when the template is applied to the control.
    /// </summary>
    /// <param name="e">The template applied event arguments.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DefaultItemsSharedSizeGroup = $"Shared{Guid.NewGuid():N}";
        DefaultItemsGroup = $"Group{Guid.NewGuid():N}";
        _templateApplied = true;

        if (!_expandedWidth.HasValue) RememberExpandedWidth(Width);
        ApplyWidthImmediately(Expanded);
    }

    /// <summary>
    ///     Called when the control is attached to the visual tree.
    /// </summary>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;

        if (_templateApplied) ApplyWidthImmediately(Expanded);
    }

    /// <summary>
    ///     Called when the control is detached from the visual tree.
    /// </summary>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        CancelCurrentAnimation();
        _isAttached = false;
        base.OnDetachedFromVisualTree(e);
    }

    /// <summary>
    ///     Called when a property value changes.
    /// </summary>
    /// <param name="change">The property change event arguments containing information about the changed property.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == WidthProperty && !_settingWidth && _animationCancellation is null)
        {
            var width = change.GetNewValue<double>();
            RememberExpandedWidth(width);

            if (_templateApplied && !Expanded && IsExpandedWidth(width))
            {
                SetSidebarWidth(MinWidth);
            }
        }

        if (change.Property == ExpandedProperty)
        {
            AnimateOnExpand(change.GetNewValue<bool>());
        }
    }

    /// <summary>
    ///     Animates the sidebar expansion or collapse with the specified easing and duration.
    /// </summary>
    /// <param name="toExpand">A value indicating whether to expand or collapse the sidebar.</param>
    private void AnimateOnExpand(bool toExpand)
    {
        var currentWidth = GetRenderedWidth();
        var hadActiveAnimation = _animationCancellation is not null;
        CancelCurrentAnimation();

        if (!toExpand && !hadActiveAnimation) RememberExpandedWidth(Width);

        var animationCancellation = new CancellationTokenSource();
        _animationCancellation = animationCancellation;

        var targetWidth = toExpand ? GetExpandedWidth() : MinWidth;
        var duration = TimeSpan.FromMilliseconds(
            toExpand ? ExpandAnimationDuration : CollapseAnimationDuration);

        if (!_templateApplied || !_isAttached || duration <= TimeSpan.Zero ||
            !double.IsFinite(currentWidth) || !double.IsFinite(targetWidth) ||
            Math.Abs(currentWidth - targetWidth) < 0.5)
        {
            ApplyWidthImmediately(toExpand);
            CompleteAnimation(animationCancellation);
            return;
        }

        var easing = toExpand ? ExpandEasing : CollapseEasing;
        _ = FluentAnimatorExtensions.ObserveAnimationAsync(
            RunAnimationAsync(toExpand, currentWidth, targetWidth, duration, easing, animationCancellation));
    }

    private async Task RunAnimationAsync(bool toExpand, double currentWidth, double targetWidth, TimeSpan duration,
        Easing easing, CancellationTokenSource animationCancellation)
    {
        try
        {
            var cancellation = animationCancellation.Token;
            var widthAnimation = this.Animate(WidthProperty)
                .From(currentWidth)
                .To(targetWidth)
                .WithEasing(easing)
                .WithDuration(duration)
                .WithCancellationToken(cancellation)
                .RunAsync();

            // Keep the target as the base value so removing the animation does not
            // reveal the previous collapsed/expanded width for a frame.
            SetSidebarWidth(targetWidth);

            if (MinWidth == 0)
            {
                var targetOpacity = toExpand ? 1.0 : 0.0;
                var opacityAnimation = this.Animate(OpacityProperty)
                    .From(Opacity)
                    .To(targetOpacity)
                    .WithEasing(easing)
                    .WithDuration(duration)
                    .WithCancellationToken(cancellation)
                    .RunAsync();

                Opacity = targetOpacity;
                await Task.WhenAll(widthAnimation, opacityAnimation);
            }
            else
            {
                await widthAnimation;
            }

            cancellation.ThrowIfCancellationRequested();
        }
        finally
        {
            CompleteAnimation(animationCancellation);
        }
    }

    private void ApplyWidthImmediately(bool expanded)
    {
        SetSidebarWidth(expanded ? GetExpandedWidth() : MinWidth);
        Opacity = expanded || MinWidth > 0 ? 1 : 0;
    }

    private double GetExpandedWidth() => _expandedWidth ?? Width;

    private double GetRenderedWidth()
    {
        if (Bounds.Width > 0) return Bounds.Width;
        if (double.IsFinite(Width)) return Width;
        return DesiredSize.Width > 0 ? DesiredSize.Width : MinWidth;
    }

    private bool IsExpandedWidth(double width) => double.IsNaN(width) || width > MinWidth;

    private void RememberExpandedWidth(double width)
    {
        if (IsExpandedWidth(width)) _expandedWidth = width;
    }

    private void SetSidebarWidth(double width)
    {
        _settingWidth = true;
        try
        {
            Width = width;
        }
        finally
        {
            _settingWidth = false;
        }
    }

    private void CancelCurrentAnimation()
    {
        var animation = _animationCancellation;
        _animationCancellation = null;
        animation?.Cancel();
        animation?.Dispose();
    }

    private void CompleteAnimation(CancellationTokenSource animation)
    {
        if (!ReferenceEquals(_animationCancellation, animation)) return;

        _animationCancellation = null;
        animation.Dispose();
    }
}
