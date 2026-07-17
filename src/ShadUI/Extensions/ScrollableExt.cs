using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Usable extension methods for making an element scrollable.
/// </summary>
internal static class ScrollableExt
{
    private static readonly ConditionalWeakTable<Visual, Attachment> Attachments = new();

    /// <summary>
    ///     Makes the visual scrollable.
    /// </summary>
    /// <param name="compositionVisual"></param>
    public static void MakeScrollable(this CompositionVisual? compositionVisual)
    {
        if (compositionVisual == null) return;

        var compositor = compositionVisual.Compositor;

        var animationGroup = compositor.CreateAnimationGroup();
        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = "Offset";

        offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        offsetAnimation.Duration = TimeSpan.FromMilliseconds(250);

        var implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
        animationGroup.Add(offsetAnimation);
        implicitAnimationCollection["Offset"] = animationGroup;
        compositionVisual.ImplicitAnimations = implicitAnimationCollection;
    }

    public static void SetAnimatedScroll(Visual element, bool enabled)
    {
        if (Attachments.TryGetValue(element, out var existing))
        {
            element.AttachedToVisualTree -= existing.Handler;
            Attachments.Remove(element);
        }

        var visual = ElementComposition.GetElementVisual(element);
        if (!enabled)
        {
            if (visual is not null) visual.ImplicitAnimations = null;
            return;
        }

        EventHandler<VisualTreeAttachmentEventArgs> handler = (_, _) =>
            ElementComposition.GetElementVisual(element).MakeScrollable();

        element.AttachedToVisualTree += handler;
        Attachments.Add(element, new Attachment(handler));
        visual.MakeScrollable();
    }

    private sealed record Attachment(EventHandler<VisualTreeAttachmentEventArgs> Handler);
}
