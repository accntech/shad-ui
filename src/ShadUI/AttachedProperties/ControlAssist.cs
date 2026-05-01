using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides attached properties and methods for assisting with control behaviors.
/// </summary>
public static class ControlAssist
{
    static ControlAssist()
    {
        LabelProperty.Changed.AddClassHandler<TemplatedControl>((control, args) =>
        {
            control.TemplateApplied += (sender, eventArgs) =>
            {
                var label = eventArgs.NameScope.Find<TextBlock>("PART_Label");
                if (label is null || string.IsNullOrEmpty(args.NewValue?.ToString())) return;

                if (sender is TextBox tb) tb.UseFloatingPlaceholder = true;

                label.Text = args.NewValue!.ToString();
            };
        });
    }

    /// <summary>
    ///     Add a label or override the default control floating watermark.
    /// </summary>
    public static readonly AttachedProperty<string> LabelProperty =
        AvaloniaProperty.RegisterAttached<TemplatedControl, string>("Label", typeof(TemplatedControl));

    /// <summary>
    ///     Get the value of the <see cref="LabelProperty" />.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string GetLabel(TemplatedControl control) => control.GetValue(LabelProperty);

    /// <summary>
    ///     Set the value of the <see cref="LabelProperty" />.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="value"></param>
    public static void SetLabel(TemplatedControl control, string value)
    {
        control.SetValue(LabelProperty, value);
    }

    /// <summary>
    ///     Show a hint text.
    /// </summary>
    public static readonly AttachedProperty<string> HintProperty =
        AvaloniaProperty.RegisterAttached<TemplatedControl, string>("Hint", typeof(TemplatedControl));

    /// <summary>
    ///     Get the value of the <see cref="HintProperty" />.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string GetHint(TemplatedControl control) => control.GetValue(HintProperty);

    /// <summary>
    ///     Set the value of the <see cref="HintProperty" />.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="value"></param>
    public static void SetHint(TemplatedControl control, string value)
    {
        control.SetValue(HintProperty, value);
    }

    /// <summary>
    ///     Indicates whether the control should show progress.
    /// </summary>
    public static readonly AttachedProperty<bool> ShowProgressProperty =
        AvaloniaProperty.RegisterAttached<TemplatedControl, bool>("ShowProgress", typeof(TemplatedControl));

    /// <summary>
    ///     Gets the value of the <see cref="ShowProgressProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>The value of the <see cref="ShowProgressProperty" />.</returns>
    public static bool GetShowProgress(TemplatedControl control) => control.GetValue(ShowProgressProperty);

    /// <summary>
    ///     Sets the value of the <see cref="ShowProgressProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="value">The value to set.</param>
    public static void SetShowProgress(TemplatedControl control, bool value)
    {
        control.SetValue(ShowProgressProperty, value);
    }

    /// <summary>
    ///     Defines an attached property for setting the height excluding label and hint of a control.
    /// </summary>
    public static readonly AttachedProperty<double> HeightProperty =
        AvaloniaProperty.RegisterAttached<TemplatedControl, double>("Height", typeof(TemplatedControl));

    /// <summary>
    ///     Gets the value of the <see cref="HeightProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>The height value.</returns>
    public static double GetHeight(TemplatedControl control) => control.GetValue(HeightProperty);

    /// <summary>
    ///     Sets the value of the <see cref="HeightProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="value">
    ///     The height value to set. Must be used to define the actual height of control excluding label and
    ///     hint.
    /// </param>
    public static void SetHeight(TemplatedControl control, double value) => control.SetValue(HeightProperty, value);

    /// <summary>
    ///     Defines an attached property for setting the minimum height of a control.
    /// </summary>
    public static readonly AttachedProperty<double> MinHeightProperty =
        AvaloniaProperty.RegisterAttached<TemplatedControl, double>("MinHeight", typeof(TemplatedControl));

    /// <summary>
    ///     Gets the value of the <see cref="MinHeightProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>The minimum height value.</returns>
    public static double GetMinHeight(TemplatedControl control) => control.GetValue(MinHeightProperty);

    /// <summary>
    ///     Sets the value of the <see cref="MinHeightProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="value">
    ///     The minimum height value to set. Must be used to define the actual minimum height of control
    ///     excluding label and hint.
    /// </param>
    public static void SetMinHeight(TemplatedControl control, double value) => control.SetValue(MinHeightProperty, value);

    /// <summary>
    ///     Defines an attached property for setting the maximum height of a control.
    /// </summary>
    public static readonly AttachedProperty<double> MaxHeightProperty =
        AvaloniaProperty.RegisterAttached<TemplatedControl, double>("MaxHeight", typeof(TemplatedControl));

    /// <summary>
    ///     Gets the value of the <see cref="MaxHeightProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>The maximum height value.</returns>
    public static double GetMaxHeight(TemplatedControl control) => control.GetValue(MaxHeightProperty);

    /// <summary>
    ///     Sets the value of the <see cref="MaxHeightProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="value">
    ///     The maximum height value to set. Must be used to define the actual maximum height of control
    ///     excluding label and hint.
    /// </param>
    public static void SetMaxHeight(TemplatedControl control, double value) => control.SetValue(MaxHeightProperty, value);

    /// <summary>
    ///     Defines an attached property that determines if a control should be read-only.
    /// </summary>
    public static readonly AttachedProperty<bool> ReadOnlyProperty =
        AvaloniaProperty.RegisterAttached<TemplatedControl, bool>("ReadOnly", typeof(ControlAssist));

    /// <summary>
    ///     Gets the value of the <see cref="ReadOnlyProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>A boolean value indicating whether the control is read-only.</returns>
    public static bool GetReadOnly(TemplatedControl control) => control.GetValue(ReadOnlyProperty);

    /// <summary>
    ///     Sets the value of the <see cref="ReadOnlyProperty" />.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="value">
    ///     The value to set - true to make the control read-only, false to make it editable.
    /// </param>
    public static void SetReadOnly(TemplatedControl control, bool value) => control.SetValue(ReadOnlyProperty, value);

    /// <summary>
    ///     Defines an attached property that determines if a control should display an icon.
    /// </summary>
    public static readonly AttachedProperty<bool> ShowIconProperty =
        AvaloniaProperty.RegisterAttached<TemplatedControl, bool>("ShowIcon", typeof(ControlAssist));

    /// <summary>
    ///     Sets the value of the <see cref="ShowIconProperty" />.
    /// </summary>
    /// <param name="obj">The control.</param>
    /// <param name="value">
    ///     The value to set - true to show the icon, false to hide it.
    /// </param>
    public static void SetShowIcon(TemplatedControl obj, bool value)
    {
        obj.SetValue(ShowIconProperty, value);
    }

    /// <summary>
    ///     Gets the value of the <see cref="ShowIconProperty" />.
    /// </summary>
    /// <param name="obj">The control.</param>
    /// <returns>A boolean value indicating whether the icon should be displayed.</returns>
    public static bool GetShowIcon(TemplatedControl obj)
    {
        return obj.GetValue(ShowIconProperty);
    }
}