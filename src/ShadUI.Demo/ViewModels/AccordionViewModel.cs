using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

public sealed partial class AccordionViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _accordionCode = """
                                    <StackPanel Spacing="8" Orientation="Horizontal" HorizontalAlignment="Center">
                                        <controls:Accordion Width="495">
                                            <controls:AccordionItem Header="Product Information">
                                                <controls:AccordionItem.Content>
                                                    <StackPanel Spacing="10">
                                                        <StackPanel>
                                                            <TextBlock Text="Our flagship product combines cutting-edge technology with sleek design." />
                                                            <TextBlock Text="Built with premium materials, it offers unparalleled performance and reliability." />
                                                        </StackPanel>
                                                        <TextBlock Text="Key features include advanced processing capabilities, and an intuitive user interface designed for both beginners and experts." />
                                                    </StackPanel>
                                                </controls:AccordionItem.Content>
                                            </controls:AccordionItem>
                                            <controls:AccordionItem Header="Shipping Details">
                                                <controls:AccordionItem.Content>
                                                    <StackPanel Spacing="10">
                                                        <TextBlock Text="We offer worldwide shipping through trusted courier partners. Standard delivery takes 3-5 business days, while express shipping ensures delivery within 1-2 business days." />
                                                        <TextBlock Text="All orders are carefully packaged and fully insured. Track your shipment in real-time through our dedicated tracking portal." />
                                                    </StackPanel>
                                                </controls:AccordionItem.Content>
                                            </controls:AccordionItem>
                                            <controls:AccordionItem Header="Return policy">
                                                <controls:AccordionItem.Content>
                                                    <StackPanel Spacing="10">
                                                        <TextBlock Text="We stand behind our products with a comprehensive 30-day return policy. If you're not completely satisfied, simply return the item in its original condition." />
                                                        <TextBlock Text="Our hassle-free return process includes free return shipping and full refunds processed within 48 hours of receiving the returned item." />
                                                    </StackPanel>
                                                </controls:AccordionItem.Content>
                                            </controls:AccordionItem>
                                        </controls:Accordion>
                                    </StackPanel>
                                    """;
}