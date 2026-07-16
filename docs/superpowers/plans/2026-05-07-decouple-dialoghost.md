# Decouple DialogHost from ShadUI.Window — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Remove `DialogHost`'s compile-time dependency on `ShadUI.Window` so it works in any container, with Window-only features (drag/maximize/snap-layout suppression) auto-activating when an `Avalonia.Controls.Window` ancestor exists.

**Architecture:** DialogHost stops referencing `ShadUI.Window`; it caches an `Avalonia.Controls.Window` ancestor for drag/maximize and exposes `HasOpenDialog` as the single source of truth. `ShadUI.Window` flips the dependency direction by observing `DialogHost.HasOpenDialog` for any DialogHost in its `Hosts` collection. The `Owner` property on DialogHost is deleted (breaking change).

**Tech Stack:** C# / .NET 9 (multi-target net9/10), Avalonia 12.0.2, xUnit, Avalonia.Headless for tests.

**Spec:** `docs/superpowers/specs/2026-05-07-decouple-dialoghost-design.md`

---

## File Structure

**Modify:**
- `src/ShadUI/Controls/Dialog/DialogHost.axaml.cs` — drop `Owner`, cache ancestor `Avalonia.Controls.Window`, drop `Owner.HasOpenDialog` writes
- `src/ShadUI/Controls/Window/Window.axaml.cs` — observe DialogHost.HasOpenDialog through `Hosts` collection
- `src/ShadUI.Demo/MainWindow.axaml` — remove `Owner="..."` binding
- `src/ShadUI/ShadUI.csproj` — add `InternalsVisibleTo("ShadUI.Tests")` so tests can reach internal `HasOpenDialog`

**Create:**
- `src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs` — verify Window aggregation, dynamic Hosts changes, cleanup, no-Owner

---

## Task 1: Make ShadUI internals visible to the test assembly

**Files:**
- Modify: `src/ShadUI/ShadUI.csproj`

The new tests need to read `Window.HasOpenDialog` and `DialogHost.HasOpenDialog`, both `internal`. Add `InternalsVisibleTo` so the test project can see them.

- [ ] **Step 1: Add InternalsVisibleTo entry**

Edit `src/ShadUI/ShadUI.csproj`. After the `</PropertyGroup>` block on line 11 (the first one), add an `ItemGroup`:

```xml
    <ItemGroup>
        <InternalsVisibleTo Include="ShadUI.Tests" />
    </ItemGroup>
```

- [ ] **Step 2: Verify build**

Run: `dotnet build /Users/ogie/RiderProjects/shad-ui/ShadUI.sln`
Expected: build succeeds, no new warnings.

- [ ] **Step 3: Commit**

```bash
git -C /Users/ogie/RiderProjects/shad-ui add src/ShadUI/ShadUI.csproj
git -C /Users/ogie/RiderProjects/shad-ui commit -m "chore(shadui): expose internals to ShadUI.Tests"
```

---

## Task 2: Window observes DialogHosts in `Hosts` for HasOpenDialog (TDD)

**Files:**
- Modify: `src/ShadUI/Controls/Window/Window.axaml.cs`
- Test: `src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs` (new)

**Background.** Today `DialogHost` writes `Owner.HasOpenDialog = ...`. We're inverting that: `ShadUI.Window` will subscribe to each `DialogHost` in `Hosts` and aggregate `HasOpenDialog`. Test first.

- [ ] **Step 1: Write the failing test**

Create `src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs`:

```csharp
using Avalonia.Controls;
using Avalonia.Headless;
using ShadUI;
using Xunit;

namespace ShadUI.Tests.Controls;

public class DialogHostDecouplingTests
{
    [Fact]
    public void Window_HasOpenDialog_Reflects_Child_DialogHost_State()
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            var host = new DialogHost();
            var window = new ShadUI.Window();
            window.Hosts.Add(host);

            // Simulate the window being shown so OnLoaded fires.
            window.Show();
            try
            {
                Assert.False(window.HasOpenDialog);

                host.HasOpenDialog = true;
                Assert.True(window.HasOpenDialog);

                host.HasOpenDialog = false;
                Assert.False(window.HasOpenDialog);
            }
            finally
            {
                window.Close();
            }
        });
    }
}
```

- [ ] **Step 2: Run the test, verify it fails**

Run: `dotnet test /Users/ogie/RiderProjects/shad-ui/src/ShadUI.Tests/ShadUI.Tests.csproj --filter "FullyQualifiedName~DialogHostDecouplingTests.Window_HasOpenDialog_Reflects_Child_DialogHost_State"`

