using Avalonia;
using ShadUI.Controls;

namespace ShadUI.Extensions;

/// <summary>
///     Useful extensions for the <see cref="ShadUI.Controls.AccordionItem" /> class.
/// </summary>
public static class AccordionAssist
{
    /// <summary>
    ///     Change header padding
    /// </summary>
    public static readonly AttachedProperty<Thickness> HeaderPaddingProperty =
        AvaloniaProperty.RegisterAttached<AccordionItem, Thickness>("HeaderPadding", typeof(AccordionItem));

    /// <summary>
    ///     Get the value of the <see cref="HeaderPaddingProperty" />.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static Thickness GetHeaderPadding(AccordionItem control)
    {
        return control.GetValue(HeaderPaddingProperty);
    }
    
    /// <summary>
    ///         Set the value of the <see cref="HeaderPaddingProperty" />.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="value"></param>
    public static void SetHeaderPadding(AccordionItem control, Thickness value)
    {
        control.SetValue(HeaderPaddingProperty, value);
    }
}