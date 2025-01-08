using System;
using Avalonia.Media;
using ShadUI.Enums;

namespace ShadUI.Themes;

public record ColorTheme
{
    public string DisplayName { get; }

    public Color Primary { get; }

    public IBrush PrimaryBrush => new SolidColorBrush(Primary);

    public Color PrimaryDark { get; }

    public IBrush PrimaryDarkBrush => new SolidColorBrush(PrimaryDark);

    public Color Accent { get; }

    public IBrush AccentBrush => new SolidColorBrush(Accent);

    public Color AccentDark { get; }

    public IBrush AccentDarkBrush => new SolidColorBrush(AccentDark);

    // Used in shaders to save calculating them per-frame.
    internal Color BackgroundPrimary { get; }
    internal Color BackgroundAccent { get; }
    internal Color Background { get; }

    private const double darkScale = 0.5;

    public ColorTheme(string displayName, Color primary, Color accent)
    {
        DisplayName = displayName;
        Primary = primary;
        Accent = accent;
        PrimaryDark = new Color(primary.A, (byte) (primary.R * darkScale), (byte) (primary.G * darkScale),
            (byte) (primary.B * darkScale));
        AccentDark = new Color(accent.A, (byte) (accent.R * darkScale), (byte) (accent.G * darkScale),
            (byte) (accent.B * darkScale));
        Background = GetBackgroundColor(Primary);
        BackgroundPrimary = new Color(primary.A, (byte) (primary.R / 1), (byte) (primary.G / 1), (byte) (primary.B / 1));
        BackgroundAccent = new Color(accent.A, (byte) (accent.R / 1), (byte) (accent.G / 1), (byte) (accent.B / 1));
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash *= 31 + Primary.GetHashCode();
            hash *= 31 + Accent.GetHashCode();
            hash *= 31 + DisplayName.GetHashCode();
            return hash;
        }
    }

    public override string ToString() => DisplayName;

    private static Color GetBackgroundColor(Color input)
    {
        int r = input.R;
        int g = input.G;
        int b = input.B;

        var minValue = Math.Min(Math.Min(r, g), b);
        var maxValue = Math.Max(Math.Max(r, g), b);

        r = r == minValue ? 47 : r == maxValue ? 47 : 27;
        g = g == minValue ? 47 : g == maxValue ? 47 : 27;
        b = b == minValue ? 47 : b == maxValue ? 47 : 27;
        return new Color(255, (byte) r, (byte) g, (byte) b);
    }
}

internal record DefaultColorTheme : ColorTheme
{
    internal BaseColor ThemeColor { get; }

    internal DefaultColorTheme(BaseColor themeColor, Color primary, Color accent)
        : base(themeColor.ToString(), primary, accent)
    {
        ThemeColor = themeColor;
    }
}