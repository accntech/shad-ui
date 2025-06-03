using CommunityToolkit.Mvvm.ComponentModel;

namespace ShadUI.Demo.ViewModels;

public sealed partial class ExpandersViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _expandDirectionUpCode = """
                                  <StackPanel HorizontalAlignment="Center" Orientation="Horizontal" Spacing="8">
                                      <Expander Content="Expander content" ExpandDirection="Up" Header="Header"
                                          Padding="10" Width="200" />
                                  </StackPanel>
                                  """;
    [ObservableProperty]
    private string _expandDirectionDownCode = """
                                   <StackPanel HorizontalAlignment="Center" Orientation="Horizontal" Spacing="8">
                                       <Expander Content="Expander content" ExpandDirection="Down" Header="Header"
                                           Padding="10" Width="200" />
                                   </StackPanel>
                                   """;
    [ObservableProperty]
    private string _expandDirectionLeftCode = """
                                   <StackPanel HorizontalAlignment="Center" Orientation="Horizontal" Spacing="8">
                                       <Expander Content="Expander content" ExpandDirection="Left" Header="Header"
                                           HorizontalAlignment="Center" MinWidth="200" Padding="10,0" />
                                   </StackPanel>
                                   """;
    [ObservableProperty]
    private string _expandDirectionRightCode = """
                                   <StackPanel HorizontalAlignment="Center" Orientation="Horizontal" Spacing="8">
                                       <Expander Content="Expander content" ExpandDirection="Right" Header="Header"
                                           HorizontalAlignment="Center" MinWidth="200" Padding="10,0" />
                                   </StackPanel>
                                   """;
    [ObservableProperty]
    private string _expandDirectionDisabledCode = """
                                               <StackPanel HorizontalAlignment="Center" Orientation="Horizontal" Spacing="8">
                                                   <Expander Content="Expander content" ExpandDirection="Down" Header="Header"
                                                       IsEnabled="False" Padding="10" Width="200" />
                                               </StackPanel>
                                               """;
    [ObservableProperty]
    private string _expandDirectionErrorCode = """
                                               <StackPanel HorizontalAlignment="Center" Orientation="Horizontal" Spacing="8">
                                                   <Expander Content="Expander content" ExpandDirection="Down" Header="Header"
                                                       IsEnabled="False" Padding="10" Width="200"
                                                       extensions:ControlAssist.Hint="This control is currently disabled"
                                                       extensions:ControlAssist.Label="Disabled Selection">
                                                       <DataValidationErrors.Error>
                                                           <system:Exception />
                                                       </DataValidationErrors.Error>
                                                   </Expander>
                                               </StackPanel>
                                               """;
}