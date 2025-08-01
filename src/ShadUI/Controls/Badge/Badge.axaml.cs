using Avalonia;
using Avalonia.Controls;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents a badge control that displays small status indicators, counts, or labels.
/// </summary>
public class Badge : ContentControl
{
    /// <summary>
    ///     Defines the <see cref="Variant" /> property.
    /// </summary>
    public static readonly StyledProperty<BadgeVariant> VariantProperty =
        AvaloniaProperty.Register<Badge, BadgeVariant>(nameof(Variant), BadgeVariant.Default);

    static Badge()
    {
        VariantProperty.Changed.AddClassHandler<Badge>((badge, e) => badge.OnVariantChanged(e));
    }

    /// <summary>
    ///     Gets or sets the visual variant of the badge.
    /// </summary>
    /// <value>
    ///     The badge variant that determines the visual appearance. Default is <see cref="BadgeVariant.Default"/>.
    /// </value>
    public BadgeVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    private void OnVariantChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.OldValue is BadgeVariant oldVariant)
        {
            Classes.Remove(oldVariant.ToString());
        }

        if (e.NewValue is BadgeVariant newVariant && newVariant != BadgeVariant.Default)
        {
            Classes.Add(newVariant.ToString());
        }
    }
}

/// <summary>
///     Defines the visual variants available for the Badge control.
/// </summary>
public enum BadgeVariant
{
    /// <summary>
    ///     Default badge variant using primary theme colors.
    /// </summary>
    Default,

    /// <summary>
    ///     Secondary badge variant using secondary theme colors.
    /// </summary>
    Secondary,

    /// <summary>
    ///     Destructive badge variant using destructive/error theme colors.
    /// </summary>
    Destructive,

    /// <summary>
    ///     Outline badge variant with transparent background and border.
    /// </summary>
    Outline
}