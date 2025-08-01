using Avalonia.Threading;

namespace ShadUI.Tests;

public abstract class BadgeTestBase
{
    protected BadgeTestBase()
    {
        AvaloniaTestFixture.EnsureInitialized();
    }

    protected T RunOnUIThread<T>(Func<T> action)
    {
        return AvaloniaTestFixture.RunOnUIThread(action);
    }

    protected void RunOnUIThread(Action action)
    {
        AvaloniaTestFixture.RunOnUIThread(action);
    }

    protected Badge CreateBadge()
    {
        return RunOnUIThread(() => new Badge());
    }
}