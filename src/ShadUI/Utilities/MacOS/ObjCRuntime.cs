using System;
using System.Runtime.InteropServices;

namespace ShadUI.Utilities.MacOS;

/// <summary>
/// Provides P/Invoke declarations for Objective-C runtime interop on macOS.
/// </summary>
internal static class ObjCRuntime
{
    private const string ObjCLibrary = "/usr/lib/libobjc.dylib";

    /// <summary>
    /// Creates a selector from a string name.
    /// </summary>
    [DllImport(ObjCLibrary, EntryPoint = "sel_registerName")]
    public static extern IntPtr GetSelector(string name);

    /// <summary>
    /// Sends a message to an object and returns an IntPtr.
    /// </summary>
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern IntPtr SendMessage(IntPtr receiver, IntPtr selector);

    /// <summary>
    /// Sends a message with an int parameter.
    /// </summary>
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern IntPtr SendMessage(IntPtr receiver, IntPtr selector, int arg1);

    /// <summary>
    /// Sends a message with a CGPoint parameter (for setFrameOrigin:).
    /// CGPoint is passed as two doubles on ARM64.
    /// </summary>
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void SendMessage(IntPtr receiver, IntPtr selector, double x, double y);

    /// <summary>
    /// Sends a message to get a CGRect (frame). On ARM64, returns via registers.
    /// </summary>
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern CGRect SendMessageCGRect(IntPtr receiver, IntPtr selector);

    /// <summary>
    /// Represents a CGRect structure for frame operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CGRect
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
    }

    private static IntPtr _standardWindowButtonSelector;
    private static IntPtr _frameSelector;
    private static IntPtr _setFrameOriginSelector;
    private static IntPtr _superviewSelector;

    public static IntPtr StandardWindowButtonSelector =>
        _standardWindowButtonSelector != IntPtr.Zero
            ? _standardWindowButtonSelector
            : (_standardWindowButtonSelector = GetSelector("standardWindowButton:"));

    public static IntPtr FrameSelector =>
        _frameSelector != IntPtr.Zero
            ? _frameSelector
            : (_frameSelector = GetSelector("frame"));

    public static IntPtr SetFrameOriginSelector =>
        _setFrameOriginSelector != IntPtr.Zero
            ? _setFrameOriginSelector
            : (_setFrameOriginSelector = GetSelector("setFrameOrigin:"));

    public static IntPtr SuperviewSelector =>
        _superviewSelector != IntPtr.Zero
            ? _superviewSelector
            : (_superviewSelector = GetSelector("superview"));
}
