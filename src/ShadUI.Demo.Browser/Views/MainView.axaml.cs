using Avalonia.Controls;

namespace ShadUI.Demo.Browser.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SizeChanged += (_, _) => MobileMenu.CloseFlyout();
    }
}
