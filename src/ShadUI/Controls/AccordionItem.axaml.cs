using Avalonia;
using Avalonia.Controls;

namespace ShadUI.Controls;

/// <summary>
/// An interactive header that reveals a section of content.
/// </summary>
public class AccordionItem : ListBoxItem
{
    /// <summary>
    /// Defines the <see cref="Header"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<AccordionItem, object?>(nameof(Header));

    /// <summary>
    ///     Gets or sets the header of the accordion item.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}