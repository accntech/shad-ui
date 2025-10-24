using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Dialog service for showing dialogs.
/// </summary>
public sealed class DialogManager
{
    internal event EventHandler<DialogShownEventArgs>? OnDialogShown;
    internal event EventHandler<DialogClosedEventArgs>? OnDialogClosed;

    internal readonly Dictionary<Control, DialogOptions> Dialogs = [];

    /// <summary>
    ///     Shows a dialog with the provided options.
    /// </summary>
    /// <param name="control">Control to be shown as dialog</param>
    /// <param name="options">Dialog options</param>
    internal void Show(Control control, DialogOptions options)
    {
        if (Dialogs.Count > 0)
        {
            if (control is SimpleDialog simple)
            {
                var existingSimpleDialog = Dialogs.FirstOrDefault(x => x.Key is SimpleDialog d && d.Id == simple.Id)
                    .Key;

                if (existingSimpleDialog is not null) return;
            }

            var existingCustomDialog =
                Dialogs.FirstOrDefault(x =>
                    x.Key.DataContext?.GetType() == control.DataContext?.GetType()).Key;
            if (existingCustomDialog is not null) return;

            var last = Dialogs.Last();
            if (last.Key != control)
            {
                OnDialogClosed?.Invoke(this, new DialogClosedEventArgs { ReplaceExisting = true, Control = last.Key });
            }
        }

        Dialogs.TryAdd(control, options);
        OnDialogShown?.Invoke(this, new DialogShownEventArgs { Control = control, Options = options });
    }

    internal void CloseDialog(Control control)
    {
        var context = control.DataContext;
        if (context is not null)
        {
            var contextType = context.GetType();
            OnSuccessCallbacks.Remove(contextType);
            OnSuccessWithContextCallbacks.Remove(contextType);
            OnSuccessAsyncCallbacks.Remove(contextType);
            OnSuccessWithContextAsyncCallbacks.Remove(contextType);
            OnCancelCallbacks.Remove(contextType);
            OnCancelAsyncCallbacks.Remove(contextType);
        }

        Dialogs.Remove(control);

        OnDialogClosed?.Invoke(this, new DialogClosedEventArgs
        {
            ReplaceExisting = Dialogs.Count > 0,
            Control = control
        }
        );

        if (control is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    internal void OpenLast()
    {
        if (Dialogs.Count == 0) return;

        var lastDialog = Dialogs.Last();
        OnDialogShown?.Invoke(this, new DialogShownEventArgs { Control = lastDialog.Key, Options = lastDialog.Value });
    }

    internal void RemoveLast()
    {
        if (Dialogs.Count == 0) return;

        var lastDialog = Dialogs.Last();
        CloseDialog(lastDialog.Key);

        var context = lastDialog.Key.DataContext;
        var contextType = context?.GetType();
        if (contextType is not null) InvokeCallBacks(context, contextType, false);
    }

    internal readonly Dictionary<Type, Type> CustomDialogs = [];

    /// <summary>
    ///     Registers a custom dialog view with its DataContext type.
    /// </summary>
    /// <typeparam name="TView">The type of view</typeparam>
    /// <typeparam name="TContext">The type of DataContext</typeparam>
    /// <returns></returns>
    public DialogManager Register<TView, TContext>() where TView : Control
    {
        CustomDialogs.Add(typeof(TContext), typeof(TView));
        return this;
    }

    internal readonly Dictionary<Type, Action> OnCancelCallbacks = [];
    internal readonly Dictionary<Type, Func<Task>> OnCancelAsyncCallbacks = [];
    internal readonly Dictionary<Type, Action> OnSuccessCallbacks = [];
    internal readonly Dictionary<Type, Action<object>> OnSuccessWithContextCallbacks = [];
    internal readonly Dictionary<Type, Func<Task>> OnSuccessAsyncCallbacks = [];
    internal readonly Dictionary<Type, Func<object, Task>> OnSuccessWithContextAsyncCallbacks = [];

    private void InvokeCallBacks(object? context, Type type, bool success)
    {
        if (OnSuccessCallbacks.Remove(type, out var successCallback) && success)
        {
            successCallback?.Invoke();
        }

        if (OnSuccessWithContextCallbacks.Remove(type, out var successWithContextCallback) && success)
        {
            if(context is not null) successWithContextCallback?.Invoke(context);
        }

        if (OnSuccessAsyncCallbacks.Remove(type, out var successAsyncCallback) && success)
        {
            successAsyncCallback?.Invoke();
        }

        if (OnSuccessWithContextAsyncCallbacks.Remove(type, out var successWithContextAsyncCallback) && success)
        {
            if(context is not null) successWithContextAsyncCallback?.Invoke(context);
        }

        if (OnCancelCallbacks.Remove(type, out var cancelCallback) && !success)
        {
            cancelCallback?.Invoke();
        }

        if (OnCancelAsyncCallbacks.Remove(type, out var cancelAsyncCallback) && !success)
        {
            cancelAsyncCallback?.Invoke();
        }
    }

    /// <summary>
    ///     Closes the dialog associated with the specified context and invokes the appropriate callbacks.
    /// </summary>
    /// <typeparam name="TContext">The type of the DataContext associated with the dialog.</typeparam>
    /// <param name="context">The DataContext of the dialog to close.</param>
    /// <param name="options">Optional parameters for closing the dialog.</param>
    public void Close<TContext>(TContext context, CloseDialogOptions? options = null)
    {
        var clearAll = options?.ClearAll ?? false;
        var dialogs = Dialogs.Where(x => Equals(x.Key.DataContext, context)).ToList();

        if (clearAll) RemoveAll();

        foreach (var dialog in dialogs) CloseDialog(dialog.Key);

        var success = options?.Success ?? false;
        InvokeCallBacks(context, typeof(TContext), success);

        if (!clearAll) OpenLast();
    }

    private void RemoveAll()
    {
        Dialogs.Clear();
        OnSuccessAsyncCallbacks.Clear();
        OnSuccessCallbacks.Clear();
        OnSuccessWithContextCallbacks.Clear();
        OnSuccessWithContextAsyncCallbacks.Clear();
        OnCancelAsyncCallbacks.Clear();
        OnCancelCallbacks.Clear();
    }

    /// <summary>
    ///     Clears all dialogs and callbacks. Use this to prevent memory leaks when closing windows.
    /// </summary>
    public void Dispose()
    {
        // Close all dialogs
        var dialogs = Dialogs.Keys.ToList();
        foreach (var dialog in dialogs)
        {
            CloseDialog(dialog);
        }

        RemoveAll();
    }

    internal event EventHandler<bool>? AllowDismissChanged;

    /// <summary>
    ///     Disables the ability to dismiss dialogs. This overrides the <see cref="DialogHost.Dismissible" /> property of the
    ///     <see cref="DialogHost" />.
    /// </summary>
    public void PreventDismissal()
    {
        AllowDismissChanged?.Invoke(this, false);
    }

    /// <summary>
    ///     Enables the ability to dismiss dialogs. This overrides the <see cref="DialogHost.Dismissible" /> property of the
    ///     <see cref="DialogHost" />.
    /// </summary>
    public void AllowDismissal()
    {
        AllowDismissChanged?.Invoke(this, true);
    }
}

internal sealed class DialogShownEventArgs : EventArgs
{
    public Control Control { get; set; } = null!;
    public DialogOptions Options { get; set; } = null!;
}

internal sealed class DialogClosedEventArgs : EventArgs
{
    public bool ReplaceExisting { get; set; }
    public Control Control { get; set; } = null!;
}