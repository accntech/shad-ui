using System;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Fluent API for creating toast notifications.
/// </summary>
public static class ToastBuilderExtensions
{
    /// <summary>
    ///     Creates a new toast notification.
    /// </summary>
    /// <param name="manager">The toast manager</param>
    /// <param name="title">The title of the toast</param>
    /// <returns></returns>
    public static ToastBuilder CreateToast(this ToastManager manager, string title)
        => new ToastBuilder(manager).CreateToast(title);

    /// <summary>
    ///     Sets the content of the toast.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <param name="content">The content of the toast</param>
    /// <returns></returns>
    public static ToastBuilder WithContent(this ToastBuilder builder, object content)
    {
        builder.Content = content;
        return builder;
    }

    /// <summary>
    ///     Sets the delay before the toast is dismissed in seconds.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <param name="delay">The delay in seconds</param>
    /// <returns></returns>
    public static ToastBuilder WithDelay(this ToastBuilder builder, double delay)
    {
        builder.Delay = delay;
        return builder;
    }

    /// <summary>
    ///     Sets the action callback and label for the toast's action button.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <param name="label">The label of action button</param>
    /// <param name="action">The action callback, which triggers when action button is clicked</param>
    /// <returns></returns>
    public static ToastBuilder WithAction(this ToastBuilder builder, string label, Action action)
    {
        builder.ActionLabel = label;
        builder.Action = action;
        return builder;
    }

    /// <summary>
    ///     Enables display of the ProgressBar.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <param name="isIndeterminate">
    /// If <c>true</c>, the progress bar will show an indeterminate (animated) state.
    /// If <c>false</c>, the progress bar will reflect a specific value provided by <paramref name="progressValue"/>.
    /// </param>
    /// <param name="progressValue">
    /// The initial progress value (0–100) to display when <paramref name="isIndeterminate"/> is <c>false</c>.
    /// Ignored if <paramref name="isIndeterminate"/> is <c>true</c>.
    /// </param>
    /// <returns></returns>
    public static ToastBuilder WithProgressBar(this ToastBuilder builder, bool isIndeterminate = true, int progressValue = 0)
    {
        builder.ShowProgressBar = true;
        builder.IsProgressIndeterminate = isIndeterminate;
        builder.ProgressValue = progressValue;
        return builder;
    }

    /// <summary>
    ///     Sets the toast position to the top left.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <returns>
    ///     <see cref="ToastBuilder" />
    /// </returns>
    public static ToastBuilder OnTopLeft(this ToastBuilder builder)
    {
        builder.Position = ToastPosition.TopLeft;
        return builder;
    }

    /// <summary>
    ///     Sets the toast position to the top center.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <returns>
    ///     <see cref="ToastBuilder" />
    /// </returns>
    public static ToastBuilder OnTopCenter(this ToastBuilder builder)
    {
        builder.Position = ToastPosition.TopCenter;
        return builder;
    }

    /// <summary>
    ///     Sets the toast position to the top right.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <returns>
    ///     <see cref="ToastBuilder" />
    /// </returns>
    public static ToastBuilder OnTopRight(this ToastBuilder builder)
    {
        builder.Position = ToastPosition.TopRight;
        return builder;
    }

    /// <summary>
    ///     Sets the toast position to the bottom left.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <returns>
    ///     <see cref="ToastBuilder" />
    /// </returns>
    public static ToastBuilder OnBottomLeft(this ToastBuilder builder)
    {
        builder.Position = ToastPosition.BottomLeft;
        return builder;
    }

    /// <summary>
    ///     Sets the toast position to the bottom center.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <returns>
    ///     <see cref="ToastBuilder" />
    /// </returns>
    public static ToastBuilder OnBottomCenter(this ToastBuilder builder)
    {
        builder.Position = ToastPosition.BottomCenter;
        return builder;
    }

    /// <summary>
    ///     Sets the toast position to the bottom right.
    /// </summary>
    /// <param name="builder">The current <see cref="ToastBuilder" /></param>
    /// <returns>
    ///     <see cref="ToastBuilder" />
    /// </returns>
    public static ToastBuilder OnBottomRight(this ToastBuilder builder)
    {
        builder.Position = ToastPosition.BottomRight;
        return builder;
    }

    /// <summary>
    ///     Sets the toast to be dismissed when clicked.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ToastBuilder DismissOnClick(this ToastBuilder builder)
    {
        builder.DismissOnClick = true;
        return builder;
    }

    /// <summary>
    ///     Show basic toast notification
    /// </summary>
    /// <param name="builder"></param>
    public static void Show(this ToastBuilder builder)
    {
        builder.Notification = Notification.Basic;
        builder.Show();
    }

    /// <summary>
    ///     Shows a toast notification in the specified style.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="type">Notification type sent</param>
    public static void Show(this ToastBuilder builder, Notification type)
    {
        builder.Notification = type;
        builder.Show();
    }

    /// <summary>
    ///     Shows an info styled toast notification
    /// </summary>
    /// <param name="builder"></param>
    public static void ShowInfo(this ToastBuilder builder)
    {
        builder.Notification = Notification.Info;
        builder.Show();
    }

    /// <summary>
    ///     Shows a success styled toast notification
    /// </summary>
    /// <param name="builder"></param>
    public static void ShowSuccess(this ToastBuilder builder)
    {
        builder.Notification = Notification.Success;
        builder.Show();
    }

    /// <summary>
    ///     Shows a warning styled toast notification
    /// </summary>
    /// <param name="builder"></param>
    public static void ShowWarning(this ToastBuilder builder)
    {
        builder.Notification = Notification.Warning;
        builder.Show();
    }

    /// <summary>
    ///     Shows an error styled toast notification
    /// </summary>
    /// <param name="builder"></param>
    public static void ShowError(this ToastBuilder builder)
    {
        builder.Notification = Notification.Error;
        builder.Show();
    }
}