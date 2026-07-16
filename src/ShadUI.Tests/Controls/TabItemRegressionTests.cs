using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using Xunit;

namespace ShadUI.Tests.Controls;

[Collection(AvaloniaUiRegressionCollection.Name)]
public class TabItemRegressionTests : AvaloniaUiRegressionTestBase
{
    [Fact]
    public void TabItem_Template_Background_Covers_Empty_Header_Area()
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            var tabItem = new TabItem
            {
                Background = Brushes.Transparent,
                Header = "Tab",
                Width = 160
            };
            var window = Show(new TabControl { ItemsSource = new[] { tabItem } });

            try
            {
                var templateBorder = tabItem.GetVisualDescendants().OfType<Border>().First();
                Assert.Same(Brushes.Transparent, templateBorder.Background);
            }
            finally
            {
                window.Close();
            }
        });
    }
}