Expected: FAIL — `window.HasOpenDialog` stays `false` because nothing wires the two together yet.

The headless platform supports `Window.Show()`. `RunOnUIThread` calls `AvaloniaTestFixture.EnsureInitialized()` which configures `UseHeadless(...)`, so the lifecycle (including `OnLoaded`) fires on `Show()`. If you instead see an `InvalidOperationException` about no platform, the fixture wasn't initialized — make sure the test method body uses `AvaloniaTestFixture.RunOnUIThread(...)` rather than running on the xUnit thread directly.

- [ ] **Step 3: Add subscription logic to Window**

Edit `src/ShadUI/Controls/Window/Window.axaml.cs`.

3a. Add a using at the top with the others:

```csharp
using System.Collections.Specialized;
using System.Linq;
```

3b. Add private fields near the other private fields (around line 347, near `_maximizeButton`):

```csharp
    private readonly System.Collections.Generic.List<DialogHost> _subscribedHosts = new();
```

3c. Replace the body of `OnLoaded` (currently lines 288–295) with:

```csharp
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow is Window window && window != this) Icon ??= window.Icon;
        }

        Hosts.CollectionChanged += OnHostsCollectionChanged;
        foreach (var host in Hosts.OfType<DialogHost>())
        {
            SubscribeToDialogHost(host);
        }
        UpdateHasOpenDialog();
    }
```

Note: the original `OnLoaded` early-returns `if (...is not IClassicDesktopStyleApplicationLifetime desktop) return;`. The replacement reorders so the new subscription logic still runs in non-desktop lifetimes (e.g., headless tests).

3d. Add a matching `OnUnloaded` override directly under `OnLoaded`:

```csharp
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        Hosts.CollectionChanged -= OnHostsCollectionChanged;
        foreach (var host in _subscribedHosts.ToArray())
        {
            UnsubscribeFromDialogHost(host);
        }
        UpdateHasOpenDialog();
    }
```

3e. Add the helper methods at the bottom of the class, just before the static `Window()` constructor:

```csharp
    private void OnHostsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<DialogHost>())
            {
                UnsubscribeFromDialogHost(item);
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<DialogHost>())
            {
                SubscribeToDialogHost(item);
            }
        }

        UpdateHasOpenDialog();
    }

    private void SubscribeToDialogHost(DialogHost host)
    {
        if (_subscribedHosts.Contains(host)) return;
        _subscribedHosts.Add(host);
        host.PropertyChanged += OnDialogHostPropertyChanged;
    }

    private void UnsubscribeFromDialogHost(DialogHost host)
    {
        if (!_subscribedHosts.Remove(host)) return;
        host.PropertyChanged -= OnDialogHostPropertyChanged;
    }

    private void OnDialogHostPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == DialogHost.HasOpenDialogProperty)
        {
            UpdateHasOpenDialog();
        }
    }

    private void UpdateHasOpenDialog()
    {
        HasOpenDialog = _subscribedHosts.Any(h => h.HasOpenDialog);
    }
```

- [ ] **Step 4: Run the test, verify it passes**

Run: `dotnet test /Users/ogie/RiderProjects/shad-ui/src/ShadUI.Tests/ShadUI.Tests.csproj --filter "FullyQualifiedName~DialogHostDecouplingTests"`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git -C /Users/ogie/RiderProjects/shad-ui add src/ShadUI/Controls/Window/Window.axaml.cs src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs
git -C /Users/ogie/RiderProjects/shad-ui commit -m "feat(window): observe DialogHost.HasOpenDialog from Hosts collection"
```

---

## Task 3: Window handles DialogHosts added/removed at runtime (TDD)

**Files:**
- Test: `src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs` (extend)
- (No production change expected — Task 2 already wired `Hosts.CollectionChanged`. This task verifies it.)

- [ ] **Step 1: Write the failing test**

Append to `src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs`:

```csharp
    [Fact]
    public void Window_Tracks_DialogHosts_Added_After_Load()
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            var window = new ShadUI.Window();
            window.Show();
            try
            {
                Assert.False(window.HasOpenDialog);

                var host = new DialogHost();
                window.Hosts.Add(host);

                host.HasOpenDialog = true;
                Assert.True(window.HasOpenDialog);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [Fact]
    public void Window_Untracks_DialogHosts_Removed_From_Hosts()
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            var host = new DialogHost();
            var window = new ShadUI.Window();
            window.Hosts.Add(host);
            window.Show();
            try
            {
                host.HasOpenDialog = true;
                Assert.True(window.HasOpenDialog);

                window.Hosts.Remove(host);
                // Aggregator should recompute and drop to false even though host stays open.
                Assert.False(window.HasOpenDialog);
            }
            finally
            {
                window.Close();
            }
        });
    }
