using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Reactive;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShadUI
{
    /// <summary>
    /// 
    /// </summary>
    public class PopupMask : TemplatedControl
    {
        ///     These controls are displayed above all others and fill the entire window.
        ///     Useful for things like popups.
        /// </summary>
        public static readonly StyledProperty<Controls> HostsProperty =
            AvaloniaProperty.Register<PopupMask, Controls>(nameof(Hosts), []);

        /// <summary>
        ///     These controls are displayed above all others and fill the entire window.
        /// </summary>
        public Controls Hosts
        {
            get => GetValue(HostsProperty);
            set => SetValue(HostsProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
        }
        /// <summary>
        /// 
        /// </summary>
        public PopupMask()
        {
            Hosts = [];
        }
    }
}
