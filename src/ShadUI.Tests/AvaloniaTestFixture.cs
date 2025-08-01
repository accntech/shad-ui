using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;

namespace ShadUI.Tests;

public class AvaloniaTestFixture
{
    private static bool _initialized = false;
    private static readonly object _lock = new object();

    public static void EnsureInitialized()
    {
        if (_initialized) return;

        lock (_lock)
        {
            if (_initialized) return;

            // Initialize Avalonia once for all tests
            if (Application.Current == null)
            {
                AppBuilder.Configure<Application>()
                    .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                    .SetupWithoutStarting();
            }

            _initialized = true;
        }
    }

    public static T RunOnUIThread<T>(Func<T> action)
    {
        EnsureInitialized();
        
        if (Dispatcher.UIThread.CheckAccess())
        {
            return action();
        }
        else
        {
            return Dispatcher.UIThread.Invoke(action);
        }
    }

    public static void RunOnUIThread(Action action)
    {
        EnsureInitialized();
        
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.UIThread.Invoke(action);
        }
    }
}