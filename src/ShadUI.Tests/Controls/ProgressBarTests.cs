using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace ShadUI.Tests.Controls;

[Collection(AvaloniaUiRegressionCollection.Name)]
public class ProgressBarTests : AvaloniaUiRegressionTestBase
{
    [Theory]
    [InlineData(0)]
    [InlineData(33)]
    [InlineData(100)]
    public void Theme_Uses_A_Slim_Rounded_Track_And_Accurate_Fill(double value)
    {
        WithProgress(new ProgressBar { Width = 200, Value = value }, bar =>
        {
            var track = Part<Border>(bar, "PART_Track");
            Assert.Equal(8, bar.Bounds.Height);
            Assert.True(track.ClipToBounds);
            Assert.Equal(new CornerRadius(999), track.CornerRadius);
            Assert.Equal(value * 2, Part<Border>(bar, "PART_Indicator").Bounds.Width, 5);
            Assert.Equal(value, bar.Percentage);
        });
    }

    [Fact]
    public void Fill_Recalculates_After_Resize_Padding_And_Range_Changes()
    {
        WithProgress(new ProgressBar { Width = 200, Minimum = 20, Maximum = 60, Value = 40 }, bar =>
        {
            var indicator = Part<Border>(bar, "PART_Indicator");
            Assert.Equal(100, indicator.Bounds.Width);
            bar.Width = 300;
            bar.Padding = new Thickness(10, 0);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(140, indicator.Bounds.Width);
            bar.Maximum = 100;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(70, indicator.Bounds.Width);
            bar.Minimum = 100;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(280, indicator.Bounds.Width);
            Assert.Equal(100, bar.Percentage);
        });
    }

    [Fact]
    public void Mode_Switching_Hides_Stale_Text_And_Restores_Current_Value()
    {
        WithProgress(new ProgressBar { Width = 200, Value = 56, ShowProgressText = true }, bar =>
        {
            var indicator = Part<Border>(bar, "PART_Indicator");
            var animated = Part<Border>(bar, "PART_IndeterminateIndicator");
            var text = Part<LayoutTransformControl>(bar, "PART_ProgressText");
            Assert.Equal("56%", text.GetVisualDescendants().OfType<TextBlock>().Single().Text);
            bar.IsIndeterminate = true;
            Dispatcher.UIThread.RunJobs();
            Assert.False(indicator.IsEffectivelyVisible);
            Assert.True(animated.IsEffectivelyVisible);
            Assert.False(text.IsEffectivelyVisible);
            Assert.False(Part<Border>(bar, "PART_ProgressTextClip").IsEffectivelyVisible);
            Assert.Equal(80, animated.Bounds.Width);
            bar.Value = 75;
            bar.IsIndeterminate = false;
            Dispatcher.UIThread.RunJobs();
            Assert.True(indicator.IsEffectivelyVisible);
            Assert.False(animated.IsEffectivelyVisible);
            Assert.True(text.IsEffectivelyVisible);
            Assert.Equal(150, indicator.Bounds.Width);
            Assert.Equal("75%", text.GetVisualDescendants().OfType<TextBlock>().Single().Text);
        });
    }

    [Fact]
    public void Vertical_Progress_Fills_From_The_Bottom_And_Can_Change_Orientation()
    {
        WithProgress(new ProgressBar { Orientation = Orientation.Vertical, Height = 200, Value = 25 }, bar =>
        {
            var indicator = Part<Border>(bar, "PART_Indicator");
            Assert.Equal(8, bar.Bounds.Width);
            Assert.Equal(50, indicator.Bounds.Height);
            Assert.Equal(150, indicator.Bounds.Y);
            bar.IsIndeterminate = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(80, Part<Border>(bar, "PART_IndeterminateIndicator").Bounds.Height);
            bar.IsIndeterminate = false;
            bar.Height = double.NaN;
            bar.Width = 200;
            bar.Orientation = Orientation.Horizontal;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(50, indicator.Bounds.Width);
            Assert.Equal(8, indicator.Bounds.Height);
        });
    }

    [Fact]
    public void RightToLeft_Fill_Is_Mirrored()
    {
        WithProgress(new ProgressBar { Width = 200, Value = 25, FlowDirection = FlowDirection.RightToLeft }, bar =>
        {
            var indicator = Part<Border>(bar, "PART_Indicator");
            var window = TopLevel.GetTopLevel(bar)!;
            var left = indicator.TranslatePoint(new Point(0, 0), window)!.Value.X;
            var right = indicator.TranslatePoint(new Point(50, 0), window)!.Value.X;
            var barRight = bar.TranslatePoint(new Point(0, 0), window)!.Value.X;
            Assert.True(left > right);
            Assert.Equal(barRight, left, 5);
        });
    }

