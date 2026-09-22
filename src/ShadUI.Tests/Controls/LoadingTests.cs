using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;
using Path = Avalonia.Controls.Shapes.Path;

namespace ShadUI.Tests.Controls;

[Collection(AvaloniaUiRegressionCollection.Name)]
public class LoadingTests : AvaloniaUiRegressionTestBase
{
    [Fact]
    public void Default_Loading_Remains_An_Indeterminate_Spinner()
    {
        WithLoading(new Loading(), loading =>
        {
            Assert.True(loading.IsIndeterminate);
            Assert.Equal(0, loading.Minimum);
            Assert.Equal(100, loading.Maximum);
            Assert.Equal(0, loading.Value);
            Assert.True(Part<PathIcon>(loading, "PART_Spinner").IsEffectivelyVisible);
            Assert.False(Part<Path>(loading, "PART_Indicator").IsEffectivelyVisible);
            Assert.Contains(":indeterminate", loading.Classes);
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(75)]
    [InlineData(100)]
    public void Determinate_Ring_Handles_Endpoints_And_Partial_Values(double value)
    {
        WithLoading(new Loading { IsIndeterminate = false, Value = value }, loading =>
        {
            var indicator = Part<Path>(loading, "PART_Indicator");
            Assert.True(indicator.IsEffectivelyVisible);
            Assert.False(Part<PathIcon>(loading, "PART_Spinner").IsEffectivelyVisible);
            Assert.Equal(0.15, Part<Ellipse>(loading, "PART_Track").Opacity);
            Assert.Equal(PenLineCap.Round, indicator.StrokeLineCap);
            if (value == 0) Assert.Null(indicator.Data);
            else if (value == 100) Assert.IsType<EllipseGeometry>(indicator.Data);
            else Assert.IsType<StreamGeometry>(indicator.Data);
        });
    }

    [Fact]
    public void Range_And_Mode_Changes_Refresh_The_Visible_Geometry()
    {
        WithLoading(new Loading { IsIndeterminate = false, Minimum = 20, Maximum = 60, Value = 40 }, loading =>
        {
            var indicator = Part<Path>(loading, "PART_Indicator");
            var halfway = Assert.IsType<StreamGeometry>(indicator.Data);
            loading.Maximum = 100;
            Assert.NotSame(halfway, indicator.Data);
            loading.Value = 200;
            Assert.Equal(100, loading.Value);
            Assert.IsType<EllipseGeometry>(indicator.Data);
            loading.Value = -10;
            Assert.Equal(20, loading.Value);
            Assert.Null(indicator.Data);
            loading.Minimum = 100;
            Assert.IsType<EllipseGeometry>(indicator.Data);

            loading.IsIndeterminate = true;
            Assert.Contains(":indeterminate", loading.Classes);
            Assert.Null(indicator.Data);
            Assert.True(Part<PathIcon>(loading, "PART_Spinner").IsEffectivelyVisible);
            loading.IsIndeterminate = false;
            Assert.DoesNotContain(":indeterminate", loading.Classes);
            Assert.IsType<EllipseGeometry>(indicator.Data);
        });
    }

    [Fact]
    public void Ring_Resizes_Uniformly_And_Inherits_Foreground()
    {
        WithLoading(new Loading { IsIndeterminate = false, Value = 50, Width = 50, Height = 30, Foreground = Brushes.Red }, loading =>
        {
            var indicator = Part<Path>(loading, "PART_Indicator");
            var track = Part<Ellipse>(loading, "PART_Track");
            Assert.Same(Brushes.Red, indicator.Stroke);
            Assert.Same(Brushes.Red, track.Stroke);
            var transform = indicator.TransformToVisual(loading)!.Value;
            Assert.Equal(1.5, transform.M11, 5);
            Assert.Equal(transform.M11, transform.M22);
            loading.Height = 50;
            Dispatcher.UIThread.RunJobs();
            transform = indicator.TransformToVisual(loading)!.Value;
            Assert.Equal(2.5, transform.M11, 5);
        });
    }

    [Fact]
    public void Bound_Value_Survives_Mode_Changes()
    {
        var source = new Slider { Value = 25 };
        var loading = new Loading { IsIndeterminate = false };
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            loading.Bind(Loading.ValueProperty, new Binding(nameof(Slider.Value)) { Source = source });
        });
        WithLoading(loading, control =>
        {
            Assert.Equal(25, control.Value);
            control.IsIndeterminate = true;
            source.Value = 75;
            control.IsIndeterminate = false;
            Assert.Equal(75, control.Value);
            Assert.IsType<StreamGeometry>(Part<Path>(control, "PART_Indicator").Data);
        });
    }

    [Fact]
    public void Automation_Exposes_A_ReadOnly_Range_Only_When_Determinate()
    {
        WithLoading(new Loading(), loading =>
        {
            AutomationProperties.SetName(loading, "Upload progress");
            var peer = ControlAutomationPeer.CreatePeerForElement(loading)!;
            Assert.Equal(AutomationControlType.ProgressBar, peer.GetAutomationControlType());
            Assert.Equal("Upload progress", peer.GetName());
            Assert.Null(peer.GetProvider<IRangeValueProvider>());

            loading.IsIndeterminate = false;
            loading.Value = 56;
            var provider = peer.GetProvider<IRangeValueProvider>()!;
            Assert.NotNull(provider);
            Assert.True(provider.IsReadOnly);
            Assert.Equal(0, provider.Minimum);
            Assert.Equal(100, provider.Maximum);
            Assert.Equal(56, provider.Value);
            Assert.Throws<InvalidOperationException>(() => provider.SetValue(80));
            Assert.Equal(56, loading.Value);

            var valueChanged = false;
            peer.PropertyChanged += (_, e) => valueChanged |= e.Property == RangeValuePatternIdentifiers.ValueProperty;
            loading.Value = 75;
            Assert.True(valueChanged);
            loading.IsIndeterminate = true;
            Assert.Null(peer.GetProvider<IRangeValueProvider>());
        });
    }

    [Fact]
    public void Existing_Busy_Controls_Keep_Their_Spinners()
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            var button = new Button { Content = "Save" };
            ButtonAssist.SetShowProgress(button, true);
            var input = new TextBox();
            ControlAssist.SetShowProgress(input, true);
            var busy = new BusyArea { IsBusy = true };
            var panel = new StackPanel { Children = { button, input, busy } };
            var window = Show(panel);
            try
            {
                foreach (var control in new Control[] { button, input, busy })
                {
                    var spinner = Assert.Single(control.GetVisualDescendants().OfType<Loading>());
                    Assert.True(spinner.IsIndeterminate);
                    Assert.True(Part<PathIcon>(spinner, "PART_Spinner").IsEffectivelyVisible);
                }
            }
            finally { window.Close(); }
        });
    }

    private static T Part<T>(Control control, string name) where T : Control =>
        control.GetVisualDescendants().OfType<T>().Single(x => x.Name == name);

    private static void WithLoading(Loading loading, Action<Loading> check)
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            loading.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            loading.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            var window = Show(loading);
            try { check(loading); }
            finally { window.Close(); }
        });
    }
}
