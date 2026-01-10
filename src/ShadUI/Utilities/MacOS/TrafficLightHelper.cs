using System;
using System.Runtime.InteropServices;
using Avalonia;

namespace ShadUI.Utilities.MacOS;

/// <summary>
/// Helper class for repositioning macOS traffic light buttons (close, minimize, zoom).
/// </summary>
internal static class TrafficLightHelper
{
    private enum NSWindowButton
    {
        Close = 0,
        Miniaturize = 1,
        Zoom = 2
    }

    // Default macOS traffic light positions (distance from edges)
    private const double DefaultLeftMargin = 7.0;
    private const double DefaultTopMargin = 3.0;
    private const double ButtonSpacing = 20.0;

    /// <summary>
    /// Repositions the traffic light buttons with the specified offset from default positions.
    /// </summary>
    /// <param name="nsWindowHandle">The native NSWindow handle.</param>
    /// <param name="offset">The offset (X moves right, Y moves down).</param>
    public static void SetTrafficLightOffset(IntPtr nsWindowHandle, Point offset)
    {
        if (nsWindowHandle == IntPtr.Zero) return;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;

        try
        {
            RepositionButton(nsWindowHandle, NSWindowButton.Close, 0, offset);
            RepositionButton(nsWindowHandle, NSWindowButton.Miniaturize, 1, offset);
            RepositionButton(nsWindowHandle, NSWindowButton.Zoom, 2, offset);
        }
        catch
        {
            // ignore
        }
    }

    private static void RepositionButton(IntPtr nsWindowHandle, NSWindowButton buttonType, int buttonIndex, Point offset)
    {
        var button = ObjCRuntime.SendMessage(
            nsWindowHandle,
            ObjCRuntime.StandardWindowButtonSelector,
            (int)buttonType);

        if (button == IntPtr.Zero) return;

        var superview = ObjCRuntime.SendMessage(button, ObjCRuntime.SuperviewSelector);
        if (superview == IntPtr.Zero) return;

        var superviewFrame = ObjCRuntime.SendMessageCGRect(superview, ObjCRuntime.FrameSelector);
        var buttonFrame = ObjCRuntime.SendMessageCGRect(button, ObjCRuntime.FrameSelector);
        var defaultX = DefaultLeftMargin + (buttonIndex * ButtonSpacing);
        var defaultY = superviewFrame.Height - DefaultTopMargin - buttonFrame.Height;

        var newX = defaultX + offset.X;
        var newY = defaultY - offset.Y;

        ObjCRuntime.SendMessage(button, ObjCRuntime.SetFrameOriginSelector, newX, newY);
    }
}
