using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Watches and manages theme changes in the application, providing access to current theme colors
///     and notifications when the theme changes.
/// </summary>
public class ThemeWatcher
{
    private readonly Application _app;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ThemeWatcher" /> class.
    /// </summary>
    /// <param name="app">The Avalonia application instance to watch for theme changes.</param>
    public ThemeWatcher(Application app)
    {
        _app = app;
        _app.ActualThemeVariantChanged += OnThemeChanged;
    }

    /// <summary>
    ///     Gets the current theme colors used in the application.
    /// </summary>
    public ThemeColors ThemeColors { get; private set; } = new();

    /// <summary>
    ///     Handles theme change events and updates the current theme colors.
    /// </summary>
    private void OnThemeChanged(object? sender, EventArgs e)
    {
        var colors = GetThemeColors();
        ThemeColors = colors;
        ThemeChanged?.Invoke(this, colors);
    }

    /// <summary>
    ///     Initializes the theme watcher by setting the initial theme colors.
    /// </summary>
    public void Initialize()
    {
        ThemeColors = GetThemeColors();
    }

    /// <summary>
    ///     Retrieves the current theme colors from the application resources.
    /// </summary>
    /// <returns>A new <see cref="ThemeColors" /> instance containing all theme color values.</returns>
    private ThemeColors GetThemeColors()
    {
        return new ThemeColors
        {
            // Basic Colors
            ForegroundColor = TryGetColor("ForegroundColor"),
            ForegroundLeadColor = TryGetColor("ForegroundLeadColor"),
            BackgroundColor = TryGetColor("BackgroundColor"),
            MutedColor = TryGetColor("MutedColor"),
            BorderColor = TryGetColor("BorderColor"),
            BorderColor60 = TryGetColor("BorderColor60"),
            BorderColor30 = TryGetColor("BorderColor30"),
            OutlineColor = TryGetColor("OutlineColor"),
            GhostColor = TryGetColor("GhostColor"),
            GhostHoverColor = TryGetColor("GhostHoverColor"),
            GhostHoverColor50 = TryGetColor("GhostHoverColor50"),

            // Theme Colors
            PrimaryColor = TryGetColor("PrimaryColor"),
            PrimaryColor75 = TryGetColor("PrimaryColor75"),
            PrimaryColor50 = TryGetColor("PrimaryColor50"),
            PrimaryColor10 = TryGetColor("PrimaryColor10"),
            PrimaryForegroundColor = TryGetColor("PrimaryForegroundColor"),
            SecondaryColor = TryGetColor("SecondaryColor"),
            SecondaryColor75 = TryGetColor("SecondaryColor75"),
            SecondaryColor50 = TryGetColor("SecondaryColor50"),
            SecondaryForegroundColor = TryGetColor("SecondaryForegroundColor"),
            DestructiveColor = TryGetColor("DestructiveColor"),
            DestructiveColor75 = TryGetColor("DestructiveColor75"),
            DestructiveColor50 = TryGetColor("DestructiveColor50"),
            DestructiveColor10 = TryGetColor("DestructiveColor10"),
            DestructiveForegroundColor = TryGetColor("DestructiveForegroundColor"),

            // Notification Colors
            InfoColor = TryGetColor("InfoColor"),
            InfoColor60 = TryGetColor("InfoColor60"),
            InfoColor20 = TryGetColor("InfoColor20"),
            InfoColor10 = TryGetColor("InfoColor10"),
            InfoColor5 = TryGetColor("InfoColor5"),
            SuccessColor = TryGetColor("SuccessColor"),
            SuccessColor60 = TryGetColor("SuccessColor60"),
            SuccessColor20 = TryGetColor("SuccessColor20"),
            SuccessColor10 = TryGetColor("SuccessColor10"),
            SuccessColor5 = TryGetColor("SuccessColor5"),
            WarningColor = TryGetColor("WarningColor"),
            WarningColor60 = TryGetColor("WarningColor60"),
            WarningColor20 = TryGetColor("WarningColor20"),
            WarningColor10 = TryGetColor("WarningColor10"),
            WarningColor5 = TryGetColor("WarningColor5"),
            ErrorColor = TryGetColor("ErrorColor"),
            ErrorColor60 = TryGetColor("ErrorColor60"),
            ErrorColor20 = TryGetColor("ErrorColor20"),
            ErrorColor10 = TryGetColor("ErrorColor10"),
            ErrorColor5 = TryGetColor("ErrorColor5"),

            // Specific Control Colors
            BusyAreaOverlayColor = TryGetColor("BusyAreaOverlayColor"),
            CardBackgroundColor = TryGetColor("CardBackgroundColor"),
            DialogOverlayColor = TryGetColor("DialogOverlayColor"),
            DialogBackgroundColor = TryGetColor("DialogBackgroundColor"),
            TitleBarBackgroundColor = TryGetColor("TitleBarBackgroundColor"),
            WindowBackgroundColor = TryGetColor("WindowBackgroundColor"),
            WindowButtonHoverColor = TryGetColor("WindowButtonHoverColor"),
            SidebarBackgroundColor = TryGetColor("SidebarBackgroundColor"),
            SwitchBackgroundColor = TryGetColor("SwitchBackgroundColor"),
            SwitchForegroundColor = TryGetColor("SwitchForegroundColor"),
            TabItemSelectedColor = TryGetColor("TabItemSelectedColor"),
            TabItemsBackgroundColor = TryGetColor("TabItemsBackgroundColor")
        };
    }

    /// <summary>
    ///     Attempts to find a color resource by its key in the application resources.
    /// </summary>
    /// <param name="resourceKey">The key of the color resource to find.</param>
    /// <returns>The color if found; otherwise, returns the default color value.</returns>
    private Color TryGetColor(string resourceKey)
    {
        if (_app.TryFindResource(resourceKey, _app.ActualThemeVariant, out var resource) && resource is Color color)
        {
            return color;
        }

        return default;
    }

    /// <summary>
    ///     Occurs when the application theme changes, providing the new theme colors.
    /// </summary>
    public event EventHandler<ThemeColors>? ThemeChanged;

    /// <summary>
    ///     Switches the theme of the application based on the specified mode.
    /// </summary>
    /// <param name="mode">The theme mode to switch to.</param>
    public void SwitchTheme(ThemeMode mode)
    {
        var variant = mode switch
        {
            ThemeMode.Dark => ThemeVariant.Dark,
            ThemeMode.Light => ThemeVariant.Light,
            _ => ThemeVariant.Default
        };

        Dispatcher.UIThread.Invoke(() => _app.RequestedThemeVariant = variant);
    }
}