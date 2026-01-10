using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides extension methods for managing window state persistence in Avalonia applications.
///     This class allows windows to automatically save and restore their position, size, and state
///     across application sessions in temporary files.
/// </summary>
public static class WindowExt
{
    private static readonly Dictionary<Window, EventHandler<WindowClosingEventArgs>> ClosingHandlers = new();
    private static readonly Dictionary<Window, EventHandler?> ClosedHandlers = new();
    private static readonly Dictionary<string, WindowSettings?> Cache = new();
    private static readonly object CacheLock = new();

    /// <summary>
    ///     Enables automatic window state management for the specified window.
    ///     The window's position, size, and state will be automatically saved when the window closes
    ///     and restored when the window is opened again.
    /// </summary>
    /// <param name="window">The window to manage state for.</param>
    /// <param name="key">
    ///     A unique identifier for this window's state. Used to distinguish between different windows.
    ///     Defaults to "main" if not specified.
    /// </param>
    public static void ManageWindowState(this Window window, string key = "main")
    {
        if (ClosingHandlers.ContainsKey(window)) return;

        var file = Path.Combine(Path.GetTempPath(), $"shadui_{key}.txt");
        RestoreWindowState(window, file);

        EventHandler<WindowClosingEventArgs> closingHandler = async void (_, _) =>
        {
            try
            {
                var current = new WindowSettings
                {
                    X = window.Position.X,
                    Y = window.Position.Y,
                    Width = window.Width,
                    Height = window.Height,
                    WindowState = window.WindowState
                };

                await SaveWindowSettingsAsync(current, file);
            }
            catch (Exception)
            {
                //ignore
            }
        };
        window.Closing += closingHandler;
        ClosingHandlers[window] = closingHandler;

        EventHandler? closedHandler = null;
        closedHandler = (_, _) =>
        {
            window.Closed -= closedHandler;
            window.UnmanageWindowState();
        };
        window.Closed += closedHandler;
        ClosedHandlers[window] = closedHandler;
    }

    /// <summary>
    ///     Disables automatic window state management for the specified window.
    ///     Removes the event handler that was previously attached by <see cref="ManageWindowState" />.
    /// </summary>
    /// <param name="window">The window to stop managing state for.</param>
    public static void UnmanageWindowState(this Window window)
    {
        if (ClosingHandlers.TryGetValue(window, out var closingHandler))
        {
            window.Closing -= closingHandler;
            ClosingHandlers.Remove(window);
        }

        if (ClosedHandlers.TryGetValue(window, out var closedHandler))
        {
            if (closedHandler != null)
                window.Closed -= closedHandler;
            ClosedHandlers.Remove(window);
        }
    }

    /// <summary>
    /// Clears the cached window settings. Call during application shutdown if needed.
    /// </summary>
    public static void ClearCache()
    {
        lock (CacheLock)
        {
            Cache.Clear();
        }
    }

    private static void RestoreWindowState(Window window, string file)
    {
        WindowSettings? state;

        lock (CacheLock)
        {
            if (!Cache.TryGetValue(file, out state))
            {
                if (File.Exists(file))
                {
                    try
                    {
                        state = LoadWindowSettings(file);
                    }
                    catch
                    {
                        //ignore
                    }
                }

                Cache[file] = state;
            }
        }

        if (state == null) return;

        window.Position = new PixelPoint(state.X, state.Y);

        if (state.WindowState == WindowState.Normal)
        {
            window.Width = state.Width;
            window.Height = state.Height;
        }

        window.WindowState = state.WindowState == WindowState.Minimized
            ? WindowState.Normal
            : state.WindowState;
    }

    private static WindowSettings? LoadWindowSettings(string file)
    {
        if (!File.Exists(file))
        {
            return null;
        }

        var settings = new WindowSettings();
        var lines = File.ReadAllLines(file);

        foreach (var line in lines)
        {
            var lineSpan = line.AsSpan();
            var equalsIndex = lineSpan.IndexOf('=');
            if (equalsIndex <= 0 || equalsIndex >= lineSpan.Length - 1) continue;

            var keySpan = lineSpan.Slice(0, equalsIndex).Trim();
            var valueSpan = lineSpan.Slice(equalsIndex + 1).Trim();

            if (keySpan.SequenceEqual("X".AsSpan()) && int.TryParse(valueSpan, out var x))
            {
                settings.X = x;
            }
            else if (keySpan.SequenceEqual("Y".AsSpan()) && int.TryParse(valueSpan, out var y))
            {
                settings.Y = y;
            }
            else if (keySpan.SequenceEqual("Width".AsSpan()) && double.TryParse(valueSpan, out var width))
            {
                settings.Width = width;
            }
            else if (keySpan.SequenceEqual("Height".AsSpan()) && double.TryParse(valueSpan, out var height))
            {
                settings.Height = height;
            }
            else if (keySpan.SequenceEqual("WindowState".AsSpan()) &&
                     Enum.TryParse<WindowState>(valueSpan.ToString(), out var windowState))
            {
                settings.WindowState = windowState;
            }
        }

        return settings;
    }

