using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides fluent extension methods for animating Avalonia controls.
///     <para>
///         Code extracted from SukiUI (https://github.com/kikipoulet/SukiUI).
///     </para>
/// </summary>
public static class FluentAnimatorExtensions
{
    internal static async Task ObserveAnimationAsync(Task animation)
    {
        try
        {
            await animation.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Cancellation is the expected way to replace an in-flight animation.
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Animation failed: {exception}");
        }
    }

    /// <summary>
    ///     Creates a fluent animator for the specified property on the given control.
    /// </summary>
    /// <typeparam name="T">The type of the property being animated.</typeparam>
    /// <param name="control">The control to animate.</param>
    /// <param name="property">The property to animate.</param>
    /// <returns>A fluent animator instance for chaining animation configuration.</returns>
    /// <example>
    ///     <code>
    ///         _myButton.Animate(OpacityProperty)
    ///                  .From(0).To(1)
    ///                  .WithDuration(TimeSpan.FromMilliseconds(1000))
    ///                  .WithEasing(new CubicEaseOut())
    ///                  .Start();
    ///     </code>
    /// </example>
    public static FluentAnimator<T> Animate<T>(this Animatable control, AvaloniaProperty<T> property)
    {
        return new FluentAnimator<T>(control, property);
    }
}

/// <summary>
///     A fluent animator that provides a chainable API for configuring and running animations on Avalonia controls.
///     This struct allows for easy creation of animations with custom duration, easing, and cancellation support.
///     <para>
///         Code extracted from SukiUI (https://github.com/kikipoulet/SukiUI).
///     </para>
/// </summary>
/// <typeparam name="T">The type of the property being animated.</typeparam>
/// <param name="control">The control to animate.</param>
/// <param name="property">The property to animate.</param>
public ref struct FluentAnimator<T>(Animatable control, AvaloniaProperty<T> property)
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(.5);
    private static readonly Easing DefaultEasing = new CubicEaseInOut();

    private T? _from;
    private T? _to;
    private TimeSpan? _duration;
    private Easing? _easing;

    private CancellationToken _cancellation;

    /// <summary>
    ///     Sets the starting value for the animation.
    /// </summary>
    /// <param name="value">The starting value.</param>
    /// <returns>The fluent animator instance for method chaining.</returns>
    public FluentAnimator<T> From(T value)
    {
        return this with { _from = value };
    }

    /// <summary>
    ///     Sets the ending value for the animation.
    /// </summary>
    /// <param name="value">The ending value.</param>
    /// <returns>The fluent animator instance for method chaining.</returns>
    public FluentAnimator<T> To(T value)
    {
        return this with { _to = value };
    }

    /// <summary>
    ///     Sets the duration of the animation.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <returns>The fluent animator instance for method chaining.</returns>
    public FluentAnimator<T> WithDuration(TimeSpan duration)
    {
        return this with { _duration = duration };
    }

    /// <summary>
    ///     Sets the easing function for the animation.
    /// </summary>
    /// <param name="easing">The easing function to use.</param>
    /// <returns>The fluent animator instance for method chaining.</returns>
    public FluentAnimator<T> WithEasing(Easing easing)
    {
        return this with { _easing = easing };
    }

    /// <summary>
    ///     Sets the cancellation token for the animation.
    /// </summary>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>The fluent animator instance for method chaining.</returns>
    public FluentAnimator<T> WithCancellationToken(CancellationToken cancellation)
    {
        return this with { _cancellation = cancellation };
    }

    /// <summary>
    ///     Starts the animation asynchronously without waiting for completion.
    /// </summary>
    public readonly void Start()
    {
        _ = FluentAnimatorExtensions.ObserveAnimationAsync(RunAsync());
    }

    /// <summary>
    ///     Runs the animation asynchronously and returns a task that completes when the animation finishes.
    /// </summary>
    /// <returns>A task that represents the asynchronous animation operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the 'To' value is not set before starting the animation.</exception>
    public readonly Task RunAsync()
    {
        if (_to == null)
        {
            throw new InvalidOperationException("The 'To' value must be set before starting the animation.");
        }

        var duration = _duration ?? DefaultDuration;
        var easing = _easing ?? DefaultEasing;

        var animation = new Animation
        {
            Duration = duration,
            Easing = easing,
            FillMode = FillMode.Forward
        };

        if (_from != null)
        {
            var fromKeyFrame = new KeyFrame
            {
                Cue = new Cue(0),
                Setters =
                {
                    new Setter { Property = property, Value = _from }
                }
            };

            animation.Children.Add(fromKeyFrame);
        }

        var toKeyFrame = new KeyFrame
        {
            Cue = new Cue(1),
            Setters =
            {
                new Setter { Property = property, Value = _to }
            }
        };

        animation.Children.Add(toKeyFrame);

        return animation.RunAsync(control, _cancellation);
    }
}
