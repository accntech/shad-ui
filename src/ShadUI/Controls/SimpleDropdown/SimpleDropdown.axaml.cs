using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     A simple dropdown control that displays a list of items in a popup when triggered.
/// </summary>
[PseudoClasses(":open", ":pressed")]
[TemplatePart("PART_BorderContainer", typeof(Border))]
[TemplatePart("PART_ItemsPresenter", typeof(ItemsPresenter))]
[TemplatePart("PART_Border", typeof(Border))]
public class SimpleDropdown : ItemsControl
{
    /// <summary>
    ///     Defines the <see cref="TriggerContent" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> TriggerContentProperty =
        AvaloniaProperty.Register<SimpleDropdown, object?>(
            nameof(TriggerContent));

    /// <summary>
    ///     Gets or sets the content to display in the trigger button.
    /// </summary>
    public object? TriggerContent
    {
        get => GetValue(TriggerContentProperty);
        set => SetValue(TriggerContentProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="MenuLabel" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> MenuLabelProperty =
        AvaloniaProperty.Register<SimpleDropdown, object?>(
            nameof(MenuLabel));

    /// <summary>
    ///     Gets or sets the label displayed at the top of the dropdown menu.
    /// </summary>
    public object? MenuLabel
    {
        get => GetValue(MenuLabelProperty);
        set => SetValue(MenuLabelProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="IsDropDownOpen" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<SimpleDropdown, bool>(
            nameof(IsDropDownOpen));

    /// <summary>
    ///     Gets or sets a value that indicates whether the dropdown is currently open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="PopupPlacement" /> property.
    /// </summary>
    public static readonly StyledProperty<PlacementMode> PopupPlacementProperty =
        AvaloniaProperty.Register<SimpleDropdown, PlacementMode>(
            nameof(PopupPlacement));

    /// <summary>
    ///     Gets or sets the placement mode of the popup relative to the trigger.
    /// </summary>
    public PlacementMode PopupPlacement
    {
        get => GetValue(PopupPlacementProperty);
        set => SetValue(PopupPlacementProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="PopupVerticalOffset" /> property.
    /// </summary>
    public static readonly StyledProperty<double> PopupVerticalOffsetProperty =
        AvaloniaProperty.Register<SimpleDropdown, double>(
            nameof(PopupVerticalOffset));

    /// <summary>
    ///     Gets or sets the vertical offset of the popup relative to its default position.
    /// </summary>
    public double PopupVerticalOffset
    {
        get => GetValue(PopupVerticalOffsetProperty);
        set => SetValue(PopupVerticalOffsetProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="PopupHorizontalOffset" /> property.
    /// </summary>
    public static readonly StyledProperty<double> PopupHorizontalOffsetProperty =
        AvaloniaProperty.Register<SimpleDropdown, double>(
            nameof(PopupHorizontalOffset));

    /// <summary>
    ///     Gets or sets the horizontal offset of the popup relative to its default position.
    /// </summary>
    public double PopupHorizontalOffset
    {
        get => GetValue(PopupHorizontalOffsetProperty);
        set => SetValue(PopupHorizontalOffsetProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="PopupWidth" /> property.
    /// </summary>
    public static readonly StyledProperty<double> PopupWidthProperty =
        AvaloniaProperty.Register<SimpleDropdown, double>(
            nameof(PopupWidth), double.NaN);

    /// <summary>
    ///     Gets or sets the width of the popup.
    /// </summary>
    public double PopupWidth
    {
        get => GetValue(PopupWidthProperty);
        set => SetValue(PopupWidthProperty, value);
    }

    /// <summary>
    ///     Called when the template is applied to the control.
    /// </summary>
    /// <param name="e">Contains event data.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DetachEventHandlers(); // to avoid duplicate event handlers
        AttachEventHandlers(e.NameScope);
    }

    private void AttachEventHandlers(INameScope nameScope)
    {
        if (nameScope.Find<Border>("PART_BorderContainer") is { } rootBorder)
        {
            rootBorder.PointerPressed += OnRootBorderPressed;
            rootBorder.PointerReleased += OnRootBorderReleased;
            rootBorder.PointerCaptureLost += OnRootBorderCaptureLost;
        }

        if (nameScope.Find<ItemsPresenter>("PART_ItemsPresenter") is { } itemsPresenter)
        {
            itemsPresenter.AttachedToVisualTree += OnItemsPresenterAttached;
        }
    }

    private void DetachEventHandlers()
    {
        if (this.GetTemplateDescendants().FirstOrDefault(x => x.Name == "PART_BorderContainer") is Border rootBorder)
        {
            rootBorder.PointerPressed -= OnRootBorderPressed;
            rootBorder.PointerReleased -= OnRootBorderReleased;
            rootBorder.PointerCaptureLost -= OnRootBorderCaptureLost;
        }

        if (this.GetTemplateDescendants().FirstOrDefault(x => x.Name == "PART_ItemsPresenter") is ItemsPresenter
            itemsPresenter)
        {
            itemsPresenter.AttachedToVisualTree -= OnItemsPresenterAttached;
        }

        // Remove click handlers from all items
        foreach (var item in Items)
        {
            if (item is Control control)
            {
                control.RemoveHandler(Button.ClickEvent, OnMenuItemClicked);
            }
        }
    }

    private void OnItemsPresenterAttached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        UpdateItemsClickHandler();
    }

    /// <summary>
    ///     Called when a property value changes.
    /// </summary>
    /// <param name="change">Contains event data.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty)
        {
            UpdateItemsClickHandler();
        }
        else if (change.Property == IsDropDownOpenProperty)
        {
            var isOpen = (bool)change.NewValue!;
            PseudoClasses.Set(":open", isOpen);
        }
    }

    private void UpdateItemsClickHandler()
    {
        foreach (var item in Items)
        {
            if (item is not Control control) continue;

            control.RemoveHandler(Button.ClickEvent, OnMenuItemClicked);
            control.AddHandler(Button.ClickEvent, OnMenuItemClicked);
        }
    }

    private void OnMenuItemClicked(object? sender, RoutedEventArgs e)
    {
        IsDropDownOpen = false;
    }

    private void OnRootBorderPressed(object? sender, PointerPressedEventArgs e)
    {
        PseudoClasses.Set(":pressed", true);
        IsDropDownOpen = !IsDropDownOpen;
        e.Handled = true;
    }

    private void OnRootBorderReleased(object? sender, PointerReleasedEventArgs e)
    {
        PseudoClasses.Set(":pressed", false);
        e.Handled = true;
    }

    private void OnRootBorderCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        PseudoClasses.Set(":pressed", false);
    }

    private void OnPopupBorderPressed(object? sender, PointerPressedEventArgs e)
    {
        // Stop the event from propagating to prevent the popup from closing
        e.Handled = true;
    }

    /// <summary>
    ///     Called when the control is unloaded from the visual tree.
    /// </summary>
    /// <param name="e">Contains event data.</param>
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        DetachEventHandlers();
    }

    /// <summary>
    ///     Called when the control is loaded into the visual tree.
    /// </summary>
    /// <param name="e">Contains event data.</param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        // Use GetTemplateChildren for OnLoaded since we don't have access to a proper INameScope
        if (this.GetTemplateDescendants().FirstOrDefault(x => x.Name == "PART_BorderContainer") is Border rootBorder)
        {
            rootBorder.PointerPressed += OnRootBorderPressed;
            rootBorder.PointerReleased += OnRootBorderReleased;
            rootBorder.PointerCaptureLost += OnRootBorderCaptureLost;
        }

        if (this.GetTemplateDescendants().FirstOrDefault(x => x.Name == "PART_ItemsPresenter") is ItemsPresenter
            itemsPresenter)
        {
            itemsPresenter.AttachedToVisualTree += OnItemsPresenterAttached;
        }

        if (this.GetTemplateDescendants().FirstOrDefault(x => x.Name == "PART_Border") is Border popupBorder)
        {
            popupBorder.PointerPressed += OnPopupBorderPressed;
        }
    }
}