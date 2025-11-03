using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Builds a toast notification.
/// </summary>
public sealed class ToastBuilder
{
    private readonly ToastManager _manager;

    /// <summary>
    ///     Returns a new instance of <see cref="ToastBuilder" />.
    /// </summary>
    /// <param name="manager">The toast manager</param>
    internal ToastBuilder(ToastManager manager)
    {
        _manager = manager;
    }

    private Toast? _toast;

    internal Notification Notification { get; set; }

    internal object? Content { get; set; }

    internal double Delay { get; set; } = 10;

    internal string ActionLabel { get; set; } = string.Empty;
    internal Action? Action { get; set; }

    internal bool DismissOnClick { get; set; }

    internal ToastPosition? Position { get; set; }

    internal bool ShowProgressBar { get; set; }
    internal bool IsProgressIndeterminate { get; set; }
    internal int ProgressValue { get; set; }

    internal ToastBuilder CreateToast(string title)
    {
        _toast = new Toast(_manager)
        {
            Title = title
        };
        return this;
    }

    internal void Show()
    {
        _toast ??= new Toast(_manager);

        _toast.Notification = Notification;
        _toast.Content = Content;
        _toast.Delay = Delay;
        _toast.ActionLabel = ActionLabel;
        _toast.CanDismissByClicking = DismissOnClick;
        _toast.Action = Action;
        _toast.Position = Position;
        _toast.ShowProgressBar = ShowProgressBar;
        _toast.IsProgressIndeterminate = IsProgressIndeterminate;
        _toast.ProgressValue = ProgressValue;
        _manager.Queue(_toast);
    }
}