using System;
using System.Globalization;
using Avalonia.Data.Converters;
using LucideAvalonia;
using LucideAvalonia.Enum;
using ShadUI.Themes;

public class ThemeToIconConverter : IValueConverter
{
    private static readonly Lucide[] ThemeIcons =
    [
        new() { Icon = LucideIconNames.SunMoon, StrokeThickness = 1.5, Width = 18, Height = 18 },
        new() { Icon = LucideIconNames.Sun,      StrokeThickness = 1.5, Width = 18, Height = 18 },
        new() { Icon = LucideIconNames.Moon,     StrokeThickness = 1.5, Width = 18, Height = 18 }
    ];

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        switch (value)
        {
            case ThemeMode mode:
            {
                var index = mode switch
                {
                    ThemeMode.System => 0,
                    ThemeMode.Light  => 1,
                    ThemeMode.Dark   => 2,
                    _                => 0
                };
                return ThemeIcons[index];
            }
            case int idx and >= 0 when idx < ThemeIcons.Length:
                return ThemeIcons[idx];
            default:
                return ThemeIcons[0];
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}