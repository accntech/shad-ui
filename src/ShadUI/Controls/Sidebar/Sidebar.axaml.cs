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
        _cacheWidth = Width;
    }

    /// <summary>
    ///     Stores the width of the sidebar before collapsing for animation purposes.
    /// </summary>
    private double _cacheWidth;

    /// <summary>
    ///     Called when a property value changes.
    /// </summary>
    /// <param name="change">The property change event arguments containing information about the changed property.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ExpandedProperty)
        {
            var toExpand = change.GetNewValue<bool>();

            AnimateOnExpand(toExpand);
        }
    }

    /// <summary>
    ///     Animates the sidebar expansion or collapse with the specified easing and duration.
    /// </summary>
    /// <param name="toExpand">A value indicating whether to expand or collapse the sidebar.</param>
    private async void AnimateOnExpand(bool toExpand)
    {
        _animationCancellation?.Cancel();
        _animationCancellation?.Dispose();
        var animationCancellation = new CancellationTokenSource();
        _animationCancellation = animationCancellation;
        var cancellation = animationCancellation.Token;

        if (!toExpand && Width > MinWidth) _cacheWidth = Width;

        try
        {
            if (toExpand)
            {
                Width = _cacheWidth;
                this.Animate(OpacityProperty)
                    .From(0.0)
                    .To(1.0)
                    .WithEasing(ExpandEasing)
                    .WithDuration(TimeSpan.FromMilliseconds(ExpandAnimationDuration))
                    .WithCancellationToken(cancellation)
                    .Start();

                return;
            }

            if (MinWidth == 0)
            {
                this.Animate(OpacityProperty)
                    .From(1.0)
                    .To(0.0)
                    .WithEasing(CollapseEasing)
                    .WithDuration(TimeSpan.FromMilliseconds(CollapseAnimationDuration))
                    .WithCancellationToken(cancellation)
                    .Start();

                await Task.Delay(TimeSpan.FromMilliseconds(CollapseAnimationDuration), cancellation);
            }

            cancellation.ThrowIfCancellationRequested();
            Width = MinWidth;
            Opacity = 1;
        }
        catch (OperationCanceledException)
        {
            // A newer Expanded value replaced this transition.
        }
        finally
        {
            if (ReferenceEquals(_animationCancellation, animationCancellation))
            {
                _animationCancellation = null;
                animationCancellation.Dispose();
            }
        }
    }
}
