using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShadUI
{
    /// <summary>
    /// ListBoxItemAssist 
    /// </summary>
    public static class ListBoxItemAssist
    {
        /// <summary>
        /// Gets or sets control display check icon.
        /// </summary>
        public static readonly AttachedProperty<bool> ShowCheckIconProperty =
       AvaloniaProperty.RegisterAttached<ListBoxItem, bool>(
           "ShowCheckIcon", typeof(ListBoxItemAssist),true);
        /// <summary>
        ///  Gets the value of <see cref="ListBoxItemAssist" />
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetShowCheckIcon(AvaloniaObject element, bool value) =>
            element.SetValue(ShowCheckIconProperty, value);
        /// <summary>
        /// Gets the value of <see cref="ListBoxItemAssist" />
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetShowCheckIcon(AvaloniaObject element) =>
            element.GetValue(ShowCheckIconProperty);
    }
}
