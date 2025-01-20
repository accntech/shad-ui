﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;

namespace ShadUI.Extensions;

/// <summary>
///     Useful extensions for any element.
/// </summary>
public class Element
{
    static Element()
    {
        ClassesProperty.Changed.AddClassHandler<AvaloniaObject>(HandleClassesChanged);
    }

    /// <summary>
    ///     Set the classes of the element using <see cref="string" />.
    /// </summary>
    public static readonly AttachedProperty<string> ClassesProperty =
        AvaloniaProperty.RegisterAttached<Element, StyledElement, string>(
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
    /// <param name="commandValue"></param>
    public static void SetClasses(AvaloniaObject element, Template commandValue)
    {
        element.SetValue(ClassesProperty, commandValue);
    }

    /// <summary>
    ///     Gets the classes of the element.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static string GetClasses(AvaloniaObject element) => element.GetValue(ClassesProperty);

    /// <summary>
    /// Gets whether the element should be focused when loaded.
    /// </summary>
    public static readonly AttachedProperty<bool> FocusOnLoadProperty =
        AvaloniaProperty.RegisterAttached<Element, Control, bool>(
            "FocusOnLoad", false, false, BindingMode.OneTime);

    /// <summary>
    /// Sets whether the element should be focused when loaded.
    /// </summary>
    public static void SetFocusOnLoad(AvaloniaObject element, bool value)
    {
        element.SetValue(FocusOnLoadProperty, value);
        if (element is Control control)
        {
            control.Loaded += (_, _) =>
            {
                if (GetFocusOnLoad(control)) control.Focus();
            };
        }
    }

    /// <summary>
    /// Gets whether the element should be focused when loaded.
    /// </summary>
    public static bool GetFocusOnLoad(AvaloniaObject element)
        => element.GetValue(FocusOnLoadProperty);
}