using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     A modern window with a customizable title bar.
/// </summary>
[TemplatePart("PART_Root", typeof(Panel))]
[TemplatePart("PART_TitleBarBackground", typeof(Control))]
[TemplatePart("PART_MaximizeButton", typeof(Button))]
[TemplatePart("PART_MinimizeButton", typeof(Button))]
[TemplatePart("PART_CloseButton", typeof(Button))]
public class Window : Avalonia.Controls.Window
{
    /// <summary>
    ///     The style key of the window.
    /// </summary>
    protected override Type StyleKeyOverride => typeof(Window);

    /// <summary>
    ///     The font size of the title.
    /// </summary>
    public static readonly StyledProperty<double> TitleFontSizeProperty =
        AvaloniaProperty.Register<Window, double>(nameof(TitleFontSize), 14);

    /// <summary>
    ///     Gets or sets the value of the <see cref="TitleFontSizeProperty" />.
    /// </summary>
    public double TitleFontSize
    {
        get => GetValue(TitleFontSizeProperty);
        set => SetValue(TitleFontSizeProperty, value);
    }

    /// <summary>
    ///     The font weight of the title.
    /// </summary>
    public static readonly StyledProperty<FontWeight> TitleFontWeightProperty =
        AvaloniaProperty.Register<Window, FontWeight>(nameof(TitleFontWeight), FontWeight.Medium);

    /// <summary>
    ///     Gets or sets the value of the <see cref="TitleFontWeightProperty" />.
    /// </summary>
    public FontWeight TitleFontWeight
    {
        get => GetValue(TitleFontWeightProperty);
        set => SetValue(TitleFontWeightProperty, value);
    }

    /// <summary>
    ///     The content of the logo.
    /// </summary>
    public static readonly StyledProperty<Control?> LogoContentProperty =
        AvaloniaProperty.Register<Window, Control?>(nameof(LogoContent));

    /// <summary>
    ///     Gets or sets the value of the <see cref="LogoContentProperty" />.
    /// </summary>
    public Control? LogoContent
    {
        get => GetValue(LogoContentProperty);
        set => SetValue(LogoContentProperty, value);
    }

