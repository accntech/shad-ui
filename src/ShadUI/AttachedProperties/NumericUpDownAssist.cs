using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides attached properties for enhancing NumericUpDown functionality.
/// </summary>
/// <remarks>
///     The NumericUpDownAssist class offers attached properties that extend the functionality
///     of NumericUpDown controls in Avalonia applications. It provides input restriction
///     capabilities that prevent users from entering non-numeric characters.
///     Example usage in XAML:
///     <code>
///     &lt;NumericUpDown shadui:NumericUpDownAssist.NumericOnly="True" /&gt;
///     </code>
/// </remarks>
public class NumericUpDownAssist
{
    static NumericUpDownAssist()
    {
        NumericOnlyProperty.Changed.AddClassHandler<NumericUpDown>(HandleNumericOnlyChanged);
    }

    /// <summary>
    ///     Gets or sets whether the NumericUpDown should only accept numeric input.
    /// </summary>
    /// <remarks>
    ///     When set to true, this property prevents users from entering non-numeric characters.
    ///     Allowed input includes digits 0-9, decimal point (.), minus sign (-) for negative numbers,
    ///     navigation keys (arrows, home, end), editing keys (backspace, delete),
    ///     and tab for focus navigation.
    /// </remarks>
    public static readonly AttachedProperty<bool> NumericOnlyProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDownAssist, NumericUpDown, bool>(
            "NumericOnly", false);

    /// <summary>
    ///     Sets whether the NumericUpDown should only accept numeric input.
    /// </summary>
    /// <param name="element">The NumericUpDown element.</param>
    /// <param name="value">True to restrict to numeric input; false to allow any input.</param>
    public static void SetNumericOnly(NumericUpDown element, bool value)
    {
        element.SetValue(NumericOnlyProperty, value);
    }

    /// <summary>
    ///     Gets whether the NumericUpDown is restricted to numeric input.
    /// </summary>
    /// <param name="element">The NumericUpDown element.</param>
    /// <returns>True if restricted to numeric input; false otherwise.</returns>
    public static bool GetNumericOnly(NumericUpDown element)
    {
        return element.GetValue(NumericOnlyProperty);
    }

    private static void HandleNumericOnlyChanged(NumericUpDown numericUpDown, AvaloniaPropertyChangedEventArgs args)
    {
        CleanupHandlers(numericUpDown);

        if (args.NewValue is not true) return;

        var textBox = numericUpDown.FindControl<TextBox>("PART_TextBox");

        if (textBox == null)
        {
            void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
            {
                numericUpDown.TemplateApplied -= OnTemplateApplied;

                if (!GetNumericOnly(numericUpDown)) return;

                if (e.NameScope.Find<TextBox>("PART_TextBox") is { } tb)
                {
                    SetupInputFiltering(numericUpDown, tb);
                }
            }

            numericUpDown.TemplateApplied += OnTemplateApplied;
        }
        else
        {
            SetupInputFiltering(numericUpDown, textBox);
        }
    }

    private static void SetupInputFiltering(NumericUpDown numericUpDown, TextBox textBox)
    {
        numericUpDown.SetValue(InputHandlersProperty, new InputHandlers
        {
            TextBox = textBox,
            TextInputHandler = OnTextInput,
            KeyDownHandler = OnKeyDown
        });

        textBox.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
        textBox.KeyDown += OnKeyDown;
        return;

        void OnTextInput(object? sender, TextInputEventArgs e)
        {
            if (sender is not TextBox tb || string.IsNullOrEmpty(e.Text)) return;

            foreach (var c in e.Text)
            {
                if (char.IsDigit(c)) continue;

                var currentText = tb.Text ?? string.Empty;
                var selectionStart = tb.SelectionStart;
                var isAllSelected = tb.SelectionStart == 0 && tb.SelectionEnd == currentText.Length;

                if (c == '-')
                {
                    // Allow minus only at position 0 and if there's no existing minus
                    if (selectionStart == 0 && !currentText.StartsWith("-"))
                        continue;

                    // Also allow if the entire text is selected (replacing all)
                    if (isAllSelected)
                        continue;
                }

                if (c == '.')
                {
                    // Allow decimal point if there's no existing one
                    if (!currentText.Contains("."))
                        continue;

                    // Also allow if the entire text is selected (replacing all)
                    if (isAllSelected)
                        continue;
                }

                e.Handled = true;
                return;
            }
        }

        void OnKeyDown(object? sender, KeyEventArgs e)
        {
            // Reserved for future key filtering if needed
        }
    }

    private static void CleanupHandlers(NumericUpDown? numericUpDown)
    {
        var handlers = numericUpDown?.GetValue(InputHandlersProperty);

        if (handlers is null) return;

        if (handlers.TextBox is not null)
        {
            if (handlers.TextInputHandler is not null)
                handlers.TextBox.RemoveHandler(InputElement.TextInputEvent, handlers.TextInputHandler);

            if (handlers.KeyDownHandler is not null)
                handlers.TextBox.KeyDown -= handlers.KeyDownHandler;
        }

        numericUpDown?.ClearValue(InputHandlersProperty);
    }

    private class InputHandlers
    {
        public TextBox? TextBox { get; set; }
        public EventHandler<TextInputEventArgs>? TextInputHandler { get; set; }
        public EventHandler<KeyEventArgs>? KeyDownHandler { get; set; }
    }

    private static readonly AttachedProperty<InputHandlers> InputHandlersProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDownAssist, NumericUpDown, InputHandlers>("InputHandlers");
}
