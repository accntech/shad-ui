using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ShadUI.Demo.Browser.Views;

public partial class CatalogSidebar : UserControl
{
    public CatalogSidebar() => InitializeComponent();

    private void OnExternalLinkClick(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { Tag: string url }) BrowserHistory.OpenExternal(url);
    }
}
