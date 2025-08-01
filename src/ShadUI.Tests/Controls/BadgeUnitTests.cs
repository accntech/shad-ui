using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Xunit;

namespace ShadUI.Tests.Controls;

public class BadgeUnitTests : BadgeTestBase
{
    [Fact]
    public void Badge_Should_Inherit_From_ContentControl()
    {
        // Arrange & Act
        var badge = CreateBadge();

        // Assert
        Assert.IsAssignableFrom<ContentControl>(badge);
    }

    [Fact]
    public void Badge_Should_Have_Default_Variant_By_Default()
    {
        // Arrange & Act
        var badge = CreateBadge();

        // Assert
        RunOnUIThread(() =>
        {
            Assert.Equal(BadgeVariant.Default, badge.Variant);
        });
    }

    [Theory]
    [InlineData(BadgeVariant.Default)]
    [InlineData(BadgeVariant.Secondary)]
    [InlineData(BadgeVariant.Destructive)]
    [InlineData(BadgeVariant.Outline)]
    public void Badge_Should_Set_Variant_Property_Correctly(BadgeVariant variant)
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Variant = variant;
            Assert.Equal(variant, badge.Variant);
        });
    }

    [Fact]
    public void Badge_Should_Add_CSS_Class_For_Non_Default_Variants()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Variant = BadgeVariant.Secondary;
            Assert.Contains("Secondary", badge.Classes);
        });
    }

    [Fact]
    public void Badge_Should_Not_Add_CSS_Class_For_Default_Variant()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Variant = BadgeVariant.Default;
            Assert.DoesNotContain("Default", badge.Classes);
        });
    }

    [Fact]
    public void Badge_Should_Remove_Old_CSS_Class_When_Variant_Changes()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Variant = BadgeVariant.Secondary;
            badge.Variant = BadgeVariant.Destructive;
            
            Assert.DoesNotContain("Secondary", badge.Classes);
            Assert.Contains("Destructive", badge.Classes);
        });
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("1")]
    [InlineData("99+")]
    [InlineData("")]
    [InlineData(null)]
    public void Badge_Should_Handle_String_Content(string content)
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = content;
            Assert.Equal(content, badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Handle_Numeric_Content()
    {
        // Arrange
        var badge = CreateBadge();
        var numericContent = 42;

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = numericContent;
            Assert.Equal(numericContent, badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Handle_Complex_Content_Objects()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            var complexContent = new TextBlock { Text = "Complex Content" };
            badge.Content = complexContent;
            
            Assert.Equal(complexContent, badge.Content);
            Assert.IsType<TextBlock>(badge.Content);
        });
    }

    [Fact]
    public void Badge_VariantProperty_Should_Be_Styled_Property()
    {
        // Arrange & Act
        var property = Badge.VariantProperty;

        // Assert
        Assert.NotNull(property);
        Assert.Equal(nameof(Badge.Variant), property.Name);
        Assert.Equal(typeof(BadgeVariant), property.PropertyType);
        Assert.Equal(typeof(Badge), property.OwnerType);
        Assert.Equal(BadgeVariant.Default, property.GetDefaultValue(typeof(Badge)));
    }

    [Fact]
    public void Badge_Should_Support_Property_Binding()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            var binding = new Avalonia.Data.Binding("TestProperty");
            // Should not throw
            badge.Bind(Badge.VariantProperty, binding);
        });
    }

    [Fact]
    public void Badge_Should_Inherit_Standard_ContentControl_Properties()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Test that we can set standard properties
            badge.Background = Brushes.Red;
            badge.Foreground = Brushes.White;
            badge.Padding = new Thickness(10);
            badge.Margin = new Thickness(5);

            Assert.Equal(Brushes.Red, badge.Background);
            Assert.Equal(Brushes.White, badge.Foreground);
            Assert.Equal(new Thickness(10), badge.Padding);
            Assert.Equal(new Thickness(5), badge.Margin);
        });
    }

    [Fact]
    public void Badge_Should_Support_IsEnabled_Property()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.IsEnabled = false;
            Assert.False(badge.IsEnabled);
        });
    }

    [Theory]
    [InlineData(BadgeVariant.Default, "Default")]
    [InlineData(BadgeVariant.Secondary, "Secondary")]
    [InlineData(BadgeVariant.Destructive, "Destructive")]
    [InlineData(BadgeVariant.Outline, "Outline")]
    public void BadgeVariant_Enum_Should_Have_Correct_String_Representation(BadgeVariant variant, string expected)
    {
        // Act
        var result = variant.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Badge_Should_Handle_Multiple_Variant_Changes()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Variant = BadgeVariant.Secondary;
            Assert.Equal(BadgeVariant.Secondary, badge.Variant);
            Assert.Contains("Secondary", badge.Classes);

            badge.Variant = BadgeVariant.Destructive;
            Assert.Equal(BadgeVariant.Destructive, badge.Variant);
            Assert.DoesNotContain("Secondary", badge.Classes);
            Assert.Contains("Destructive", badge.Classes);

            badge.Variant = BadgeVariant.Default;
            Assert.Equal(BadgeVariant.Default, badge.Variant);
            Assert.DoesNotContain("Destructive", badge.Classes);
        });
    }

    [Fact]
    public void Badge_Should_Support_Content_Property_Changes()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Content = "Initial";
            Assert.Equal("Initial", badge.Content);

            badge.Content = "Changed";
            Assert.Equal("Changed", badge.Content);

            badge.Content = 123;
            Assert.Equal(123, badge.Content);

            badge.Content = null;
            Assert.Null(badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Support_Theme_Resource_Binding()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            var primaryBrush = new SolidColorBrush(Colors.Blue);
            var primaryForegroundBrush = new SolidColorBrush(Colors.White);

            badge.Background = primaryBrush;
            badge.Foreground = primaryForegroundBrush;

            Assert.Equal(primaryBrush, badge.Background);
            Assert.Equal(primaryForegroundBrush, badge.Foreground);
        });
    }

    [Fact]
    public void Badge_Should_Support_Corner_Radius_Theming()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            var cornerRadius = new CornerRadius(4);
            badge.CornerRadius = cornerRadius;
            Assert.Equal(cornerRadius, badge.CornerRadius);
        });
    }

    [Fact]
    public void Badge_Should_Support_Font_Properties_Theming()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.FontSize = 12;
            badge.FontWeight = FontWeight.Medium;

            Assert.Equal(12, badge.FontSize);
            Assert.Equal(FontWeight.Medium, badge.FontWeight);
        });
    }
}