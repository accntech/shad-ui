using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;

namespace ShadUI.AotSmoke;

internal static class Program
{
    private static int Main()
    {
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        try
        {
            Dispatcher.UIThread.Invoke(Run);
            Console.WriteLine("ShadUI AOT smoke test passed.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ShadUI AOT smoke test failed: {ex}");
            return 1;
        }
    }

    private static void Run()
    {
        var dialogManager = new DialogManager().Register<SmokeDialog, SmokeDialogContext>();
        var toastManager = new ToastManager();

        var window = new SmokeWindow
        {
            Width = 800,
            Height = 600,
            Hosts =
            {
                new DialogHost { Manager = dialogManager },
                new ToastHost { Manager = toastManager }
            },
            // Controls created in code have no XAML name scope, which NumericUpDownAssist relies on.
            Content = WithNameScope(new StackPanel
            {
                Children =
                {
                    new Button { Content = "Button" },
                    new TextBox { Text = "TextBox" },
                    new CheckBox { Content = "CheckBox" },
                    new ComboBox { ItemsSource = new[] { "One", "Two" }, SelectedIndex = 0 },
                    new NumericUpDown { Value = 1 },
                    new ColorView(),
                    new Calendar()
                }
            })
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        dialogManager.CreateDialog("Simple dialog", "Message").WithPrimaryButton("OK", () => { }).Show();
        Dispatcher.UIThread.RunJobs();

        var context = new SmokeDialogContext();
        dialogManager.CreateDialog(context).Show();
        Dispatcher.UIThread.RunJobs();
        dialogManager.Close(context);

        toastManager.CreateToast("Toast").Show();
        Dispatcher.UIThread.RunJobs();

        window.Close();
    }

    private static T WithNameScope<T>(T control) where T : Control
    {
        NameScope.SetNameScope(control, new NameScope());
        return control;
    }
}

internal sealed class App : Application
{
    public override void Initialize()
    {
        Styles.Add(new ShadTheme());
    }
}

internal sealed class SmokeWindow : Window;

internal sealed class SmokeDialogContext;

// Only referenced through DialogManager.Register, so the trimmer must keep its constructor.
internal sealed class SmokeDialog : UserControl
{
    public SmokeDialog()
    {
        Content = new TextBlock { Text = "Custom dialog" };
    }
}
