using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShadUI.Demo;

public sealed class PageManager(ServiceProvider serviceProvider) : IDisposable
{
    private readonly Dictionary<Type, INavigable> _pages = [];
    private Action<INavigable, string>? _onNavigate;
    private bool _disposed;

    public void Navigate<T>() where T : INavigable
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var attr = typeof(T).GetCustomAttribute<PageAttribute>();
        if (attr is null) throw new InvalidOperationException("Not a valid page type, missing PageAttribute");

        if (!_pages.TryGetValue(typeof(T), out var page))
        {
            try
            {
                page = serviceProvider.GetService<T>();
                if (page is null) throw new InvalidOperationException("Page not found");
                _pages.Add(typeof(T), page);
            }
            finally
            {
                CodeSnippetHelper.ClearCache();
            }
        }

        _onNavigate?.Invoke(page, attr.Route);
    }

    public void RegisterNavigationHandler(Action<INavigable, string> handler)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_onNavigate is not null)
        {
            throw new InvalidOperationException("A navigation handler is already registered");
        }

        _onNavigate = handler;
    }

    public void UnregisterNavigationHandler(Action<INavigable, string> handler)
    {
        if (_onNavigate == handler) _onNavigate = null;
    }

    public void Dispose()
    {
        if (_disposed) return;

        _onNavigate = null;
        foreach (var page in _pages.Values)
        {
            if (page is IDisposable disposable) disposable.Dispose();
        }

        _pages.Clear();
        _disposed = true;
    }
}

public interface INavigable
{
    void Initialize()
    {
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PageAttribute(string route) : Attribute
{
    public string Route { get; } = route;
}
