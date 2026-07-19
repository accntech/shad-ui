using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ShadUI.Demo.Browser.Views;

public partial class MobileCatalogMenu : UserControl
{
    public MobileCatalogMenu() => InitializeComponent();

    public void CloseFlyout() => MenuButton.Flyout?.Hide();

    private void OnExternalLinkClick(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { Tag: string url }) BrowserHistory.OpenExternal(url);
    }
}
