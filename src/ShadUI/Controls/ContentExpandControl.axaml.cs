using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace ShadUI.Controls;

/// <summary>
/// 
/// </summary>
public class ContentExpandControl : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Multiplier"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MultiplierProperty =
        AvaloniaProperty.Register<ContentExpandControl, double>(nameof(Multiplier));

    /// <summary>
    /// Gets or sets the multiplier for changing the width and height.
    /// </summary>
    public double Multiplier
    {
        get => GetValue(MultiplierProperty);
        set => SetValue(MultiplierProperty, value);
    }
        
    /// <summary>
    /// Defines the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<ContentExpandControl, Orientation>(nameof(Orientation));
        
    /// <summary>
    /// Gets or sets the orientation in which child controls will be layed out.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
        
    static ContentExpandControl()
    {
        AffectsArrange<ContentExpandControl>(MultiplierProperty, OrientationProperty);
        AffectsMeasure<ContentExpandControl>(MultiplierProperty, OrientationProperty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureCore(Size availableSize)
    {
        var result = base.MeasureCore(availableSize);
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);

        if (Parent is Control control)
        {
            control.Margin = new Thickness(1);
        }
        return result;
    }

    /// <summary>
    /// Override measurement based on multiplier
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected override Size MeasureOverride(Size availableSize)
    {
        var result = base.MeasureOverride(availableSize);

        double w = result.Width;
        double h = result.Height;

        switch (Orientation)
        {
            case Orientation.Horizontal:
                w *= Multiplier;
                break;

            case Orientation.Vertical:
                h *= Multiplier;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (Parent is Control control)
        {
            control.Margin = new Thickness(0);
        }
        
        return new Size(w, h);
    }
}