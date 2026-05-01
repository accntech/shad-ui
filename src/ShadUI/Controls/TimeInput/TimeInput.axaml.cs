using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     A control that allows users to input and display time values in hours, minutes, and optionally seconds.
///     Supports both 12-hour and 24-hour time formats.
/// </summary>
[TemplatePart("PART_HourTextBox", typeof(TextBox))]
[TemplatePart("PART_MinuteTextBox", typeof(TextBox))]
[TemplatePart("PART_SecondTextBox", typeof(TextBox))]
[TemplatePart("PART_ToggleButton", typeof(ToggleButton))]
public class TimeInput : TemplatedControl
{
    /// <summary>
    ///     Defines the <see cref="Value" /> property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan?> ValueProperty =
        AvaloniaProperty.Register<TimeInput, TimeSpan?>(nameof(Value),
            defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    /// <summary>
    ///     Gets or sets the current time value.
    /// </summary>
    public TimeSpan? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="HourString" /> property.
    /// </summary>
    internal static readonly StyledProperty<string> HourStringProperty =
        AvaloniaProperty.Register<TimeInput, string>(nameof(HourString), "00");

    /// <summary>
    ///     Gets or sets the string representation of the hours.
    /// </summary>
    internal string HourString
    {
        get => GetValue(HourStringProperty);
        set => SetValue(HourStringProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="MinuteString" /> property.
    /// </summary>
    internal static readonly StyledProperty<string> MinuteStringProperty =
        AvaloniaProperty.Register<TimeInput, string>(nameof(MinuteString), "00");

    /// <summary>
    ///     Gets or sets the string representation of the minutes.
    /// </summary>
    internal string MinuteString
    {
        get => GetValue(MinuteStringProperty);
        set => SetValue(MinuteStringProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="SecondString" /> property.
    /// </summary>
    internal static readonly StyledProperty<string> SecondStringProperty =
        AvaloniaProperty.Register<TimeInput, string>(nameof(SecondString), "00");

    /// <summary>
    ///     Gets or sets the string representation of the seconds.
    /// </summary>
    internal string SecondString
    {
        get => GetValue(SecondStringProperty);
        set => SetValue(SecondStringProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="ClockIdentifier" /> property.
    /// </summary>
    public static readonly StyledProperty<string> ClockIdentifierProperty =
        AvaloniaProperty.Register<TimePicker, string>(nameof(ClockIdentifier), "12HourClock",
            coerce: CoerceClockIdentifier);

    /// <summary>
    ///     Gets or sets the clock format identifier. Valid values are "12HourClock" or "24HourClock".
    /// </summary>
    public string ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }

    /// <summary>
    ///     Coerces the clock identifier value to ensure it's valid.
    /// </summary>
    /// <param name="sender">The object that is being coerced.</param>
    /// <param name="value">The value to coerce.</param>
    /// <returns>The coerced value.</returns>
    /// <exception cref="ArgumentException">Thrown when the clock identifier is invalid.</exception>
    private static string CoerceClockIdentifier(AvaloniaObject sender, string value)
    {
        if (!(string.IsNullOrEmpty(value) || value == "12HourClock" || value == "24HourClock"))
        {
            throw new ArgumentException("Invalid ClockIdentifier", default(string));
        }

        return value;
    }

    /// <summary>
    ///     Defines the <see cref="UseSeconds" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> UseSecondsProperty =
        AvaloniaProperty.Register<TimeInput, bool>(nameof(UseSeconds));

    /// <summary>
    ///     Gets or sets whether seconds should be displayed and editable.
    /// </summary>
    public bool UseSeconds
    {
        get => GetValue(UseSecondsProperty);
        set => SetValue(UseSecondsProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="InputFocus" /> property.
    /// </summary>
    internal static readonly StyledProperty<bool> InputFocusProperty =
        AvaloniaProperty.Register<TimeInput, bool>(nameof(InputFocus));

    /// <summary>
    ///     Gets or sets whether any part of the control has input focus.
    /// </summary>
    internal bool InputFocus
    {
        get => GetValue(InputFocusProperty);
        set => SetValue(InputFocusProperty, value);
    }

    private TextBox? _hourTextBox;
    private TextBox? _minuteTextBox;
    private TextBox? _secondTextBox;
    private ToggleButton? _toggleButton;

    /// <summary>
    ///     Called when the template is applied to the control.
    ///     Sets up event handlers and initializes the control state.
    /// </summary>
    /// <param name="e">Contains information about the template being applied.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var hourTextBox = e.NameScope.Get<TextBox>("PART_HourTextBox");
        var minuteTextBox = e.NameScope.Get<TextBox>("PART_MinuteTextBox");
        var secondTextBox = e.NameScope.Get<TextBox>("PART_SecondTextBox");
        var toggleButton = e.NameScope.Get<ToggleButton>("PART_ToggleButton");

        _hourTextBox = hourTextBox;
        _minuteTextBox = minuteTextBox;
        _secondTextBox = secondTextBox;
        _toggleButton = toggleButton;

        _toggleButton.IsChecked = Value is { Hours: >= 12 };

        _hourTextBox.LostFocus += OnTextBoxLostFocus;
        _minuteTextBox.LostFocus += OnTextBoxLostFocus;
        _secondTextBox.LostFocus += OnTextBoxLostFocus;
        _toggleButton.LostFocus += (_, _) => InputFocus = false;

        _hourTextBox.GotFocus += (_, _) => InputFocus = true;
        _minuteTextBox.GotFocus += (_, _) => InputFocus = true;
        _secondTextBox.GotFocus += (_, _) => InputFocus = true;
        _toggleButton.GotFocus += (_, _) => InputFocus = true;

        _hourTextBox.TextChanged += OnInputChanged;
        _minuteTextBox.TextChanged += OnInputChanged;
        _secondTextBox.TextChanged += OnInputChanged;
        _toggleButton.Click += OnToggleButtonCheckChanged;

        _hourTextBox.KeyDown += (_, _) => _fromInput = true;
        _minuteTextBox.KeyDown += (_, _) => _fromInput = true;
        _secondTextBox.KeyDown += (_, _) => _fromInput = true;

        _hourTextBox.KeyUp += (_, _) => _fromInput = false;
        _minuteTextBox.KeyUp += (_, _) => _fromInput = false;
        _secondTextBox.KeyUp += (_, _) => _fromInput = false;

        if (ClockIdentifier != "12HourClock") return;

        if (_toggleButton is not null) _toggleButton.IsChecked = Value?.Hours >= 12;

        var hours = Value?.Hours ?? 0;
        if (hours > 12) hours -= 12;
        if (_hourTextBox is not null) _hourTextBox.Text = hours.ToString().PadLeft(2, '0');
    }

    private bool _updating;
    private bool _fromInput;

    private void OnInputChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox || !_fromInput) return;

        if (textBox.Text?.Length < 2) return;

        var nextTextBox = textBox.Name switch
        {
            "PART_HourTextBox" => _minuteTextBox,
            "PART_MinuteTextBox" => _secondTextBox,
            _ => null
        };

        nextTextBox?.Focus();
        nextTextBox?.SelectAll();
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        InputFocus = false;

        if (sender is not TextBox textBox) return;
        int.TryParse(textBox.Text, out var value);

        if (textBox.Name == "PART_HourTextBox")
        {
            if (value >= 24) value = 0;

            switch (ClockIdentifier)
            {
                case "12HourClock" when value >= 12:
                    textBox.Text = value == 12 ? "12" : (value - 12).ToString().PadLeft(2, '0');
                    _toggleButton!.IsChecked = true;
                    Value = new TimeSpan(value, Value?.Minutes ?? 0, Value?.Seconds ?? 0);
                    return;
                case "12HourClock" when value == 0:
                    _toggleButton!.IsChecked = false;
                    textBox.Text = "00";
                    Value = new TimeSpan(value, Value?.Minutes ?? 0, Value?.Seconds ?? 0);
                    return;
            }
        }
        else
        {
            if (value >= 60) value = 0;
        }

        textBox.Text = value.ToString().PadLeft(2, '0');
    }

    private void OnToggleButtonCheckChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton toggleButton) return;

        if (toggleButton.IsChecked == true)
        {
            Value ??= new TimeSpan(12, 0, 0);

            if (Value is { Hours: < 12 })
            {
                Value = new TimeSpan(Value.Value.Hours + 12, Value.Value.Minutes, Value.Value.Seconds);
            }

            if (ClockIdentifier == "12HourClock" && _hourTextBox!.Text == "00") _hourTextBox.Text = "12";
        }
        else
        {
            if (Value is { Hours: >= 12 })
            {
                Value = new TimeSpan(Value.Value.Hours - 12, Value.Value.Minutes, Value.Value.Seconds);
            }

            if (ClockIdentifier == "12HourClock" && _hourTextBox!.Text == "12") _hourTextBox.Text = "00";
        }
    }

    /// <summary>
    ///     Called when a property value changes. Handles updates to the time value and updates the display strings.
    /// </summary>
    /// <param name="e">Information about the property that changed.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (_updating) return;
        if (e.Property == ValueProperty) UpdateValue(e);
        if (e.Property == HourStringProperty) UpdateHour(e);
        if (e.Property == MinuteStringProperty) UpdateMinute(e);
        if (e.Property == SecondStringProperty) UpdateSecond(e);
    }

    private void UpdateValue(AvaloniaPropertyChangedEventArgs args)
    {
        _updating = true;

        var time = args.GetNewValue<TimeSpan?>();

        if (time.HasValue)
        {
            var hour = time.Value.Hours;

            HourString = hour.ToString().PadLeft(2, '0');
            MinuteString = time.Value.Minutes.ToString().PadLeft(2, '0');
            SecondString = UseSeconds ? time.Value.Seconds.ToString().PadLeft(2, '0') : "00";

            if (!UseSeconds)
            {
                Value = new TimeSpan(time.Value.Hours, time.Value.Minutes, 0);
                SecondString = "00";
            }

            if (ClockIdentifier == "12HourClock")
            {
                if (hour > 12) hour -= 12;

                HourString = hour.ToString().PadLeft(2, '0');
                if (_toggleButton is not null) _toggleButton.IsChecked = time.Value.Hours >= 12;
                if (_hourTextBox is not null) _hourTextBox!.Text = hour.ToString().PadLeft(2, '0');
            }
        }
        else
        {
            HourString = "00";
            MinuteString = "00";
            SecondString = "00";
        }

        _updating = false;
    }

    private void UpdateHour(AvaloniaPropertyChangedEventArgs args)
    {
        int.TryParse(args.GetNewValue<string>(), out var value);

        if (value >= 24) value = 0;

        if (ClockIdentifier == "12HourClock" && value < 12 && _toggleButton!.IsChecked == true) value += 12;

        Value = new TimeSpan(value, Value?.Minutes ?? 0, Value?.Seconds ?? 0);
    }

    private void UpdateMinute(AvaloniaPropertyChangedEventArgs args)
    {
        var parsed = int.TryParse(args.GetNewValue<string>(), out var value);

        if (!parsed || value < 0 || value > 59) value = 0;

        Value = new TimeSpan(Value?.Hours ?? 0, value, Value?.Seconds ?? 0);
    }

    private void UpdateSecond(AvaloniaPropertyChangedEventArgs args)
    {
        var parsed = int.TryParse(args.GetNewValue<string>(), out var value);

        if (!parsed || value < 0 || value > 59) value = 0;

        Value = new TimeSpan(Value?.Hours ?? 0, Value?.Minutes ?? 0, value);
    }

    /// <summary>
    ///     Updates the data validation state of the control.
    /// </summary>
    /// <param name="property">The property that is being validated.</param>
    /// <param name="state">The current binding state.</param>
    /// <param name="error">The validation error, if any.</param>
    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        base.UpdateDataValidation(property, state, error);

        if (property == ValueProperty) DataValidationErrors.SetError(this, error);
    }
}