using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides attached properties for adding smooth scroll to ScrollViewers.
///     <para>
///         Usage: Set <see cref="IsEnabledProperty" /> to <c>true</c> on a
///         ScrollViewer to enable smooth scroll.
///     </para>
/// </summary>
public class SmoothScrollAssist
{
	private static readonly bool IsMacOs =  RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
	private static readonly ConditionalWeakTable<ScrollViewer, SmoothScrollController> Controllers = new();
	
	static SmoothScrollAssist()
	{
		IsEnabledProperty.Changed.AddClassHandler<ScrollViewer>(IsEnabledChanged);
		BaseStepSizeProperty.Changed.AddClassHandler<ScrollViewer>(BaseStepSizeChanged);
		SmoothingFactorProperty.Changed.AddClassHandler<ScrollViewer>(SmoothingFactorChanged);
	}
	
	
	/// <summary>
	///     Gets or sets whether smooth scroll should be enabled.
	/// </summary>
	public static readonly AttachedProperty<bool> IsEnabledProperty =
		AvaloniaProperty.RegisterAttached<ScrollViewer, bool>("IsEnabled", typeof(SmoothScrollAssist));

	/// <summary>
	///     Gets the value of <see cref="IsEnabledProperty" />
	/// </summary>
	/// <param name="scrollViewer">The ScrollViewer.</param>
	/// <returns>The smooth scroll value.</returns>
	public static bool GetIsEnabled(ScrollViewer scrollViewer) =>
		scrollViewer.GetValue(IsEnabledProperty);

	/// <summary>
	///     Sets the value of <see cref="IsEnabledProperty" />
	/// </summary>
	/// <param name="scrollViewer">The ScrollViewer.</param>
	/// <param name="value">The smooth scroll value to set.</param>
	public static void SetIsEnabled(ScrollViewer scrollViewer, bool value) =>
		scrollViewer.SetValue(IsEnabledProperty, value);

	private static void IsEnabledChanged(ScrollViewer scrollViewer, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.NewValue is true && !Controllers.TryGetValue(scrollViewer, out _))
		{
			scrollViewer.IsScrollInertiaEnabled = false;

			var baseStepSize = GetBaseStepSize(scrollViewer);
			var smoothingFactor = GetSmoothingFactor(scrollViewer);
			SmoothScrollController controller = new(scrollViewer, baseStepSize, smoothingFactor);
			
			Controllers.Add(scrollViewer, controller);
		}
		else if (Controllers.TryGetValue(scrollViewer, out var controller))
		{
			controller.Stop();
			
			Controllers.Remove(scrollViewer);
			
			scrollViewer.IsScrollInertiaEnabled = true;
		}
	}
	
	
	/// <summary>
	///     Gets or sets the multiplier for each notch of the scroll wheel.
	/// </summary>
	public static readonly AttachedProperty<double> BaseStepSizeProperty =
		AvaloniaProperty.RegisterAttached<ScrollViewer, double>("BaseStepSize", typeof(SmoothScrollAssist), IsMacOs ? 40 : 70);
	
	/// <summary>
	///     Gets the value of <see cref="BaseStepSizeProperty" />
	/// </summary>
	public static double GetBaseStepSize(ScrollViewer scrollViewer) =>
		scrollViewer.GetValue(BaseStepSizeProperty);

	/// <summary>
	///     Sets the value of <see cref="BaseStepSizeProperty" />
	/// </summary>
	public static void SetBaseStepSize(ScrollViewer scrollViewer, double value) =>
		scrollViewer.SetValue(BaseStepSizeProperty, value);

	private static void BaseStepSizeChanged(ScrollViewer scrollViewer, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.NewValue is not double newValue || !Controllers.TryGetValue(scrollViewer, out var controller))
			return;
		
		controller.BaseStepSize = newValue;
	}
	
	
	/// <summary>
	///     Gets or sets the smoothing intensity. Higher values feel "snappier", lower values feel "floatier".
	/// </summary>
	public static readonly AttachedProperty<double> SmoothingFactorProperty =
		AvaloniaProperty.RegisterAttached<ScrollViewer, double>("SmoothingFactor", typeof(SmoothScrollAssist), IsMacOs ? 50 : 20);
	
	/// <summary>
	///     Gets the value of <see cref="SmoothingFactorProperty" />
	/// </summary>
	public static double GetSmoothingFactor(ScrollViewer scrollViewer) =>
		scrollViewer.GetValue(SmoothingFactorProperty);

	/// <summary>
	///     Sets the value of <see cref="SmoothingFactorProperty" />
	/// </summary>
	public static void SetSmoothingFactor(ScrollViewer scrollViewer, double value) =>
		scrollViewer.SetValue(SmoothingFactorProperty, value);

	private static void SmoothingFactorChanged(ScrollViewer scrollViewer, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.NewValue is not double newValue || !Controllers.TryGetValue(scrollViewer, out var controller))
			return;
		
		controller.SmoothingFactor = newValue;
	}
}