using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace ShadUI.Tests.Controls;

[Collection(AvaloniaUiRegressionCollection.Name)]
public class ColorPickerRegressionTests : AvaloniaUiRegressionTestBase
{
    [Fact]
    public void ColorPicker_Preview_Updates_Inside_Dialog_Content()
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            var picker = new ColorPicker
            {
                Color = Colors.Red,
                // The regression concerns the preview content template. Keeping this
                // control template minimal avoids instantiating the closed flyout.
                Template = new FuncControlTemplate<ColorPicker>((control, _) =>
                    new ContentControl { Content = control.Content })
            };
            var host = new DialogHost
            {
                Dialog = new UserControl { Content = picker },
                IsDialogOpen = true
            };
            var window = Show(host);

            try
            {
                var preview = picker.GetVisualDescendants()
                    .OfType<TextBlock>()
                    .Single(x => x.Text == "#FFFF0000");

                picker.Color = Colors.Blue;
                Dispatcher.UIThread.RunJobs();

                Assert.Equal("#FF0000FF", preview.Text);
            }
            finally
            {
                window.Close();
            }
        });
    }
}
