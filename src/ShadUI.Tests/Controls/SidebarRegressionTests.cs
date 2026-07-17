using Avalonia.Threading;
using Xunit;

namespace ShadUI.Tests.Controls;

[Collection(AvaloniaUiRegressionCollection.Name)]
public class SidebarRegressionTests : AvaloniaUiRegressionTestBase
{
    [Fact]
    public void Sidebar_Restores_Expanded_Width_After_Starting_Collapsed()
    {
        AvaloniaTestFixture.RunOnUIThread(() =>
        {
            var sidebar = new Sidebar
            {
                Width = 220,
                MinWidth = 60,
                Expanded = false,
                ExpandAnimationDuration = 0,
                CollapseAnimationDuration = 0
            };
            var window = Show(sidebar);

            try
            {
                Assert.Equal(60, sidebar.Width);

                sidebar.Expanded = true;
                Dispatcher.UIThread.RunJobs();

                Assert.Equal(220, sidebar.Width);
                Assert.Equal(220, sidebar.Bounds.Width);
            }
            finally
            {
                window.Close();
            }
        });
    }
}