    private static async Task SaveWindowSettingsAsync(WindowSettings settings, string file)
    {
        var lines = new string[5];
        lines[0] = $"X={settings.X}";
        lines[1] = $"Y={settings.Y}";
        lines[2] = $"Width={settings.Width}";
        lines[3] = $"Height={settings.Height}";
        lines[4] = $"WindowState={settings.WindowState}";

        await File.WriteAllLinesAsync(file, lines);
    }

    /// <summary>
    ///     Represents the state of a window including its position, size, and window state.
    ///     This class is used internally to serialize and deserialize window state information
    ///     to and from temporary files.
    /// </summary>
    private class WindowSettings
    {
        /// <summary>
        ///     Gets or sets the X coordinate of the window's position on screen.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///     Gets or sets the Y coordinate of the window's position on screen.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        ///     Gets or sets the width of the window in pixels.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        ///     Gets or sets the height of the window in pixels.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        ///     Gets or sets the current state of the window (Normal, Minimized, Maximized).
        /// </summary>
        public WindowState WindowState { get; set; }
    }

    internal static void AddResizeGrip(this Window window, Panel rootPanel)
    {
        var resizeBorders = new[]
        {
            new
            {
                Tag = "North",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Cursor = StandardCursorType.SizeNorthSouth,
                IsCorner = false
            },
            new
            {
                Tag = "South",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Cursor = StandardCursorType.SizeNorthSouth,
                IsCorner = false
            },
            new
            {
                Tag = "West",
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Left,
                Cursor = StandardCursorType.SizeWestEast,
                IsCorner = false
            },
            new
            {
                Tag = "East",
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Right,
                Cursor = StandardCursorType.SizeWestEast,
                IsCorner = false
            },

            new
            {
                Tag = "NW",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Cursor = StandardCursorType.TopLeftCorner,
                IsCorner = true
            },
            new
            {
                Tag = "NE",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Cursor = StandardCursorType.TopRightCorner,
                IsCorner = true
            },
            new
            {
                Tag = "SW",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                Cursor = StandardCursorType.BottomLeftCorner,
                IsCorner = true
            },
            new
            {
                Tag = "SE",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Cursor = StandardCursorType.BottomRightCorner,
                IsCorner = true
            }
        };

        foreach (var config in resizeBorders)
        {
            var border = new Border
            {
                Tag = config.Tag,
                Background = Brushes.Transparent,
                Cursor = new Cursor(config.Cursor)
            };

            if (config.IsCorner)
            {
                border.Width = 8;
                border.Height = 8;
            }
            else
            {
                if (config.VerticalAlignment == VerticalAlignment.Stretch) border.Width = 6;
                if (config.HorizontalAlignment == HorizontalAlignment.Stretch) border.Height = 6;
            }

            border.VerticalAlignment = config.VerticalAlignment;
            border.HorizontalAlignment = config.HorizontalAlignment;

            border.PointerPressed += RaiseResize;
            rootPanel.Children.Add(border);
        }

        void RaiseResize(object? sender, PointerPressedEventArgs e)
        {
            if (!window.CanResize) return;
            if (sender is not Border { Tag: string edge }) return;
            if (window.GetVisualRoot() is not Window w) return;

            var windowEdge = edge switch
            {
                "North" => WindowEdge.North,
                "South" => WindowEdge.South,
                "West" => WindowEdge.West,
                "East" => WindowEdge.East,
                "NW" => WindowEdge.NorthWest,
                "NE" => WindowEdge.NorthEast,
                "SW" => WindowEdge.SouthWest,
                "SE" => WindowEdge.SouthEast,
                _ => throw new ArgumentOutOfRangeException()
            };

            w.BeginResizeDrag(windowEdge, e);
            e.Handled = true;
        }
    }
}