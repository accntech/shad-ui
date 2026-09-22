using System;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Represents a loading spinner or a determinate circular progress indicator.
/// </summary>
[PseudoClasses(":indeterminate")]
[TemplatePart("PART_Indicator", typeof(Path))]
public class Loading : RangeBase
{
    private Path? _indicator;

    /// <summary>
    ///     Defines the <see cref="IsIndeterminate" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsIndeterminateProperty =
        AvaloniaProperty.Register<Loading, bool>(nameof(IsIndeterminate), true);

    static Loading()
    {
        ValueProperty.OverrideMetadata<Loading>(new(defaultBindingMode: BindingMode.OneWay));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Loading" /> control.
    /// </summary>
    public Loading()
    {
        PseudoClasses.Set(":indeterminate", IsIndeterminate);
    }

    /// <summary>
    ///     Gets or sets whether to show a spinning indicator instead of the current value.
    ///     Defaults to <see langword="true" />. Set to <see langword="false" /> to display
    ///     <see cref="RangeBase.Value" /> within <see cref="RangeBase.Minimum" /> and
    ///     <see cref="RangeBase.Maximum" /> as a stationary ring.
    /// </summary>
    public bool IsIndeterminate
    {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicator = e.NameScope.Find<Path>("PART_Indicator");
        UpdateIndicator();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsIndeterminateProperty)
            PseudoClasses.Set(":indeterminate", IsIndeterminate);

        if (change.Property == ValueProperty || change.Property == MinimumProperty ||
            change.Property == MaximumProperty || change.Property == IsIndeterminateProperty)
            UpdateIndicator();
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer() => new LoadingAutomationPeer(this);

    private void UpdateIndicator()
    {
        if (_indicator is null) return;

        var range = Maximum - Minimum;
        // Halving first avoids overflow for ranges spanning both extremes of double.
        var progress = range <= 0 ? 1 : double.IsInfinity(range)
            ? (Value / 2 - Minimum / 2) / (Maximum / 2 - Minimum / 2)
            : (Value - Minimum) / range;
        progress = Math.Clamp(progress, 0, 1);

        if (IsIndeterminate || progress == 0)
        {
            _indicator.Data = null;
        }
        else if (progress == 1)
        {
            _indicator.Data = new EllipseGeometry(new Rect(1, 1, 18, 18));
        }
        else
        {
            var angle = progress * Math.Tau;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(new Point(10, 1), false);
                context.ArcTo(new Point(10 + 9 * Math.Sin(angle), 10 - 9 * Math.Cos(angle)),
                    new Size(9, 9), 0, progress > 0.5, SweepDirection.Clockwise);
                context.EndFigure(false);
            }
            _indicator.Data = geometry;
        }
    }
}
