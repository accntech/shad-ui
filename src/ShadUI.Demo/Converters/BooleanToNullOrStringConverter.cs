﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ShadUI.Demo.Converters;

public class BooleanToNullOrStringConverter : IValueConverter
{
    public static BooleanToNullOrStringConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var stringValue = parameter?.ToString();

        if (value is bool booleanValue) return booleanValue ? null : stringValue;

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}