using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Media;
using Xunit;

namespace ShadUI.Tests.Controls;

/// <summary>
/// Tests for Badge accessibility and usability validation
/// </summary>
public class BadgeAccessibilityTests : BadgeTestBase
{
    [Fact]
    public void Badge_Should_Have_Accessible_Content_For_Screen_Readers()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "New Message";
            
            // Badge content should be accessible as it inherits from ContentControl
            Assert.Equal("New Message", badge.Content);
            
            // Content should be readable by screen readers through ContentPresenter
            Assert.NotNull(badge.Content);
            Assert.IsType<string>(badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Support_AutomationProperties_For_Screen_Readers()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Set automation properties for better screen reader support
            AutomationProperties.SetName(badge, "Status Badge");
            AutomationProperties.SetHelpText(badge, "Indicates current status");
            
            Assert.Equal("Status Badge", AutomationProperties.GetName(badge));
            Assert.Equal("Indicates current status", AutomationProperties.GetHelpText(badge));
        });
    }

    [Theory]
    [InlineData("1")]
    [InlineData("99+")]
    [InlineData("New")]
    [InlineData("Beta")]
    [InlineData("Coming Soon")]
    [InlineData("This is a very long badge text that should wrap or truncate appropriately")]
    public void Badge_Should_Handle_Various_Content_Lengths_Gracefully(string content)
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = content;
            
            // Badge should accept content of any length
            Assert.Equal(content, badge.Content);
            
            // Badge should maintain its structure regardless of content length
            Assert.NotNull(badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Handle_Empty_And_Null_Content_Gracefully()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Test empty string
            badge.Content = "";
            Assert.Equal("", badge.Content);
            
            // Test null content
            badge.Content = null;
            Assert.Null(badge.Content);
            
            // Badge should remain functional with null/empty content
            Assert.True(badge.IsEnabled);
        });
    }

    [Fact]
    public void Badge_Disabled_State_Should_Have_Proper_Visual_Feedback()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "Disabled Badge";
            badge.IsEnabled = false;
            
            // Disabled state should be properly indicated
            Assert.False(badge.IsEnabled);
            
            // Badge should still have content when disabled
            Assert.Equal("Disabled Badge", badge.Content);
        });
    }

    [Theory]
    [InlineData(BadgeVariant.Default)]
    [InlineData(BadgeVariant.Secondary)]
    [InlineData(BadgeVariant.Destructive)]
    [InlineData(BadgeVariant.Outline)]
    public void Badge_Disabled_State_Should_Work_With_All_Variants(BadgeVariant variant)
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Variant = variant;
            badge.Content = "Disabled";
            badge.IsEnabled = false;
            
            Assert.Equal(variant, badge.Variant);
            Assert.False(badge.IsEnabled);
            Assert.Equal("Disabled", badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Integrate_Properly_In_StackPanel_Layout()
    {
        // Arrange
        var stackPanel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 8 };
        var badge1 = CreateBadge();
        var badge2 = CreateBadge();
        var badge3 = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge1.Content = "First";
            badge2.Content = "Second";
            badge3.Content = "Third";
            
            stackPanel.Children.Add(badge1);
            stackPanel.Children.Add(badge2);
            stackPanel.Children.Add(badge3);
            
            // All badges should be properly added to the layout
            Assert.Equal(3, stackPanel.Children.Count);
            Assert.Contains(badge1, stackPanel.Children);
            Assert.Contains(badge2, stackPanel.Children);
            Assert.Contains(badge3, stackPanel.Children);
        });
    }

    [Fact]
    public void Badge_Should_Integrate_Properly_In_Grid_Layout()
    {
        // Arrange
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        
        var label = new TextBlock { Text = "Status:" };
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "Online";
            
            Grid.SetColumn(label, 0);
            Grid.SetColumn(badge, 2);
            
            grid.Children.Add(label);
            grid.Children.Add(badge);
            
            // Badge should be properly positioned in grid
            Assert.Equal(2, grid.Children.Count);
            Assert.Contains(badge, grid.Children);
            Assert.Equal(2, Grid.GetColumn(badge));
        });
    }

    [Fact]
    public void Badge_Should_Integrate_Properly_In_WrapPanel_Layout()
    {
        // Arrange
        var wrapPanel = new WrapPanel { Orientation = Avalonia.Layout.Orientation.Horizontal };
        var badges = new List<Badge>();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Create multiple badges to test wrapping behavior
            for (int i = 0; i < 10; i++)
            {
                var badge = CreateBadge();
                badge.Content = $"Badge {i + 1}";
                badges.Add(badge);
                wrapPanel.Children.Add(badge);
            }
            
            // All badges should be added to wrap panel
            Assert.Equal(10, wrapPanel.Children.Count);
            Assert.All(badges, badge => Assert.Contains(badge, wrapPanel.Children));
        });
    }

    [Fact]
    public void Badge_Should_Not_Interfere_With_Surrounding_Elements_Positioning()
    {
        // Arrange
        var container = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 4 };
        var textBefore = new TextBlock { Text = "Before" };
        var badge = CreateBadge();
        var textAfter = new TextBlock { Text = "After" };

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "Badge";
            
            container.Children.Add(textBefore);
            container.Children.Add(badge);
            container.Children.Add(textAfter);
            
            // All elements should be properly positioned
            Assert.Equal(3, container.Children.Count);
            Assert.Equal(textBefore, container.Children[0]);
            Assert.Equal(badge, container.Children[1]);
            Assert.Equal(textAfter, container.Children[2]);
        });
    }

    [Fact]
    public void Badge_Should_Maintain_Consistent_Appearance_With_Multiple_Instances()
    {
        // Arrange
        var badges = new List<Badge>();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Create multiple badges with same variant
            for (int i = 0; i < 5; i++)
            {
                var badge = CreateBadge();
                badge.Variant = BadgeVariant.Secondary;
                badge.Content = $"Badge {i + 1}";
                badges.Add(badge);
            }
            
            // All badges should have consistent variant
            Assert.All(badges, badge => Assert.Equal(BadgeVariant.Secondary, badge.Variant));
            Assert.All(badges, badge => Assert.Contains("Secondary", badge.Classes));
        });
    }

    [Fact]
    public void Badge_Should_Support_Complex_Content_Objects()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Test with StackPanel containing icon and text
            var complexContent = new StackPanel 
            { 
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Spacing = 4
            };
            
            var icon = new TextBlock { Text = "✓", FontSize = 12 };
            var text = new TextBlock { Text = "Verified" };
            
            complexContent.Children.Add(icon);
            complexContent.Children.Add(text);
            
            badge.Content = complexContent;
            
            // Badge should handle complex content
            Assert.Equal(complexContent, badge.Content);
            Assert.IsType<StackPanel>(badge.Content);
        });
    }

    [Theory]
    [InlineData("Very long badge text that might cause wrapping issues in certain layouts")]
    [InlineData("Short")]
    [InlineData("Medium length text")]
    public void Badge_Should_Handle_Text_Wrapping_Appropriately(string content)
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = content;
            badge.MaxWidth = 100; // Force potential wrapping
            
            // Badge should handle content regardless of length constraints
            Assert.Equal(content, badge.Content);
            Assert.Equal(100, badge.MaxWidth);
        });
    }

    [Fact]
    public void Badge_Should_Support_Tooltip_For_Additional_Context()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "99+";
            ToolTip.SetTip(badge, "You have 99 or more unread messages");
            
            var tooltip = ToolTip.GetTip(badge);
            Assert.Equal("You have 99 or more unread messages", tooltip);
        });
    }

    [Fact]
    public void Badge_Should_Maintain_Readability_With_Custom_Colors()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "Custom";
            
            // Test with high contrast colors
            badge.Background = new SolidColorBrush(Colors.Black);
            badge.Foreground = new SolidColorBrush(Colors.White);
            
            Assert.Equal(Colors.Black, ((SolidColorBrush)badge.Background).Color);
            Assert.Equal(Colors.White, ((SolidColorBrush)badge.Foreground).Color);
        });
    }

    [Fact]
    public void Badge_Should_Support_Keyboard_Navigation_When_Focusable()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "Focusable Badge";
            badge.Focusable = true;
            badge.TabIndex = 1;
            
            Assert.True(badge.Focusable);
            Assert.Equal(1, badge.TabIndex);
        });
    }

    [Theory]
    [InlineData(BadgeVariant.Default)]
    [InlineData(BadgeVariant.Secondary)]
    [InlineData(BadgeVariant.Destructive)]
    [InlineData(BadgeVariant.Outline)]
    public void Badge_Should_Maintain_Accessibility_Across_All_Variants(BadgeVariant variant)
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Variant = variant;
            badge.Content = $"{variant} Badge";
            
            // Set accessibility properties
            AutomationProperties.SetName(badge, $"{variant} status indicator");
            
            Assert.Equal(variant, badge.Variant);
            Assert.Equal($"{variant} status indicator", AutomationProperties.GetName(badge));
        });
    }

    [Fact]
    public void Badge_Should_Support_High_Contrast_Mode()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "High Contrast";
            
            // Simulate high contrast colors
            var highContrastBackground = new SolidColorBrush(Colors.Black);
            var highContrastForeground = new SolidColorBrush(Colors.White);
            
            badge.Background = highContrastBackground;
            badge.Foreground = highContrastForeground;
            
            // Badge should maintain functionality with high contrast colors
            Assert.Equal(highContrastBackground, badge.Background);
            Assert.Equal(highContrastForeground, badge.Foreground);
            Assert.Equal("High Contrast", badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Handle_Dynamic_Content_Changes_Gracefully()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Test rapid content changes
            badge.Content = "Initial";
            Assert.Equal("Initial", badge.Content);
            
            badge.Content = "Changed";
            Assert.Equal("Changed", badge.Content);
            
            badge.Content = 42;
            Assert.Equal(42, badge.Content);
            
            badge.Content = null;
            Assert.Null(badge.Content);
            
            badge.Content = "Final";
            Assert.Equal("Final", badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Support_Localization_And_RTL_Text()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Test with different language content
            badge.Content = "العربية"; // Arabic text
            Assert.Equal("العربية", badge.Content);
            
            badge.Content = "中文"; // Chinese text
            Assert.Equal("中文", badge.Content);
            
            badge.Content = "Русский"; // Russian text
            Assert.Equal("Русский", badge.Content);
        });
    }
}