using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ShadUI.Tests.Controls;

[CollectionDefinition("Avalonia UI regression tests", DisableParallelization = true)]
public sealed class AvaloniaUiRegressionCollection
{
    public const string Name = "Avalonia UI regression tests";
}

public abstract class AvaloniaUiRegressionTestBase
{
    protected static Avalonia.Controls.Window Show(Control content)
    {
        var theme = new ShadTheme();
        Application.Current!.Styles.Add(theme);

        var window = new Avalonia.Controls.Window
        {
            Content = content,
            Height = 300,
            Width = 400
        };
        window.Closed += (_, _) => Application.Current?.Styles.Remove(theme);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }
        catch
        {
            Application.Current.Styles.Remove(theme);
            throw;
        }
    }
}
