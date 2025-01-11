using Avalonia;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace ShadUI.Extensions;

/// <summary>
///     Useful extension methods for <see cref="StackPanel" />
/// </summary>
public static class StackPanelExt
{
    /// <summary>
    ///     Gets or sets if scroll is animated.
    /// </summary>
    public static readonly AttachedProperty<bool> AnimatedScrollProperty =
        AvaloniaProperty.RegisterAttached<StackPanel, bool>("AnimatedScroll", typeof(StackPanel));

    static StackPanelExt()
    {
        AnimatedScrollProperty.Changed.AddClassHandler<StackPanel>(HandleAnimatedScrollChanged);
    }

    private static void HandleAnimatedScrollChanged(StackPanel interactElem, AvaloniaPropertyChangedEventArgs args)
    {
        if (GetAnimatedScroll(interactElem))
            interactElem.AttachedToVisualTree += (_, _) =>
                ScrollableExt.MakeScrollable(ElementComposition.GetElementVisual(interactElem));
    }

    /// <summary>
    ///     Gets the value of <see cref="AnimatedScrollProperty" />
    /// </summary>
    /// <param name="wrap"></param>
    /// <returns></returns>
    public static bool GetAnimatedScroll(StackPanel wrap) => wrap.GetValue(AnimatedScrollProperty);

    /// <summary>
    ///     Sets the value of <see cref="AnimatedScrollProperty" />
    /// </summary>
    /// <param name="wrap"></param>
    /// <param name="value"></param>
    public static void SetAnimatedScroll(StackPanel wrap, bool value)
    {
        wrap.SetValue(AnimatedScrollProperty, value);
    }
}