using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Manages the toast notification.
/// </summary>
public sealed class ToastManager
{
    internal event EventHandler<Toast>? OnToastQueued;
    internal event EventHandler<Toast>? OnToastDismissed;
    internal event EventHandler? OnAllToastsDismissed;

    private readonly List<Toast> _toasts = [];

    internal IReadOnlyList<Toast> Toasts => _toasts;

    internal void Queue(Toast toast)
    {
        _toasts.Add(toast);
        OnToastQueued?.Invoke(this, toast);
    }

    internal void Dismiss(Toast toast)
    {
        if (!_toasts.Contains(toast)) return;
        OnToastDismissed?.Invoke(this, toast);
        _toasts.Remove(toast);
    }

    private void Dismiss(int count)
    {
        if (!_toasts.Any()) return;
        if (count > _toasts.Count) count = _toasts.Count;
        for (var i = 0; i < count; i++)
        {
            var removed = _toasts[0];
            OnToastDismissed?.Invoke(this, removed);
            _toasts.RemoveAt(0);
        }
    }

    internal void EnsureMaximum(int maxAllowed)
    {
        if (_toasts.Count <= maxAllowed) return;
        Dismiss(_toasts.Count - maxAllowed);
    }

    /// <summary>
    ///     Dismiss all toasts.
    /// </summary>
    public void DismissAll()
    {
        if (!_toasts.Any()) return;
        OnAllToastsDismissed?.Invoke(this, EventArgs.Empty);
        _toasts.Clear();
    }

    internal bool IsDismissed(Toast toast) => !_toasts.Contains(toast);
}
