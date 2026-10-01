using System;
using System.Threading.Tasks;
using Avalonia.Controls;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Builds a dialog.
/// </summary>
/// <typeparam name="TContext">The DataContext of a control</typeparam>
public sealed class DialogBuilder<TContext>
{
    private readonly DialogManager _manager;

    internal DialogBuilder(DialogManager manager)
    {
        _manager = manager;
    }

    internal Action? OnCancelCallback { get; set; }
    internal Func<Task>? OnCancelAsyncCallback { get; set; }
    internal Action? OnSuccessCallback { get; set; }
    internal Action<object>? OnSuccessWithContextCallback { get; set; }
    internal Func<Task>? OnSuccessAsyncCallback { get; set; }
    internal Func<object, Task>? OnSuccessWithContextAsyncCallback { get; set; }
    internal DialogOptions Options { get; } = new();

    private Control? _control;

    internal DialogBuilder<TContext> CreateDialog(TContext context)
    {
        if (!_manager.CustomDialogs.TryGetValue(typeof(TContext), out var factory))
        {
            throw new InvalidOperationException($"Custom dialog with {typeof(TContext)} is not registered.");
        }

        _control = factory();
        _control.DataContext = context;
        return this;
    }

    internal void Show()
    {
        if (_control == null) throw new InvalidOperationException("Dialog control is not set.");

        if (OnSuccessCallback != null)
        {
            _manager.OnSuccessCallbacks.TryAdd(typeof(TContext), OnSuccessCallback);
        }

        if (OnSuccessWithContextCallback != null)
        {
            _manager.OnSuccessWithContextCallbacks.TryAdd(typeof(TContext), OnSuccessWithContextCallback);
        }

        if (OnSuccessAsyncCallback != null)
        {
            _manager.OnSuccessAsyncCallbacks.TryAdd(typeof(TContext), OnSuccessAsyncCallback);
        }

        if (OnSuccessWithContextAsyncCallback != null)
        {
            _manager.OnSuccessWithContextAsyncCallbacks.TryAdd(typeof(TContext), OnSuccessWithContextAsyncCallback);
        }

        if (OnCancelCallback != null)
        {
            _manager.OnCancelCallbacks.TryAdd(typeof(TContext), OnCancelCallback);
        }

        if (OnCancelAsyncCallback != null)
        {
            _manager.OnCancelAsyncCallbacks.TryAdd(typeof(TContext), OnCancelAsyncCallback);
        }

        _manager.Show(_control, Options);
    }
}