using System.Runtime.InteropServices.JavaScript;

namespace ShadUI.Demo.Browser;

internal static partial class BrowserHistory
{
    [JSImport("globalThis.shaduiBrowserRouter.getRoute")]
    internal static partial string GetRoute();

    [JSImport("globalThis.shaduiBrowserRouter.setRoute")]
    internal static partial void SetRoute(string route, bool replace);

    [JSImport("globalThis.shaduiBrowserRouter.openExternal")]
    internal static partial void OpenExternal(string url);
}
