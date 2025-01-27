﻿using Avalonia;
using Avalonia.Controls;

namespace ShadUI.Extensions;

/// <summary>
///     Useful extensions for the <see cref="Avalonia.Controls.TextBox" /> class.
/// </summary>
public static class TextBox
{
    static TextBox()
    {
        LabelProperty.Changed.AddClassHandler<Avalonia.Controls.TextBox>((textBox, args) =>
        {
            textBox.TemplateApplied += (sender, eventArgs) =>
            {
                var tb = (Avalonia.Controls.TextBox) sender;

                var label = eventArgs.NameScope.Find<TextBlock>("PART_Label");
                if (label is null || string.IsNullOrEmpty(args.NewValue?.ToString())) return;

                tb.UseFloatingWatermark = true;
                label.Text = args.NewValue!.ToString();
            };
        });
    }

    /// <summary>
    ///     Override the default text box floating watermark.
    /// </summary>
    public static readonly AttachedProperty<string> LabelProperty =
        AvaloniaProperty.RegisterAttached<Avalonia.Controls.TextBox, string>("Label", typeof(Avalonia.Controls.TextBox));

    /// <summary>
    ///     Get the value of the <see cref="LabelProperty" />.
    /// </summary>
    /// <param name="textBox"></param>
    /// <returns></returns>
    public static string GetLabel(Avalonia.Controls.TextBox textBox) => textBox.GetValue(LabelProperty);

    /// <summary>
    ///     Set the value of the <see cref="LabelProperty" />.
    /// </summary>
    /// <param name="textBox"></param>
    /// <param name="value"></param>
    public static void SetLabel(Avalonia.Controls.TextBox textBox, string value)
    {
        textBox.SetValue(LabelProperty, value);
    }

    /// <summary>
    ///     Show a hint text.
    /// </summary>
    public static readonly AttachedProperty<string> HintProperty =
        AvaloniaProperty.RegisterAttached<Avalonia.Controls.TextBox, string>("Hint", typeof(Avalonia.Controls.TextBox));

    /// <summary>
    ///     Get the value of the <see cref="HintProperty" />.
    /// </summary>
    /// <param name="textBox"></param>
    /// <returns></returns>
    public static string GetHint(Avalonia.Controls.TextBox textBox) => textBox.GetValue(HintProperty);

    /// <summary>
    ///     Set the value of the <see cref="HintProperty" />.
    /// </summary>
    /// <param name="textBox"></param>
    /// <param name="value"></param>
    public static void SetHint(Avalonia.Controls.TextBox textBox, string value)
    {
        textBox.SetValue(HintProperty, value);
    }

    /// <summary>
    ///     Indicates whether the text box should show progress.
    /// </summary>
    public static readonly AttachedProperty<bool> ShowProgressProperty =
        AvaloniaProperty.RegisterAttached<Avalonia.Controls.TextBox, bool>("ShowProgress", typeof(Avalonia.Controls.TextBox));

    /// <summary>
    ///     Gets the value of the <see cref="ShowProgressProperty" />.
    /// </summary>
    /// <param name="textBox">The text box.</param>
    /// <returns>The value of the <see cref="ShowProgressProperty" />.</returns>
    public static bool GetShowProgress(Avalonia.Controls.TextBox textBox) => textBox.GetValue(ShowProgressProperty);

    /// <summary>
    ///     Sets the value of the <see cref="ShowProgressProperty" />.
    /// </summary>
    /// <param name="textBox">The text box.</param>
    /// <param name="value">The value to set.</param>
    public static void SetShowProgress(Avalonia.Controls.TextBox textBox, bool value)
    {
        textBox.SetValue(ShowProgressProperty, value);
    }
}