    [Fact]
    public void Theme_Changes_And_Local_Appearance_Overrides_Are_Respected()
    {
        WithProgress(new ProgressBar { Width = 200, Value = 50 }, bar =>
        {
            var track = Part<Border>(bar, "PART_Track");
            var indicator = Part<Border>(bar, "PART_Indicator");
            var window = (Avalonia.Controls.Window)TopLevel.GetTopLevel(bar)!;
            window.RequestedThemeVariant = ThemeVariant.Light;
            var light = ((ISolidColorBrush)indicator.Background!).Color;
            window.RequestedThemeVariant = ThemeVariant.Dark;
            Assert.NotEqual(light, ((ISolidColorBrush)indicator.Background!).Color);
            bar.Foreground = Brushes.Red;
            bar.Background = Brushes.Blue;
            bar.CornerRadius = new CornerRadius(2);
            bar.Height = 12;
            bar.IsEnabled = false;
            Dispatcher.UIThread.RunJobs();
            Assert.Same(Brushes.Red, indicator.Background);
            Assert.Same(Brushes.Blue, track.Background);
            Assert.Equal(new CornerRadius(2), track.CornerRadius);
            Assert.Equal(12, track.Bounds.Height);
            Assert.Equal(0.5, bar.Opacity);
        });
    }

    [Fact]
    public void Percentage_Binding_Formatting_And_Accessibility_Use_The_Native_Range()
    {
        WithProgress(new ProgressBar
        {
            Width = 200, Minimum = 20, Maximum = 60, Value = 40,
            ShowProgressText = true, ProgressTextFormat = "{0:0} / {3:0} ({1:0}%)"
        }, bar =>
        {
            var value = new TextBlock();
            using var binding = value.Bind(TextBlock.TextProperty,
                new Binding(nameof(ProgressBar.Percentage)) { Source = bar, StringFormat = "{0:0}%" });
            Assert.Equal("50%", value.Text);
            Assert.Equal("40 / 60 (50%)", Part<TextBlock>(bar, "PART_ProgressTextBlock").Text);
            AutomationProperties.SetName(bar, "Upload progress");
            var peer = ControlAutomationPeer.CreatePeerForElement(bar)!;
            Assert.Equal("Upload progress", peer.GetName());
            Assert.Equal(AutomationControlType.ProgressBar, peer.GetAutomationControlType());
            Assert.True(peer.GetProvider<IRangeValueProvider>()!.IsReadOnly);
            bar.Value = 60;
            Assert.Equal("100%", value.Text);
        });
    }

    [Theory]
    [InlineData(Orientation.Horizontal, FlowDirection.LeftToRight, 25)]
    [InlineData(Orientation.Horizontal, FlowDirection.LeftToRight, 50)]
    [InlineData(Orientation.Horizontal, FlowDirection.LeftToRight, 100)]
    [InlineData(Orientation.Horizontal, FlowDirection.RightToLeft, 50)]
    [InlineData(Orientation.Vertical, FlowDirection.LeftToRight, 25)]
    [InlineData(Orientation.Vertical, FlowDirection.LeftToRight, 50)]
    [InlineData(Orientation.Vertical, FlowDirection.LeftToRight, 100)]
    public void Progress_Text_Uses_Contrasting_Aligned_Text_Over_The_Filled_Area(
        Orientation orientation, FlowDirection direction, double value)
    {
        WithProgress(new ProgressBar
        {
            Width = orientation == Orientation.Horizontal ? 200 : double.NaN,
            Height = orientation == Orientation.Vertical ? 200 : double.NaN,
            Orientation = orientation, FlowDirection = direction, Value = value, ShowProgressText = true
        }, bar =>
        {
            var clip = Part<Border>(bar, "PART_ProgressTextClip");
            var indicator = Part<Border>(bar, "PART_Indicator");
            var regularText = Part<TextBlock>(bar, "PART_ProgressTextBlock");
            var filledText = clip.GetVisualDescendants().OfType<TextBlock>().Single();
            Assert.True(clip.ClipToBounds);
            Assert.Equal(indicator.Bounds, clip.Bounds);
            Assert.Equal(regularText.Text, filledText.Text);
            Assert.Equal(regularText.TranslatePoint(default, bar), filledText.TranslatePoint(default, bar));
            Assert.NotEqual(((ISolidColorBrush)regularText.Foreground!).Color, ((ISolidColorBrush)filledText.Foreground!).Color);
            Assert.Equal(AccessibilityView.Raw, AutomationProperties.GetAccessibilityView(filledText));
        });
    }

    private static T Part<T>(Control control, string name) where T : Control =>
        control.GetVisualDescendants().OfType<T>().Single(x => x.Name == name);

    private static void WithProgress(ProgressBar bar, Action<ProgressBar> check)
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            bar.HorizontalAlignment = HorizontalAlignment.Center;
            bar.VerticalAlignment = VerticalAlignment.Center;
            var window = Show(bar);
            try { check(bar); }
            finally { window.Close(); }
        });
    }
}
