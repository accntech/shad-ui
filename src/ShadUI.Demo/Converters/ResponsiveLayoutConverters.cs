using Avalonia.Data.Converters;

namespace ShadUI.Demo.Converters;

public static class ResponsiveLayoutConverters
{
    public static readonly IValueConverter ToDashboardColumnCount =
        new FuncValueConverter<double, int>(width => width < 640 ? 1 : 2);
}
