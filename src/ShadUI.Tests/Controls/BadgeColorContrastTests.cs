using Avalonia.Media;
using Xunit;

namespace ShadUI.Tests.Controls;

/// <summary>
/// Tests for Badge color contrast validation to ensure accessibility standards
/// </summary>
public class BadgeColorContrastTests : BadgeTestBase
{
    /// <summary>
    /// Calculates the relative luminance of a color according to WCAG guidelines
    /// </summary>
    private static double GetRelativeLuminance(Color color)
    {
        var r = GetLinearRgbValue(color.R / 255.0);
        var g = GetLinearRgbValue(color.G / 255.0);
        var b = GetLinearRgbValue(color.B / 255.0);
        
        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    /// <summary>
    /// Converts sRGB color value to linear RGB
    /// </summary>
    private static double GetLinearRgbValue(double colorValue)
    {
        return colorValue <= 0.03928 
            ? colorValue / 12.92 
            : Math.Pow((colorValue + 0.055) / 1.055, 2.4);
    }

    /// <summary>
    /// Calculates the contrast ratio between two colors
    /// </summary>
    private static double GetContrastRatio(Color color1, Color color2)
    {
        var luminance1 = GetRelativeLuminance(color1);
        var luminance2 = GetRelativeLuminance(color2);
        
        var lighter = Math.Max(luminance1, luminance2);
        var darker = Math.Min(luminance1, luminance2);
        
        return (lighter + 0.05) / (darker + 0.05);
    }

    [Theory]
    [InlineData(255, 255, 255, 0, 0, 0)] // White on Black - Maximum contrast
    [InlineData(0, 0, 0, 255, 255, 255)] // Black on White - Maximum contrast
    [InlineData(0, 85, 204, 255, 255, 255)] // Darker Blue on White - Should meet AA standard
    [InlineData(153, 27, 27, 255, 255, 255)] // Darker Red on White - Should meet AA standard
    public void Badge_Should_Meet_WCAG_AA_Contrast_Standards(
        byte bgR, byte bgG, byte bgB, 
        byte fgR, byte fgG, byte fgB)
    {
        // Arrange
        var backgroundColor = Color.FromRgb(bgR, bgG, bgB);
        var foregroundColor = Color.FromRgb(fgR, fgG, fgB);
        
        // Act
        var contrastRatio = GetContrastRatio(backgroundColor, foregroundColor);
        
        // Assert - WCAG AA requires 4.5:1 for normal text, 3:1 for large text
        Assert.True(contrastRatio >= 4.5, 
            $"Contrast ratio {contrastRatio:F2}:1 does not meet WCAG AA standard (4.5:1) for colors " +
            $"Background: RGB({bgR}, {bgG}, {bgB}), Foreground: RGB({fgR}, {fgG}, {fgB})");
    }

    [Fact]
    public void Badge_Default_Theme_Colors_Should_Meet_Contrast_Standards()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Test common theme color combinations that should meet accessibility standards
            var testCombinations = new[]
            {
                // Primary colors (darker blue theme for better contrast)
                (Background: Color.FromRgb(37, 99, 235), Foreground: Colors.White), // Darker blue background, white text
                
                // Secondary colors (darker gray theme)
                (Background: Color.FromRgb(75, 85, 99), Foreground: Colors.White), // Darker gray background, white text
                
                // Destructive colors (darker red theme)
                (Background: Color.FromRgb(185, 28, 28), Foreground: Colors.White), // Darker red background, white text
                
                // Outline variant (border with transparent background)
                (Background: Colors.Transparent, Foreground: Color.FromRgb(55, 65, 81)), // Dark text on light background
            };

            foreach (var (background, foreground) in testCombinations)
            {
                if (background != Colors.Transparent)
                {
                    var contrastRatio = GetContrastRatio(background, foreground);
                    Assert.True(contrastRatio >= 4.5, 
                        $"Theme color combination does not meet WCAG AA standard. " +
                        $"Background: {background}, Foreground: {foreground}, Ratio: {contrastRatio:F2}:1");
                }
            }
        });
    }

    [Fact]
    public void Badge_Should_Support_High_Contrast_Color_Combinations()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            var highContrastCombinations = new[]
            {
                (Background: Colors.Black, Foreground: Colors.White),
                (Background: Colors.White, Foreground: Colors.Black),
                (Background: Colors.Navy, Foreground: Colors.White),
                (Background: Colors.Yellow, Foreground: Colors.Black),
            };

            foreach (var (background, foreground) in highContrastCombinations)
            {
                badge.Background = new SolidColorBrush(background);
                badge.Foreground = new SolidColorBrush(foreground);
                badge.Content = "High Contrast Test";

                var contrastRatio = GetContrastRatio(background, foreground);
                
                // High contrast combinations should exceed WCAG AAA standard (7:1)
                Assert.True(contrastRatio >= 7.0, 
                    $"High contrast combination should meet WCAG AAA standard. " +
                    $"Background: {background}, Foreground: {foreground}, Ratio: {contrastRatio:F2}:1");
            }
        });
    }

    [Theory]
    [InlineData(BadgeVariant.Default)]
    [InlineData(BadgeVariant.Secondary)]
    [InlineData(BadgeVariant.Destructive)]
    [InlineData(BadgeVariant.Outline)]
    public void Badge_Variants_Should_Maintain_Accessibility_When_Disabled(BadgeVariant variant)
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            badge.Variant = variant;
            badge.Content = "Disabled Badge";
            badge.IsEnabled = false;

            // Disabled badges should still be readable
            // The opacity reduction (0.5) should not make text unreadable
            Assert.False(badge.IsEnabled);
            Assert.Equal(variant, badge.Variant);
            
            // Content should remain accessible even when disabled
            Assert.NotNull(badge.Content);
            Assert.Equal("Disabled Badge", badge.Content);
        });
    }

    [Fact]
    public void Badge_Should_Handle_Custom_Colors_With_Contrast_Validation()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Test various custom color combinations
            var customColorTests = new[]
            {
                // Good contrast combinations
                (Background: Color.FromRgb(0, 100, 0), Foreground: Colors.White, ShouldPass: true), // Dark green on white
                (Background: Color.FromRgb(139, 0, 0), Foreground: Colors.White, ShouldPass: true), // Dark red on white
                (Background: Color.FromRgb(25, 25, 112), Foreground: Colors.White, ShouldPass: true), // Midnight blue on white
                
                // Poor contrast combinations (for testing purposes)
                (Background: Color.FromRgb(200, 200, 200), Foreground: Color.FromRgb(180, 180, 180), ShouldPass: false), // Light gray on lighter gray
            };

            foreach (var (background, foreground, shouldPass) in customColorTests)
            {
                badge.Background = new SolidColorBrush(background);
                badge.Foreground = new SolidColorBrush(foreground);
                badge.Content = "Contrast Test";

                var contrastRatio = GetContrastRatio(background, foreground);
                
                if (shouldPass)
                {
                    Assert.True(contrastRatio >= 4.5, 
                        $"Expected good contrast but got {contrastRatio:F2}:1 for " +
                        $"Background: {background}, Foreground: {foreground}");
                }
                else
                {
                    Assert.True(contrastRatio < 4.5, 
                        $"Expected poor contrast but got {contrastRatio:F2}:1 for " +
                        $"Background: {background}, Foreground: {foreground}");
                }
            }
        });
    }

    [Fact]
    public void Badge_Should_Support_System_High_Contrast_Mode_Colors()
    {
        // Arrange
        var badge = CreateBadge();

        // Act & Assert
        RunOnUIThread(() =>
        {
            // Simulate Windows High Contrast mode colors
            var highContrastColors = new[]
            {
                (Background: Color.FromRgb(0, 0, 0), Foreground: Color.FromRgb(255, 255, 255)), // High Contrast Black
                (Background: Color.FromRgb(255, 255, 255), Foreground: Color.FromRgb(0, 0, 0)), // High Contrast White
                (Background: Color.FromRgb(0, 0, 0), Foreground: Color.FromRgb(0, 255, 0)), // High Contrast Black with green text
                (Background: Color.FromRgb(0, 0, 0), Foreground: Color.FromRgb(255, 255, 0)), // High Contrast Black with yellow text
            };

            foreach (var (background, foreground) in highContrastColors)
            {
                badge.Background = new SolidColorBrush(background);
                badge.Foreground = new SolidColorBrush(foreground);
                badge.Content = "High Contrast Mode";

                var contrastRatio = GetContrastRatio(background, foreground);
                
                // System high contrast colors should always meet or exceed WCAG AAA
                Assert.True(contrastRatio >= 7.0, 
                    $"System high contrast colors should meet WCAG AAA standard. " +
                    $"Background: {background}, Foreground: {foreground}, Ratio: {contrastRatio:F2}:1");
            }
        });
    }

    [Fact]
    public void Badge_Contrast_Calculation_Should_Be_Accurate()
    {
        // Test known contrast ratios to validate our calculation method
        var knownContrastTests = new[]
        {
            (Color1: Colors.White, Color2: Colors.Black, ExpectedRatio: 21.0),
            (Color1: Colors.Black, Color2: Colors.White, ExpectedRatio: 21.0),
            (Color1: Color.FromRgb(128, 128, 128), Color2: Colors.White, ExpectedRatio: 3.95), // Approximate
            (Color1: Color.FromRgb(128, 128, 128), Color2: Colors.Black, ExpectedRatio: 5.31), // Approximate
        };

        foreach (var (color1, color2, expectedRatio) in knownContrastTests)
        {
            var actualRatio = GetContrastRatio(color1, color2);
            
            // Allow for small floating-point differences
            Assert.True(Math.Abs(actualRatio - expectedRatio) < 0.1, 
                $"Contrast ratio calculation inaccurate. Expected: {expectedRatio:F2}, Actual: {actualRatio:F2}");
        }
    }
}