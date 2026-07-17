using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;

namespace ShadUI.Tests.Controls;

public class ManagerSubscriptionLifecycleTests
{
    [Fact]
    public void Unattached_DialogHost_Is_Not_Retained_By_Manager()
    {
        var manager = new DialogManager();
        var hostReference = CreateDialogHostReference(manager);

        CollectGarbage();

        Assert.False(hostReference.IsAlive);
        GC.KeepAlive(manager);
    }

    [Fact]
    public void Unattached_ToastHost_Is_Not_Retained_By_Manager()
    {
        var manager = new ToastManager();
        var hostReference = CreateToastHostReference(manager);

        CollectGarbage();

        Assert.False(hostReference.IsAlive);
        GC.KeepAlive(manager);
    }

    [Fact]
    public void EnsureMaximum_Can_Dismiss_The_Entire_Toast_Queue()
    {
        var manager = new ToastManager();
        var toasts = new[] { new Toast(manager), new Toast(manager), new Toast(manager) };
        var dismissed = new List<Toast>();
        manager.OnToastDismissed += (_, toast) => dismissed.Add(toast);

        foreach (var toast in toasts) manager.Queue(toast);

        manager.EnsureMaximum(0);

        Assert.Empty(manager.Toasts);
        Assert.Equal(toasts, dismissed);

        foreach (var toast in toasts) toast.Dispose();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateDialogHostReference(DialogManager manager)
    {
        var host = new DialogHost { Manager = manager };
        return new WeakReference(host);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateToastHostReference(ToastManager manager)
    {
        var host = new ToastHost { Manager = manager };
        return new WeakReference(host);
    }

    private static void CollectGarbage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