    /// <summary>
    ///     Whether to show the bottom border.
    /// </summary>
    public static readonly StyledProperty<bool> ShowBottomBorderProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(ShowBottomBorder), true);

    /// <summary>
    ///     Gets or sets the value of the <see cref="ShowBottomBorderProperty" />.
    /// </summary>
    public bool ShowBottomBorder
    {
        get => GetValue(ShowBottomBorderProperty);
        set => SetValue(ShowBottomBorderProperty, value);
    }

    /// <summary>
    ///     Whether to show the title bar.
    /// </summary>
    public static readonly StyledProperty<bool> IsTitleBarVisibleProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsTitleBarVisible), true);

    /// <summary>
    ///     Gets or sets the value of the <see cref="IsTitleBarVisibleProperty" />.
    /// </summary>
    public bool IsTitleBarVisible
    {
        get => GetValue(IsTitleBarVisibleProperty);
        set => SetValue(IsTitleBarVisibleProperty, value);
    }

    /// <summary>
    ///     The corner radius of the window.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> RootCornerRadiusProperty =
        AvaloniaProperty.Register<Border, CornerRadius>(nameof(RootCornerRadius));

    /// <summary>
    ///     Gets or sets the value of <see cref="RootCornerRadiusProperty" />.
    /// </summary>
    public CornerRadius RootCornerRadius
    {
        get => GetValue(RootCornerRadiusProperty);
        set => SetValue(RootCornerRadiusProperty, value);
    }

    /// <summary>
    ///     Whether to enable title bar animation.
    /// </summary>
    public static readonly StyledProperty<bool> TitleBarAnimationEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(TitleBarAnimationEnabled), true);

    /// <summary>
    ///     Gets or sets the value of the <see cref="TitleBarAnimationEnabledProperty" />.
    /// </summary>
    public bool TitleBarAnimationEnabled
    {
        get => GetValue(TitleBarAnimationEnabledProperty);
        set => SetValue(TitleBarAnimationEnabledProperty, value);
    }

    /// <summary>
    ///     Whether to show the menu.
    /// </summary>
    public static readonly StyledProperty<bool> IsMenuVisibleProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsMenuVisible));

    /// <summary>
    ///     Gets or sets the value of the <see cref="IsMenuVisibleProperty" />.
    /// </summary>
    public bool IsMenuVisible
    {
        get => GetValue(IsMenuVisibleProperty);
        set => SetValue(IsMenuVisibleProperty, value);
    }

    /// <summary>
    ///     The menu items.
    /// </summary>
    public static readonly StyledProperty<object?> MenuBarContentProperty =
        AvaloniaProperty.Register<Window, object?>(nameof(MenuBarContent));

    /// <summary>
    ///     Gets or sets the value of the <see cref="MenuBarContentProperty" />.
    /// </summary>
    public object? MenuBarContent
    {
        get => GetValue(MenuBarContentProperty);
        set => SetValue(MenuBarContentProperty, value);
    }

    /// <summary>
    ///     Whether to show the title bar background.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTitlebarBackgroundProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(ShowTitlebarBackground), true);

    /// <summary>
    ///     Gets or sets the value of the <see cref="ShowTitlebarBackgroundProperty" />.
    /// </summary>
    public bool ShowTitlebarBackground
    {
        get => GetValue(ShowTitlebarBackgroundProperty);
        set => SetValue(ShowTitlebarBackgroundProperty, value);
    }

    /// <summary>
    ///     Whether to enable move.
    /// </summary>
    public static readonly StyledProperty<bool> CanMoveProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(CanMove), true);

    /// <summary>
    ///     Gets or sets the value of the <see cref="CanMoveProperty" />.
    /// </summary>
    public bool CanMove
    {
        get => GetValue(CanMoveProperty);
        set => SetValue(CanMoveProperty, value);
    }

    /// <summary>
    ///     The controls on the right side of the title bar.
    /// </summary>
    public static readonly StyledProperty<object?> RightWindowTitleBarContentProperty =
        AvaloniaProperty.Register<Window, object?>(nameof(RightWindowTitleBarContent));

    /// <summary>
    ///     Gets or sets the value of the <see cref="RightWindowTitleBarContentProperty" />.
    /// </summary>
    public object? RightWindowTitleBarContent
    {
        get => GetValue(RightWindowTitleBarContentProperty);
        set => SetValue(RightWindowTitleBarContentProperty, value);
    }

    /// <summary>
    ///     These controls are displayed above all others and fill the entire window.
    ///     Useful for things like popups.
    /// </summary>
    public static readonly StyledProperty<Controls> HostsProperty =
        AvaloniaProperty.Register<Window, Controls>(nameof(Hosts), []);

    /// <summary>
    ///     These controls are displayed above all others and fill the entire window.
    /// </summary>
    public Controls Hosts
    {
        get => GetValue(HostsProperty);
        set => SetValue(HostsProperty, value);
    }

    /// <summary>
    ///     Whether to save and restore the window state (position, size, etc.) between application sessions.
    /// </summary>
    public static readonly StyledProperty<bool> SaveWindowStateProperty = AvaloniaProperty.Register<Window, bool>(
        nameof(SaveWindowState));

    /// <summary>
    ///     Gets or sets the value of the <see cref="SaveWindowStateProperty" />.
    /// </summary>
    public bool SaveWindowState
    {
        get => GetValue(SaveWindowStateProperty);
        set => SetValue(SaveWindowStateProperty, value);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Window" /> class.
    /// </summary>
    protected Window()
    {
        Hosts = [];
    }

    /// <summary>
    ///     Called when the window is loaded.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        if (desktop.MainWindow is Window window && window != this) Icon ??= window.Icon;
    }

    private WindowState _lastState = WindowState.Normal;

    /// <summary>
    ///     Called when a property is changed.
    /// </summary>
    /// <param name="change">The event arguments.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == WindowStateProperty &&
            change is { OldValue: WindowState oldState, NewValue: WindowState newState })
        {
            _lastState = oldState;
            OnWindowStateChanged(newState);
        }

        if (change.Property == SaveWindowStateProperty)
        {
            var saveState = change.GetNewValue<bool>();
            if (saveState)
            {
                var assembly = Assembly.GetEntryAssembly();
                this.ManageWindowState(assembly?.GetName().Name ?? "main");
            }
            else
            {
                this.UnmanageWindowState();
            }
        }
    }

    private Button? _maximizeButton;
    private CornerRadius _lastCornerRadius;

    /// <summary>
    ///     Called when the template is applied.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        OnWindowStateChanged(WindowState);

        if (e.NameScope.Get<Button>("PART_MaximizeButton") is { } maximize)
        {
            _maximizeButton = maximize;
            _maximizeButton.Click += OnMaximizeButtonClicked;
            EnableWindowsSnapLayout(maximize);
        }

        if (e.NameScope.Get<Button>("PART_MinimizeButton") is { } minimize)
        {
            minimize.Click += (_, _) => WindowState = WindowState.Minimized;
        }

        if (e.NameScope.Get<Button>("PART_CloseButton") is { } close)
        {
            close.Click += (_, _) => Close();
        }

        if (e.NameScope.Get<Control>("PART_TitleBarBackground") is { } titleBar)
        {
            titleBar.PointerPressed += OnTitleBarPointerPressed;
            titleBar.DoubleTapped += OnMaximizeButtonClicked;
        }
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (e.NameScope.Get<Panel>("PART_Root") is { } rootPanel)
            {
                this.AddResizeGrip(rootPanel);
            }
        
            if (RootCornerRadius == default)
            {
                RootCornerRadius = new CornerRadius(10);
            }
        }

        _lastCornerRadius = RootCornerRadius;
    }

    private void OnMaximizeButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (!CanMaximize || !CanResize || WindowState == WindowState.FullScreen) return;
        
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    internal bool HasOpenDialog { get; set; }

    private bool _snapLayoutEnabled = true;

    private void EnableWindowsSnapLayout(Button maximize)
    {
        var pointerOnMaxButton = false;
        var setter = typeof(Button).GetProperty("IsPointerOver");
        var proc = (IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
        {
            if (!_snapLayoutEnabled) return IntPtr.Zero;

            switch (msg)
            {
                case 533:
                    if (!pointerOnMaxButton) break;
                    WindowState = WindowState == WindowState.Maximized
                        ? WindowState.Normal
                        : WindowState.Maximized;
                    break;
                case 0x0084:
                    var point = new PixelPoint(
                        (short)(ToInt32(lParam) & 0xffff),
                        (short)(ToInt32(lParam) >> 16));
                    var size = maximize.Bounds;
                    var buttonLeftTop = maximize.PointToScreen(FlowDirection == FlowDirection.LeftToRight
                        ? new Point(size.Width, 0)
                        : new Point(0, 0));
                    var x = (buttonLeftTop.X - point.X) / RenderScaling;
                    var y = (point.Y - buttonLeftTop.Y) / RenderScaling;
                    if (new Rect(0, 0,
                            size.Width,
                            size.Height)
                        .Contains(new Point(x, y)))
                    {
                        if (HasOpenDialog) return (IntPtr)9;

                        setter?.SetValue(maximize, true);
                        pointerOnMaxButton = true;
                        handled = true;
                        return (IntPtr)9;
                    }
                    pointerOnMaxButton = false;
                    setter?.SetValue(maximize, false);
                    break;
            }

            return IntPtr.Zero;

            static int ToInt32(IntPtr ptr)
            {
                return IntPtr.Size == 4
                    ? ptr.ToInt32()
                    : (int)(ptr.ToInt64() & 0xffffffff);
            }
        };

        Win32Properties.AddWndProcHookCallback(this, new Win32Properties.CustomWndProcHookCallback(proc));
    }

    private void OnWindowStateChanged(WindowState state)
    {
        _snapLayoutEnabled = WindowState != WindowState.FullScreen && CanMaximize && CanResize;
        switch (state)
        {
            case WindowState.FullScreen:
                ToggleMaxButtonVisibility(false);
                _lastCornerRadius = RootCornerRadius;
                RootCornerRadius = new CornerRadius(0);
                Margin = new Thickness(-1);
                break;
            case WindowState.Maximized:
                ToggleMaxButtonVisibility(CanMaximize);
                RootCornerRadius = _lastCornerRadius;
                Margin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? new Thickness(7)
                    : new Thickness(0);
                break;
            case WindowState.Normal:
                ToggleMaxButtonVisibility(CanMaximize);
                RootCornerRadius = _lastCornerRadius;
                Margin = new Thickness(0);
                break;
            default:
                Margin = new Thickness(0);
                break;
        }
    }

    private void ToggleMaxButtonVisibility(bool visible)
    {
        if (_maximizeButton is null) return;

        _maximizeButton.IsVisible = visible;
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (CanMove && WindowState != WindowState.FullScreen) BeginMoveDrag(e);
    }

    /// <summary>
    ///     Exits full screen mode and restores the previous window state.
    /// </summary>
    protected void ExitFullScreen()
    {
        if (WindowState == WindowState.FullScreen) WindowState = _lastState;
    }

    /// <summary>
    ///     Restores the last window state.
    /// </summary>
    public void RestoreWindowState()
    {
        WindowState = _lastState == WindowState.FullScreen ? WindowState.Maximized : _lastState;
    }

    static Window()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) OnScreenKeyboard.Integrate();
    }
}