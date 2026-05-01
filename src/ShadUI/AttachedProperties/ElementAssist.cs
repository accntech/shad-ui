using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Useful extensions for any element.
/// </summary>
public class ElementAssist
{
    static ElementAssist()
    {
        ClassesProperty.Changed.AddClassHandler<AvaloniaObject>(HandleClassesChanged);
        FocusOnLoadProperty.Changed.AddClassHandler<Control>(HandleFocusOnLoadChanged);
        AttachFocusProperty.Changed.AddClassHandler<Control>(HandleAttachFocusChanged);
    }

    /// <summary>
    ///     Set the classes of the element using <see cref="string" />.
    /// </summary>
    public static readonly AttachedProperty<string> ClassesProperty =
        AvaloniaProperty.RegisterAttached<ElementAssist, StyledElement, string>(
            "Classes", "", false, BindingMode.OneTime);

    private static void HandleClassesChanged(AvaloniaObject element, AvaloniaPropertyChangedEventArgs args)
    {
        if (element is not StyledElement styled) return;

        var classes = args.NewValue as string;

        styled.Classes.Clear();
        styled.Classes.AddRange(Classes.Parse(classes ?? ""));
    }

    /// <summary>
    ///     Sets the classes of the element.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetClasses(AvaloniaObject element, string value)
    {
        element.SetValue(ClassesProperty, value);
    }

    /// <summary>
    ///     Gets the classes of the element.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static string GetClasses(AvaloniaObject element)
    {
        return element.GetValue(ClassesProperty);
    }

    /// <summary>
    ///     Gets whether the element should be focused when loaded.
    /// </summary>
    public static readonly AttachedProperty<bool> FocusOnLoadProperty =
        AvaloniaProperty.RegisterAttached<ElementAssist, Control, bool>(
            "FocusOnLoad", false, false, BindingMode.OneTime);

    /// <summary>
    ///     Sets whether the element should be focused when loaded.
    /// </summary>
    public static void SetFocusOnLoad(AvaloniaObject element, bool value)
    {
        element.SetValue(FocusOnLoadProperty, value);
    }

    /// <summary>
    ///     Gets whether the element should be focused when loaded.
    /// </summary>
    public static bool GetFocusOnLoad(AvaloniaObject element)
    {
        return element.GetValue(FocusOnLoadProperty);
    }

    private static void HandleFocusOnLoadChanged(AvaloniaObject element, AvaloniaPropertyChangedEventArgs args)
    {
        if (element is not Control control) return;

        control.Loaded += (_, _) =>
        {
            if (!GetFocusOnLoad(control)) return;

            control.Focus();
            if (control is TextBox textBox) textBox.CaretIndex = textBox.Text?.Length ?? 0;
        };
    }

    /// <summary>
    ///     Attached property that allows a parent element to track focus state of a child element.
    ///     When the specified child element gains or loses focus, the parent element's Tag property
    ///     will be set to "active" or "inactive" respectively.
    /// </summary>
    public static readonly AttachedProperty<string> AttachFocusProperty =
        AvaloniaProperty.RegisterAttached<ElementAssist, Control, string>("AttachFocus");

    /// <summary>
    ///     Sets the name of the child element whose focus state should be tracked by the parent element.
    /// </summary>
    /// <param name="obj">The parent control that will track the focus state.</param>
    /// <param name="value">The name of the child element to monitor for focus changes.</param>
    public static void SetAttachFocus(Control obj, string value)
    {
        obj.SetValue(AttachFocusProperty, value);
    }

    /// <summary>
    ///     Gets the name of the child element whose focus state is being tracked by the parent element.
    /// </summary>
    /// <param name="obj">The parent control that is tracking the focus state.</param>
    /// <returns>The name of the child element being monitored, or null if no element is being tracked.</returns>
    public static string GetAttachFocus(Control obj)
    {
        return obj.GetValue(AttachFocusProperty);
    }

    private static void HandleAttachFocusChanged(Control element, AvaloniaPropertyChangedEventArgs e)
    {
        var childName = e.NewValue as string;

        CleanupFocusHandlers(element);

        if (string.IsNullOrEmpty(childName)) return;

        var child = element.FindControl<Control>(childName);

        if (child == null && element is TemplatedControl templatedControl)
        {
            void OnTemplateApplied(object? sender, TemplateAppliedEventArgs args)
            {
                templatedControl.TemplateApplied -= OnTemplateApplied;

                if (args.NameScope.Find(childName) is not Control templateChild) return;

                SetupFocusHandlers(element, templateChild);
            }

            templatedControl.TemplateApplied += OnTemplateApplied;
        }
        else if (child != null)
        {
            SetupFocusHandlers(element, child);
        }
    }

    private static void SetupFocusHandlers(Control parent, Control child)
    {
        parent.SetValue(FocusHandlersProperty, new FocusHandlers
        {
            Child = child,
            GotFocusHandler = HandleGotFocus,
            LostFocusHandler = HandleLostFocus
        });

        child.GotFocus += HandleGotFocus;
        child.LostFocus += HandleLostFocus;

        if (child.IsFocused) parent.Tag = "active";

        return;

        void HandleLostFocus(object? sender, RoutedEventArgs args)
        {
            parent.Tag = "inactive";
        }

        void HandleGotFocus(object? sender, FocusChangedEventArgs args)
        {
            parent.Tag = "active";
        }
    }

    private static void CleanupFocusHandlers(Control? element)
    {
        var handlers = element?.GetValue(FocusHandlersProperty);

        if (handlers is null) return;

        if (handlers.Child is not null)
        {
            handlers.Child.GotFocus -= handlers.GotFocusHandler;
            handlers.Child.LostFocus -= handlers.LostFocusHandler;
        }

        element?.ClearValue(FocusHandlersProperty);
    }

    // Helper class to store event handlers for cleanup
    private class FocusHandlers
    {
        public Control? Child { get; set; }
        public EventHandler<FocusChangedEventArgs>? GotFocusHandler { get; set; }
        public EventHandler<RoutedEventArgs>? LostFocusHandler { get; set; }
    }

    // Private attached property to store event handlers
    private static readonly AttachedProperty<FocusHandlers> FocusHandlersProperty =
        AvaloniaProperty.RegisterAttached<ElementAssist, Control, FocusHandlers>("FocusHandlers");
}