using System;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;

namespace ShadUI;

internal sealed class LoadingAutomationPeer(Loading owner) : RangeBaseAutomationPeer(owner), IRangeValueProvider
{
    protected override string GetClassNameCore() => nameof(Loading);

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ProgressBar;

    protected override object? GetProviderCore(Type providerType) =>
        providerType == typeof(IRangeValueProvider) && owner.IsIndeterminate
            ? null
            : base.GetProviderCore(providerType);

    public override bool IsReadOnly => true;

    double IRangeValueProvider.SmallChange => double.NaN;

    double IRangeValueProvider.LargeChange => double.NaN;

    void IRangeValueProvider.SetValue(double value) =>
        throw new InvalidOperationException("Loading is a read-only progress indicator.");
}