```

- [ ] **Step 2: Run the tests**

Run: `dotnet test /Users/ogie/RiderProjects/shad-ui/src/ShadUI.Tests/ShadUI.Tests.csproj --filter "FullyQualifiedName~DialogHostDecouplingTests"`

Expected: PASS — Task 2 already implemented `OnHostsCollectionChanged` and `UpdateHasOpenDialog`.

If either fails, step back and inspect `OnHostsCollectionChanged` — it must call `UpdateHasOpenDialog()` after subscribe/unsubscribe.

- [ ] **Step 3: Commit**

```bash
git -C /Users/ogie/RiderProjects/shad-ui add src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs
git -C /Users/ogie/RiderProjects/shad-ui commit -m "test(window): cover dynamic Hosts add/remove for HasOpenDialog"
```

---

## Task 4: DialogHost caches an ancestor `Avalonia.Controls.Window`

**Files:**
- Modify: `src/ShadUI/Controls/Dialog/DialogHost.axaml.cs`

This task introduces the new ancestor lookup *without* removing `Owner` yet — keeps the diff small and reviewable. The next task removes `Owner`.

- [ ] **Step 1: Add using and a private field**

Edit `src/ShadUI/Controls/Dialog/DialogHost.axaml.cs`. Add to existing usings:

```csharp
using Avalonia.VisualTree;
```

Add a private field near the existing `_disposed` field (line 24):

```csharp
    private Avalonia.Controls.Window? _ancestorWindow;
```

- [ ] **Step 2: Override visual tree attach/detach**

Add these overrides directly above `OnApplyTemplate` (currently line 164):

```csharp
    /// <summary>
    ///     Caches the nearest ancestor Window so drag and maximize work without an explicit Owner.
    /// </summary>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _ancestorWindow = this.FindAncestorOfType<Avalonia.Controls.Window>();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _ancestorWindow = null;
    }

    private Avalonia.Controls.Window? ResolveOwnerWindow()
        => _ancestorWindow ??= this.FindAncestorOfType<Avalonia.Controls.Window>();
```

`ResolveOwnerWindow` covers the "called before attach" edge case from the spec.

- [ ] **Step 3: Build to confirm no compile errors**

Run: `dotnet build /Users/ogie/RiderProjects/shad-ui/src/ShadUI/ShadUI.csproj`
Expected: SUCCESS, no warnings about unused `_ancestorWindow` (it's used by `ResolveOwnerWindow`).

- [ ] **Step 4: Commit**

```bash
git -C /Users/ogie/RiderProjects/shad-ui add src/ShadUI/Controls/Dialog/DialogHost.axaml.cs
git -C /Users/ogie/RiderProjects/shad-ui commit -m "refactor(dialog-host): cache ancestor Window via visual tree"
```

---

## Task 5: Remove `Owner` and route handlers through ancestor Window

**Files:**
- Modify: `src/ShadUI/Controls/Dialog/DialogHost.axaml.cs`

This is the breaking change. After this task, `DialogHost` no longer references `ShadUI.Window`.

- [ ] **Step 1: Delete `OwnerProperty` and `Owner` (lines 25–38)**

Remove the entire block:

```csharp
    /// <summary>
    ///     Defines the <see cref="Owner" /> property.
    /// </summary>
    public static readonly StyledProperty<Window?> OwnerProperty =
        AvaloniaProperty.Register<DialogHost, Window?>(nameof(Owner));

    /// <summary>
    ///     Gets or sets the owner window of the dialog host.
    /// </summary>
    public Window? Owner
    {
        get => GetValue(OwnerProperty);
        set => SetValue(OwnerProperty, value);
    }
```

- [ ] **Step 2: Replace `OnTitleBarPointerPressed` (currently lines 187–198)**

Old:

```csharp
    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
            {
                MainWindow: not null
            } desktop)
        {
            desktop.MainWindow.BeginMoveDrag(e);
        }
    }
```

New:

```csharp
    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        ResolveOwnerWindow()?.BeginMoveDrag(e);
    }
