using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Media;

// Original source code from: https://gist.github.com/tobyfirth/65c5372be2e659141c1c4b7d99e3e268#file-mainwindow-axaml-cs
// ReSharper disable once CheckNamespace
namespace ShadUI;

internal static class OnScreenKeyboard
{
    private static readonly Dictionary<IInputPane, TopLevel> TopLevelMap = new();
    private static Timer? _throttleTimer;
    private static bool _alreadyDone;
    private static (TextBox? TextBox, bool State) _lastKeyboardEvent;
    private static readonly object LockObject = new();

    public static void Integrate()
    {
        if (_alreadyDone) return;

        _alreadyDone = true;

        Control.LoadedEvent.AddClassHandler<TopLevel>((s, _) =>
        {
            var input = s.InputPane;
            if (input == null) return;

            TopLevelMap[input] = s;
            input.StateChanged += InputPaneStateChanged;
        }, handledEventsToo: true);

        Control.UnloadedEvent.AddClassHandler<TopLevel>((s, _) =>
        {
            var input = s.InputPane;
            if (input == null) return;

            input.StateChanged -= InputPaneStateChanged;
            TopLevelMap.Remove(input);
        }, handledEventsToo: true);

        _throttleTimer = new Timer(HandleKeyboardEvent, null, Timeout.Infinite, Timeout.Infinite);

        InputElement.PointerPressedEvent.AddClassHandler<TextBox>((t, e) =>
        {
            if (e.Pointer.Type == PointerType.Touch) QueueKeyboardEvent(t, true);
        }, handledEventsToo: true);

        InputElement.LostFocusEvent.AddClassHandler<TextBox>((t, _) => QueueKeyboardEvent(t, false),
            handledEventsToo: true);
    }

    /// <summary>
    /// Cleans up static resources. Call this method during application shutdown.
    /// </summary>
    public static void Cleanup()
    {
        lock (LockObject)
        {
            if (_throttleTimer != null)
            {
                _throttleTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _throttleTimer.Dispose();
                _throttleTimer = null;
            }

            TopLevelMap.Clear();
            _alreadyDone = false;
        }
    }

    private static void QueueKeyboardEvent(TextBox textBox, bool state)
    {
        lock (LockObject)
        {
            _lastKeyboardEvent = (textBox, state);
            _throttleTimer?.Change(100, Timeout.Infinite);
        }
    }

    private static void HandleKeyboardEvent(object? state)
    {
        TextBox? textBox;
        bool eventState;

        lock (LockObject)
        {
            textBox = _lastKeyboardEvent.TextBox;
            eventState = _lastKeyboardEvent.State;
            _lastKeyboardEvent.TextBox = null;
        }

        if (textBox == null) return;

        var tl = TopLevel.GetTopLevel(textBox);
        if (tl == null) return;

        var hwnd = tl.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (hwnd == IntPtr.Zero) return;

        var input = tl.InputPane;
        if (input == null) return;

        if (eventState)
        {
            if (input.State == InputPaneState.Closed) Toggle(hwnd);
        }
        else
        {
            if (input.State == InputPaneState.Open) Toggle(hwnd);
        }
    }

    private static void InputPaneStateChanged(object? sender, InputPaneStateEventArgs e)
    {
        var inputPane = (IInputPane)sender!;
        var tl = TopLevelMap[inputPane];

        if (tl.FocusManager?.GetFocusedElement() is not TextBox ctrl)
        {
            return;
        }

        if (e.NewState == InputPaneState.Open)
        {
            // Get screen position of the bottom-left point of the TextBox
            var ctrlBottomScreen = tl.PointToScreen(ctrl.Bounds.BottomLeft);
            var ctrlBottom = ctrlBottomScreen.ToPoint(tl.RenderScaling);

            // Get the screen position of the top-left point of the TopLevel
            var tlTopCoords = tl.PointToScreen(tl.Bounds.TopLeft).ToPoint(tl.RenderScaling);

            var oskBounds = e.EndRect.Translate(tlTopCoords);

            var contains = oskBounds.Contains(ctrlBottom);
            if (!contains) return;

            var diff = oskBounds.TopLeft - ctrlBottom;
            tl.RenderTransform = new TranslateTransform(0, diff.Y);
        }
        else
        {
            if (tl.RenderTransform is not null) tl.RenderTransform = null;
        }
    }

    private static void Toggle(IntPtr hwnd)
    {
        UIHostNoLaunch uiHostNoLaunch;
        try
        {
            uiHostNoLaunch = new UIHostNoLaunch();
        }
        catch (COMException e)
        {
            if ((uint)e.HResult == 0x80040154)
            {
                // Use fully qualified path to prevent command injection via PATH hijacking
                var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                var tabTipPath = Path.Combine(programFiles, "Microsoft Shared", "ink", "tabtip.exe");

                if (File.Exists(tabTipPath))
                {
                    using var process = new Process();
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = tabTipPath,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    process.Start();
                }
            }
            else
            {
                throw;
            }

            return;
        }

        // ReSharper disable once SuspiciousTypeConversion.Global
        ((ITipInvocation)uiHostNoLaunch).Toggle(hwnd);
        Marshal.ReleaseComObject(uiHostNoLaunch);
    }

    [ComImport] [Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
    private class UIHostNoLaunch
    {
    }

    [ComImport] [Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ITipInvocation
    {
        void Toggle(IntPtr hwnd);
    }
}