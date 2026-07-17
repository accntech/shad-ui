using Avalonia;
using Avalonia.Controls.Presenters;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Useful extension methods for <see cref="Avalonia.Controls.Presenters.ItemsPresenter" />.
/// </summary>
public static class ItemsPresenterAssist
{
    /// <summary>
    ///     Gets or sets if scroll is animated.
    /// </summary>
    public static readonly AttachedProperty<bool> AnimatedScrollProperty =
        AvaloniaProperty.RegisterAttached<ItemsPresenter, bool>("AnimatedScroll",
            typeof(ItemsPresenter));

    static ItemsPresenterAssist()
    {
        AnimatedScrollProperty.Changed.AddClassHandler<ItemsPresenter>(HandleAnimatedScrollChanged);
    }

    private static void HandleAnimatedScrollChanged(ItemsPresenter interactElem,
        AvaloniaPropertyChangedEventArgs args)
    {
        ScrollableExt.SetAnimatedScroll(interactElem, GetAnimatedScroll(interactElem));
    }

    /// <summary>
    ///     Gets the value of <see cref="AnimatedScrollProperty" />
    /// </summary>
    /// <param name="wrap"></param>
    /// <returns></returns>
    public static bool GetAnimatedScroll(ItemsPresenter wrap) => wrap.GetValue(AnimatedScrollProperty);

    /// <summary>
    ///     Sets the value of <see cref="AnimatedScrollProperty" />
    /// </summary>
    /// <param name="wrap"></param>
    /// <param name="value"></param>
    public static void SetAnimatedScroll(ItemsPresenter wrap, bool value)
    {
        wrap.SetValue(AnimatedScrollProperty, value);
    }
}
