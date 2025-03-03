﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace ShadUI.Extensions;

/// <summary>
///     Provides attached properties for Avalonia.Controls.MenuItem.
/// </summary>
public static class MenuItem
{
    /// <summary>
    ///     Add menu item label.
    /// </summary>
    public static readonly AttachedProperty<object> LabelProperty =
        AvaloniaProperty.RegisterAttached<Avalonia.Controls.MenuItem, object>("Label", typeof(Avalonia.Controls.MenuItem));

    /// <summary>
    ///     Get the value of the <see cref="LabelProperty" />.
    /// </summary>
    /// <param name="ctrl">The menu item.</param>
    /// <returns>The label value.</returns>
    public static object GetLabel(Avalonia.Controls.MenuItem ctrl) => ctrl.GetValue(LabelProperty);

    /// <summary>
    ///     Set the value of the <see cref="LabelProperty" />.
    /// </summary>
    /// <param name="ctrl">The menu item.</param>
    /// <param name="value">The label value to set.</param>
    public static void SetLabel(Avalonia.Controls.MenuItem ctrl, object value)
    {
        ctrl.SetValue(LabelProperty, value);
    }
    
    /// <summary>
    ///     Defines an attached property for setting the vertical offset of a popup.
    /// </summary>
    public static readonly AttachedProperty<double> PopupVerticalOffsetProperty =
        AvaloniaProperty.RegisterAttached<Avalonia.Controls.MenuItem, double>("PopupVerticalOffset", typeof(TemplatedControl));

    /// <summary>
    ///     Gets the value of the <see cref="PopupVerticalOffsetProperty" />.
    /// </summary>
    /// <param name="ctrl">The menu item.</param>
    /// <returns>The vertical offset value.</returns>
    public static double GetPopupVerticalOffset(Avalonia.Controls.MenuItem ctrl) => ctrl.GetValue(PopupVerticalOffsetProperty);

    /// <summary>
    ///     Sets the value of the <see cref="PopupVerticalOffsetProperty" />.
    /// </summary>
    /// <param name="ctrl">The menu item.</param>
    /// <param name="value">The vertical offset value to set.</param>
    public static void SetPopupVerticalOffset(Avalonia.Controls.MenuItem ctrl, double value) =>
        ctrl.SetValue(PopupVerticalOffsetProperty, value);

    /// <summary>
    ///     Defines an attached property for setting the placement mode of a popup.
    /// </summary>
    public static readonly AttachedProperty<PlacementMode> PopupPlacementProperty =
        AvaloniaProperty.RegisterAttached<Avalonia.Controls.MenuItem, PlacementMode>("PopupPlacement", typeof(TemplatedControl));

    /// <summary>
    ///     Gets the value of the <see cref="PopupPlacementProperty" />.
    /// </summary>
    /// <param name="ctrl">The menu item.</param>
    /// <returns>The placement mode value.</returns>
    public static PlacementMode GetPopupPlacement(Avalonia.Controls.MenuItem ctrl) => ctrl.GetValue(PopupPlacementProperty);

    /// <summary>
    ///     Sets the value of the <see cref="PopupPlacementProperty" />.
    /// </summary>
    /// <param name="ctrl">The menu item.</param>
    /// <param name="value">The placement mode value to set.</param>
    public static void SetPopupPlacement(Avalonia.Controls.MenuItem ctrl, PlacementMode value) =>
        ctrl.SetValue(PopupPlacementProperty, value);
}