```

- [ ] **Step 3: Replace `OnMaximizeButtonClicked` (currently lines 200–208)**

Old:

```csharp
    private void OnMaximizeButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (Owner is not null && Owner.CanMaximize)
        {
            Owner.WindowState = Owner.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
```

New:

```csharp
    private void OnMaximizeButtonClicked(object? sender, RoutedEventArgs args)
    {
        var window = ResolveOwnerWindow();
        if (window is null || !window.CanMaximize) return;

        window.WindowState = window.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
```

`CanMaximize` is on `Avalonia.Controls.Window`, so this compiles without ShadUI types.

- [ ] **Step 4: Drop `Owner.HasOpenDialog` writes in `CloseDialog` (line 219)**

Old (last line of method):

```csharp
        if (Owner is not null) Owner.HasOpenDialog = false;
```

Delete that line. `CloseDialog` now ends after `Manager?.OpenLast();`.

- [ ] **Step 5: Drop the `Owner is null` early return + write in `ManagerOnDialogShown`**

Old (line 264 + line 274):

```csharp
        if (Manager is null || Owner is null) return;
        ...
        IsDialogOpen = true;
        HasOpenDialog = true;
        Owner.HasOpenDialog = true;
    }
```

New (replace the whole method body to drop both `Owner` references):

```csharp
    private void ManagerOnDialogShown(object? sender, DialogShownEventArgs e)
    {
        if (Manager is null) return;

        Dialog = e.Control;
        Dismissible = e.Options.Dismissible;

        if (e.Options.MaxWidth > 0) DialogMaxWidth = e.Options.MaxWidth;
        if (e.Options.MinWidth > 0) DialogMinWidth = e.Options.MinWidth;

        IsDialogOpen = true;
        HasOpenDialog = true;
    }
```

- [ ] **Step 6: Drop `Owner` references in `ManagerOnDialogClosed`**

Old method:

```csharp
    private async void ManagerOnDialogClosed(object? sender, DialogClosedEventArgs e)
    {
        try
        {
            if (Manager is null || Owner is null) return;
            if (e.Control != Dialog) return;

            IsDialogOpen = false;
            if (e.ReplaceExisting) return;

            HasOpenDialog = Manager.Dialogs.Count > 0;
            Owner.HasOpenDialog = Manager.Dialogs.Count > 0;

            await Task.Delay(200); // Allow animations to complete
            if (!HasOpenDialog) Dialog = null;
        }
        catch (Exception)
        {
            //ignore
        }
    }
```

New:

```csharp
    private async void ManagerOnDialogClosed(object? sender, DialogClosedEventArgs e)
    {
        try
        {
            if (Manager is null) return;
            if (e.Control != Dialog) return;

            IsDialogOpen = false;
            if (e.ReplaceExisting) return;

            HasOpenDialog = Manager.Dialogs.Count > 0;

            await Task.Delay(200); // Allow animations to complete
            if (!HasOpenDialog) Dialog = null;
        }
        catch (Exception)
        {
            //ignore
        }
    }
```

- [ ] **Step 7: Build and check no `Owner`/`MainWindow` references remain**

Run:
```bash
dotnet build /Users/ogie/RiderProjects/shad-ui/src/ShadUI/ShadUI.csproj
grep -n "Owner" /Users/ogie/RiderProjects/shad-ui/src/ShadUI/Controls/Dialog/DialogHost.axaml.cs
```

Expected: build succeeds. The `grep` should return nothing — every `Owner` reference is gone.

- [ ] **Step 8: Run the existing decoupling tests, verify still pass**

Run: `dotnet test /Users/ogie/RiderProjects/shad-ui/src/ShadUI.Tests/ShadUI.Tests.csproj --filter "FullyQualifiedName~DialogHostDecouplingTests"`

Expected: all 3 tests PASS — DialogHost still updates its own `HasOpenDialog`, Window still aggregates.

- [ ] **Step 9: Commit**

```bash
git -C /Users/ogie/RiderProjects/shad-ui add src/ShadUI/Controls/Dialog/DialogHost.axaml.cs
git -C /Users/ogie/RiderProjects/shad-ui commit -m "refactor(dialog-host): drop Owner property and ShadUI.Window coupling"
```

---

## Task 6: Remove `Owner` binding from demo `MainWindow.axaml`

**Files:**
- Modify: `src/ShadUI.Demo/MainWindow.axaml`

The demo project still references the deleted property; build won't pass until this is fixed.

- [ ] **Step 1: Build the solution to surface the breakage**

Run: `dotnet build /Users/ogie/RiderProjects/shad-ui/ShadUI.sln`
Expected: BUILD FAILS in `ShadUI.Demo` with an error pointing at the `Owner` binding on the DialogHost element in `MainWindow.axaml` (around line 33).

- [ ] **Step 2: Remove the binding**

Edit `src/ShadUI.Demo/MainWindow.axaml`. Replace the `<shadui:Window.Hosts>` block (lines 30–35):

Old:
```xml
    <shadui:Window.Hosts>
        <shadui:DialogHost
            Manager="{Binding DialogManager}"
            Owner="{Binding RelativeSource={RelativeSource AncestorType=shadui:Window}}"
            x:Name="PART_DialogHost" />
    </shadui:Window.Hosts>
```

New:
```xml
    <shadui:Window.Hosts>
        <shadui:DialogHost
            Manager="{Binding DialogManager}"
            x:Name="PART_DialogHost" />
    </shadui:Window.Hosts>
```

- [ ] **Step 3: Build to confirm**

Run: `dotnet build /Users/ogie/RiderProjects/shad-ui/ShadUI.sln`
Expected: SUCCESS across all targets (net9/10).

- [ ] **Step 4: Commit**

```bash
git -C /Users/ogie/RiderProjects/shad-ui add src/ShadUI.Demo/MainWindow.axaml
git -C /Users/ogie/RiderProjects/shad-ui commit -m "chore(demo): drop DialogHost.Owner binding (no longer needed)"
```

---

## Task 7: Add a test that DialogHost no longer exposes `Owner`

**Files:**
- Test: `src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs` (extend)

A simple guard test so a future regression that re-adds `Owner` is caught immediately.

- [ ] **Step 1: Append the test**

Add to `DialogHostDecouplingTests`:

```csharp
    [Fact]
    public void DialogHost_Has_No_Owner_Property()
    {
        var hasOwner = typeof(DialogHost).GetProperty("Owner") is not null;
        Assert.False(hasOwner, "DialogHost.Owner must remain removed (decoupling guard).");
    }
```

- [ ] **Step 2: Run the test**

Run: `dotnet test /Users/ogie/RiderProjects/shad-ui/src/ShadUI.Tests/ShadUI.Tests.csproj --filter "FullyQualifiedName~DialogHostDecouplingTests"`

Expected: all 4 tests PASS.

- [ ] **Step 3: Commit**

```bash
git -C /Users/ogie/RiderProjects/shad-ui add src/ShadUI.Tests/Controls/DialogHostDecouplingTests.cs
git -C /Users/ogie/RiderProjects/shad-ui commit -m "test(dialog-host): guard against Owner property regression"
```

---

## Task 8: Manual smoke test in the demo app

**Files:** none modified — pure verification step from the spec's test plan.

- [ ] **Step 1: Run the demo**

Run: `dotnet run --project /Users/ogie/RiderProjects/shad-ui/src/ShadUI.Demo/ShadUI.Demo.csproj`

- [ ] **Step 2: Exercise dialog scenarios**

In the running app:
1. Navigate to "Dialog" page from the sidebar.
2. Trigger each dialog variant (basic, confirmation, custom). For each: verify it animates in, the overlay dims the window, the close button works, Escape dismisses (if dismissible), and the dialog animates out.
3. While a dialog is open, click + drag the area above the dialog (the title bar region) — the window should move.
4. Double-click the area above the dialog — the window should toggle Maximized/Normal.
5. Maximize the window manually, open a dialog, then double-click the area above it — should restore to Normal.

- [ ] **Step 3: Windows-only — snap-layout suppression check**

If on Windows: hover the maximize button in the title bar while a dialog is open. The Windows 11 snap-layout flyout should NOT appear (suppressed by `HasOpenDialog`). Close the dialog, hover again — flyout should appear normally.

(Skip this step on macOS/Linux; note in commit message which OS was tested.)

- [ ] **Step 4: Record outcome**

If all checks pass, this task is done — no commit. If anything fails, file the regression as a follow-up task and stop.

---

## Verification Summary

After all tasks land:

| Spec requirement | Implemented in |
| --- | --- |
| Delete `OwnerProperty` and `Owner` | Task 5 step 1 |
| Cache ancestor `Avalonia.Controls.Window` | Task 4 |
| `OnTitleBarPointerPressed` uses ancestor Window | Task 5 step 2 |
| `OnMaximizeButtonClicked` uses ancestor Window | Task 5 step 3 |
| Drop `Owner.HasOpenDialog` writes | Task 5 steps 4–6 |
| Window observes DialogHosts in `Hosts` | Task 2 |
| Window aggregates `HasOpenDialog` | Task 2 step 3e |
| Window handles dynamic Hosts changes | Task 2 step 3e + Task 3 |
| Window cleans up subscriptions on unload | Task 2 step 3d |
| Demo `MainWindow.axaml` drops `Owner=` | Task 6 |
| Manual UX smoke (drag/maximize/snap-layout) | Task 8 |
