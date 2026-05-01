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
///     A control that allows users to input and display date values in day, month, and year format.
/// </summary>
[TemplatePart("PART_DayTextBox", typeof(TextBox))]
[TemplatePart("PART_MonthTextBox", typeof(TextBox))]
[TemplatePart("PART_YearTextBox", typeof(TextBox))]
public class DateInput : TemplatedControl
{
    /// <summary>
    ///     Defines the <see cref="Value" /> property.
    /// </summary>
    public static readonly StyledProperty<DateTimeOffset?> ValueProperty =
        AvaloniaProperty.Register<DateInput, DateTimeOffset?>(nameof(Value),
            defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    /// <summary>
    ///     Gets or sets the current date value.
    /// </summary>
    public DateTimeOffset? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="DayString" /> property.
    /// </summary>
    internal static readonly StyledProperty<string> DayStringProperty =
        AvaloniaProperty.Register<DateInput, string>(nameof(DayString));

    /// <summary>
    ///     Gets or sets the string representation of the day.
    /// </summary>
    internal string DayString
    {
        get => GetValue(DayStringProperty);
        set => SetValue(DayStringProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="MonthString" /> property.
    /// </summary>
    internal static readonly StyledProperty<string> MonthStringProperty =
        AvaloniaProperty.Register<DateInput, string>(nameof(MonthString));

    /// <summary>
    ///     Gets or sets the string representation of the month.
    /// </summary>
    internal string MonthString
    {
        get => GetValue(MonthStringProperty);
        set => SetValue(MonthStringProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="YearString" /> property.
    /// </summary>
    internal static readonly StyledProperty<string> YearStringProperty =
        AvaloniaProperty.Register<DateInput, string>(nameof(YearString));

    /// <summary>
    ///     Gets or sets the string representation of the year.
    /// </summary>
    internal string YearString
    {
        get => GetValue(YearStringProperty);
        set => SetValue(YearStringProperty, value);
    }

    /// <summary>
    ///     Defines the <see cref="InputFocus" /> property.
    /// </summary>
    internal static readonly StyledProperty<bool> InputFocusProperty =
        AvaloniaProperty.Register<DateInput, bool>(nameof(InputFocus));

    /// <summary>
    ///     Gets or sets whether any part of the control has input focus.
    /// </summary>
    internal bool InputFocus
    {
        get => GetValue(InputFocusProperty);
        set => SetValue(InputFocusProperty, value);
    }

    private TextBox? _dayTextBox;
    private TextBox? _monthTextBox;
    private TextBox? _yearTextBox;

    /// <summary>
    ///     Called when the template is applied to the control.
    ///     Sets up event handlers and initializes the control state.
    /// </summary>
    /// <param name="e">Contains information about the template being applied.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var dayTextBox = e.NameScope.Get<TextBox>("PART_DayTextBox");
        var monthTextBox = e.NameScope.Get<TextBox>("PART_MonthTextBox");
        var yearTextBox = e.NameScope.Get<TextBox>("PART_YearTextBox");

        _dayTextBox = dayTextBox;
        _monthTextBox = monthTextBox;
        _yearTextBox = yearTextBox;

        _dayTextBox.LostFocus += OnTextBoxLostFocus;
        _monthTextBox.LostFocus += OnTextBoxLostFocus;
        _yearTextBox.LostFocus += OnTextBoxLostFocus;

        _dayTextBox.GotFocus += (_, _) => InputFocus = true;
        _monthTextBox.GotFocus += (_, _) => InputFocus = true;
        _yearTextBox.GotFocus += (_, _) => InputFocus = true;

        _dayTextBox.TextChanged += OnInputChanged;
        _monthTextBox.TextChanged += OnInputChanged;
        _yearTextBox.TextChanged += OnInputChanged;
    }

    private bool _updating;

    private void OnInputChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        var maxLength = textBox.Name == "PART_YearTextBox" ? 4 : 2;
        if (textBox.Text?.Length < maxLength) return;

        var nextTextBox = textBox.Name switch
        {
            "PART_MonthTextBox" => _dayTextBox,
            "PART_DayTextBox" => _yearTextBox,
            _ => null
        };

        if (nextTextBox is null) return;

        nextTextBox.Focus();
        nextTextBox.SelectAll();
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        InputFocus = false;

        if (sender is not TextBox textBox) return;
        var parsed = int.TryParse(textBox.Text, out var value);

        if (string.IsNullOrWhiteSpace(textBox.Text) || !parsed)
        {
            textBox.Text = string.Empty;
            return;
        }

        switch (textBox.Name)
        {
            case "PART_DayTextBox":
                // Get current month and year
                int.TryParse(_monthTextBox?.Text, out var month);
                int.TryParse(_yearTextBox?.Text, out var year);
                if (month is >= 1 and <= 12 && year > 0)
                {
                    // Get the last day of the month
                    var lastDay = DateTime.DaysInMonth(year, month);
                    if (value > lastDay) value = lastDay;
                }
                else if (value > 31)
                {
                    value = 31;
                }

                textBox.Text = value.ToString().PadLeft(2, '0');
                break;
            case "PART_MonthTextBox":
                if (value > 12) value = 12;
                textBox.Text = value.ToString().PadLeft(2, '0');
                // After month changes, validate day
                int.TryParse(_dayTextBox?.Text, out var currentDay);
                int.TryParse(_yearTextBox?.Text, out var currentYear);
                if (currentDay > 0 && currentYear > 0)
                {
                    var maxDays = DateTime.DaysInMonth(currentYear, value);
                    if (currentDay > maxDays) _dayTextBox!.Text = maxDays.ToString().PadLeft(2, '0');
                }

                break;
            case "PART_YearTextBox":
                if (!string.IsNullOrEmpty(textBox.Text) && textBox.Text!.Length < 4)
                {
                    var yearNow = DateTime.Now.Year;
                    var factor = yearNow / 1000;
                    var remainder = value % (1000 * factor);
                    if (remainder < 1000) value = factor * 1000 + remainder;
                }
                else if (value > 9999)
                {
                    value = 9999;
                }

                textBox.Text = value < 1 ? "" : value.ToString().PadLeft(4, '0');
                // After year changes, validate day for February in leap years
                int.TryParse(_dayTextBox?.Text, out var day);
                int.TryParse(_monthTextBox?.Text, out var currentMonth);
                if (day > 0 && currentMonth == 2)
                {
                    var maxDays = DateTime.DaysInMonth(value, 2);
                    if (day > maxDays) _dayTextBox!.Text = maxDays.ToString().PadLeft(2, '0');
                }

                break;
        }

        UpdateDateValue();
    }

    private void UpdateDateValue()
    {
        if (_dayTextBox == null || _monthTextBox == null || _yearTextBox == null) return;

        if (!int.TryParse(_dayTextBox.Text, out var day) ||
            !int.TryParse(_monthTextBox.Text, out var month) ||
            !int.TryParse(_yearTextBox.Text, out var year))
        {
            return;
        }

        _updating = true;

        try
        {
            var currentYear = DateTime.Now.Year;
            var factor = currentYear / 1000;
            var remainder = year % (1000 * factor);
            if (remainder < 1000) year = factor * 1000 + remainder;

            Value = new DateTimeOffset(new DateTime(year, month, day));
        }
        catch (ArgumentOutOfRangeException)
        {
            //ignore
        }

        _updating = false;
    }

    /// <summary>
    ///     Called when a property value changes. Handles updates to the date value and updates the display strings.
    /// </summary>
    /// <param name="e">Information about the property that changed.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (_updating) return;
        if (e.Property == ValueProperty) UpdateValue(e);
        if (e.Property == DayStringProperty) UpdateInput();
        if (e.Property == MonthStringProperty) UpdateInput();
        if (e.Property == YearStringProperty) UpdateInput();
    }

    private void UpdateValue(AvaloniaPropertyChangedEventArgs args)
    {
        _updating = true;

        var currentValue = args.GetNewValue<DateTimeOffset?>();

        if (currentValue.HasValue)
        {
            DayString = currentValue.Value.Day.ToString().PadLeft(2, '0');
            MonthString = currentValue.Value.Month.ToString().PadLeft(2, '0');
            YearString = currentValue.Value.Year.ToString();
        }
        else
        {
            DayString = "01";
            MonthString = "01";
            YearString = DateTime.Now.Year.ToString();
        }

        _updating = false;
    }

    private void UpdateInput()
    {
        if (_updating) return;
        UpdateDateValue();
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