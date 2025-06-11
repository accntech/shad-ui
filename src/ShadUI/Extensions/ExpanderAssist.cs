using Avalonia;
using Avalonia.Controls;

namespace ShadUI.Extensions;

/// <summary>
///     Useful extensions for the <see cref="Avalonia.Controls.Expander" /> class.
/// </summary>
public static class ExpanderAssist
{
    /// <summary>
    ///     Change header padding
    /// </summary>
    public static readonly AttachedProperty<Thickness> HeaderPaddingProperty =
        AvaloniaProperty.RegisterAttached<Expander, Thickness>("HeaderPadding", typeof(Expander));
    
    /// <summary>
    ///     Get the value of the <see cref="HeaderPaddingProperty" />.
    /// </summary>
    /// <param name="expander"></param>
    /// <returns></returns>
    public static Thickness GetHeaderPadding(Expander expander) => expander.GetValue(HeaderPaddingProperty);
    
    /// <summary>
    ///         Set the value of the <see cref="HeaderPaddingProperty" />.
    /// </summary>
    /// <param name="expander"></param>
    /// <param name="value"></param>
    public static void SetHeaderPadding(Expander expander, Thickness value)
    {
        expander.SetValue(HeaderPaddingProperty, value);
    }
}