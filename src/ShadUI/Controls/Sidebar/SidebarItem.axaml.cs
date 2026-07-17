using System;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents a selectable item within a sidebar control.
/// </summary>
[TemplatePart("PART_BorderContainer", typeof(Border))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
public class SidebarItem : RadioButton
{
    private static readonly EaseInOut ExpandEasing = new();

    /// <summary>
    ///     Icon property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<SidebarItem, object?>(nameof(Icon));

    /// <summary>
    ///     Gets or sets the icon of the menu item.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Expanded" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ExpandedProperty = AvaloniaProperty.Register<SidebarItem, bool>(
        nameof(Expanded), true);

    /// <summary>
    ///     Gets or sets a value indicating whether the sidebar item is expanded.
    /// </summary>
    public bool Expanded
    {
        get => GetValue(ExpandedProperty);
        set => SetValue(ExpandedProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Spacing" /> property.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty = AvaloniaProperty.Register<SidebarItem, double>(
        nameof(Spacing));

    /// <summary>
    ///     Gets or sets the spacing between elements in the sidebar item.
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="SharedSizeGroup" /> property.
    /// </summary>
    public static readonly StyledProperty<string?> SharedSizeGroupProperty =
        AvaloniaProperty.Register<SidebarItem, string?>(
            nameof(SharedSizeGroup));

    /// <summary>
    ///     Gets or sets the shared size group name for the icon column.
    /// </summary>
    public string? SharedSizeGroup
    {
        get => GetValue(SharedSizeGroupProperty);
        set => SetValue(SharedSizeGroupProperty, value);
    }

    private ColumnDefinition? _iconColumn;
    private ContentPresenter? _contentPresenter;

    /// <summary>
    ///     Defines the <see cref="HasIcon" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> HasIconProperty = AvaloniaProperty.Register<SidebarItem, bool>(
        nameof(HasIcon));

    /// <summary>
    ///     Gets a value indicating whether the sidebar item has an icon.
    /// </summary>
    public bool HasIcon
    {
        get => GetValue(HasIconProperty);
        private set => SetValue(HasIconProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="Route" /> property.
    /// </summary>
    public static readonly StyledProperty<string> RouteProperty = AvaloniaProperty.Register<SidebarItem, string>(
        nameof(Route));

    /// <summary>
    ///     Gets or sets the route or navigation path associated with this sidebar item.
    ///     This property is useful for:
    ///     <list type="bullet">
    ///         <item><description>Navigation-aware sidebar behavior where items can be highlighted based on the current route</description></item>
    ///         <item><description>Implementing automatic selection of sidebar items when navigating to specific pages</description></item>
    ///         <item><description>Enabling deep linking scenarios where the sidebar reflects the current application state</description></item>
    ///         <item><description>Facilitating route-based sidebar item activation and deactivation</description></item>
    ///     </list>
    /// </summary>
    public string Route
    {
        get => GetValue(RouteProperty);
        set => SetValue(RouteProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="ParentRoute" /> property.
    /// </summary>
    public static readonly StyledProperty<string> ParentRouteProperty = AvaloniaProperty.Register<SidebarItem, string>(
        nameof(ParentRoute));

    /// <summary>
    ///     Gets or sets the parent route that this sidebar item belongs to.
    /// </summary>
    public string ParentRoute
    {
        get => GetValue(ParentRouteProperty);
        internal set => SetValue(ParentRouteProperty, value);
    }
    
    /// <summary>
    ///     Called when the template is applied to the control.
    /// </summary>
    /// <param name="e">The template applied event arguments.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (e.NameScope.Find("PART_BorderContainer") is Border { Child: Grid { ColumnDefinitions.Count: > 0 } grid })
        {
            _iconColumn = grid.ColumnDefinitions[0];
            UpdateSharedSizeGroup();
        }

        if (e.NameScope.Find("PART_ContentPresenter") is ContentPresenter contentPresenter)
        {
            _contentPresenter = contentPresenter;

            AnimateExpand(Expanded);
        }
    }

    /// <summary>
    ///     Called when a property value changes.
    /// </summary>
    /// <param name="change">The property change event arguments.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SharedSizeGroupProperty)
        {
            UpdateSharedSizeGroup();
        }

        if (change.Property == ExpandedProperty)
        {
            var toExpand = change.GetNewValue<bool>();
            AnimateExpand(toExpand);
        }

        if (change.Property == IconProperty)
        {
            HasIcon = Icon != null;
        }

        if (change.Property == ParentRouteProperty)
        {
            UpdateCheckState(change.GetNewValue<string>());
        }
    }

    private void UpdateCheckState(string value)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(Route)) return;
        IsChecked = ParentRoute == Route;
    }

    private void AnimateExpand(bool toExpand)
    {
        if (toExpand)
        {
            _contentPresenter?.SetValue(IsVisibleProperty, true);
            _contentPresenter?.Animate(OpacityProperty)
                .From(0)
                .To(1)
                .WithDuration(TimeSpan.FromSeconds(0.1))
                .WithEasing(ExpandEasing)
                .Start();
        }
        else
        {
            _contentPresenter?.SetValue(IsVisibleProperty, false);
        }
    }

    /// <summary>
    ///     Updates the shared size group for the icon column.
    /// </summary>
    private void UpdateSharedSizeGroup()
    {
        if (_iconColumn != null)
        {
            _iconColumn.SharedSizeGroup = SharedSizeGroup;
        }
    }
}
