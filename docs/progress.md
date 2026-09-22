# Loading and progress

Include `ShadTheme` in your application styles. Use the `ShadUI` namespace for
`Loading`; the horizontal progress control is Avalonia's native `ProgressBar`.

```xml
xmlns:shadui="clr-namespace:ShadUI;assembly=ShadUI"
```

## Circular progress

`Loading` spins by default. Existing usages in buttons, inputs, and `BusyArea`
continue to work without changes. Set `IsIndeterminate="False"` to display a
stationary progress arc over a faint ring.

```xml
<shadui:Loading Width="24" Height="24" />
<shadui:Loading Width="40" Height="40"
                IsIndeterminate="False" Value="56"
                AutomationProperties.Name="Upload progress" />
```

The arc fills clockwise from the top. At zero only the track is visible; at
completion the indicator is a full circle. `Foreground` colors the arc and the
15%-opacity track. Width and height scale the ring and its stroke uniformly.

`Minimum`, `Maximum`, and `Value` default to 0, 100, and 0. Values are clamped by
Avalonia's `RangeBase`. For example, `Minimum="20" Maximum="60" Value="40"`
displays 50% completion. Equal minimum and maximum display a complete ring.
Setting `Value` does not change the mode; set `IsIndeterminate` explicitly.

## Progress bar

The ShadUI theme gives `ProgressBar` a rounded, 8-pixel track using the theme's
primary colors. It defaults to determinate mode and supports custom ranges,
brushes, dimensions, padding, corner radius, vertical orientation, and
right-to-left flow. Indeterminate mode animates a segment across the track.

```xml
<ProgressBar Value="33" />
<ProgressBar IsIndeterminate="True" AutomationProperties.Name="Loading" />
<ProgressBar Minimum="20" Maximum="60" Value="40" />
```

Compose a label and percentage above the bar using ordinary controls. Bind the
text to `Percentage`, which accounts for custom minimum and maximum values,
and associate the label with the bar for accessibility.

```xml
<Grid ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto" MaxWidth="384">
    <Label x:Name="UploadLabel" Content="Upload progress" Padding="0"
           Target="{Binding #UploadProgress}" />
    <TextBlock Grid.Column="1"
               Text="{Binding #UploadProgress.Percentage, StringFormat={}{0:0}%}"
               IsVisible="{Binding !#UploadProgress.IsIndeterminate}" />
    <ProgressBar x:Name="UploadProgress" Grid.Row="1" Grid.ColumnSpan="2"
                 Margin="0,8,0,0" Value="56"
                 AutomationProperties.LabeledBy="{Binding #UploadLabel}" />
</Grid>
```

Native `ShowProgressText` and `ProgressTextFormat` also remain available for text
inside the bar. The format arguments are value, percentage, minimum, and maximum
respectively. Progress text is hidden while indeterminate.

```xml
<ProgressBar Minimum="20" Maximum="60" Value="40"
             ShowProgressText="True"
             ProgressTextFormat="{}{0:0} / {3:0} ({1:0}%)" />
```

The desktop and browser catalogs include **Loading** and **Progress** pages with
interactive value and mode controls, endpoint examples, and copyable XAML.
