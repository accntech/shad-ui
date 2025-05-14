using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using LucideAvalonia;
using LucideAvalonia.Enum;
using ShadUI.Themes;

namespace ShadUI.Demo.Converters;

public class ThemeToIconConverter : IValueConverter
{
    private static readonly Dictionary<ThemeMode, Lucide> ThemeIconDictionary = new()
        {
            [ThemeMode.System] = new Lucide { Icon = LucideIconNames.SunMoon, StrokeThickness = 1.5, Width = 18, Height = 18 },
            [ThemeMode.Light]  = new Lucide { Icon = LucideIconNames.Sun,     StrokeThickness = 1.5, Width = 18, Height = 18 },
            [ThemeMode.Dark]   = new Lucide { Icon = LucideIconNames.Moon,    StrokeThickness = 1.5, Width = 18, Height = 18 },
        };

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ThemeMode mode && ThemeIconDictionary.TryGetValue(mode, out var icon))
            return icon;

        if (value is not int idx)
            return ThemeIconDictionary[ThemeMode.System];
        
        var modes = new[] { ThemeMode.System, ThemeMode.Light, ThemeMode.Dark };
        if (idx >= 0 && idx < modes.Length && ThemeIconDictionary.TryGetValue(modes[idx], out icon))
            return icon;

        return ThemeIconDictionary[ThemeMode.System];
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}