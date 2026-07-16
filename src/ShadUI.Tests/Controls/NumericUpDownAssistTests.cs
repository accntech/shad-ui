using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Xunit;

namespace ShadUI.Tests.Controls;

public class NumericUpDownAssistTests
{
    [Fact]
    public void TryHandleSpin_Raises_NonWheel_Spin_For_Valid_Direction()
    {
        var numericUpDown = new NumericUpDown { AllowSpin = true };
        var spinner = new ButtonSpinner { ValidSpinDirection = ValidSpinDirections.Increase };
        SpinEventArgs? raised = null;
        spinner.Spin += (_, args) => raised = args;

        var handled = NumericUpDownAssist.TryHandleSpin(
            numericUpDown,
            spinner,
            SpinDirection.Increase);

        Assert.True(handled);
        Assert.NotNull(raised);
        Assert.Equal(SpinDirection.Increase, raised.Direction);
        Assert.False(raised.UsingMouseWheel);
    }

    [Fact]
    public void TryHandleSpin_Does_Not_Consume_When_Spinning_Is_Disabled()
    {
        var numericUpDown = new NumericUpDown { AllowSpin = false };
        var spinner = new ButtonSpinner();
        var raised = false;
        spinner.Spin += (_, _) => raised = true;

        var handled = NumericUpDownAssist.TryHandleSpin(
            numericUpDown,
            spinner,
            SpinDirection.Increase);

        Assert.False(handled);
        Assert.False(raised);
    }

    [Theory]
    [InlineData(true, ValidSpinDirections.Increase)]
    [InlineData(false, ValidSpinDirections.Decrease)]
    public void TryHandleSpin_Consumes_Without_Raising_When_Spin_Is_Not_Allowed(
        bool isReadOnly,
        ValidSpinDirections validDirections)
    {
        var numericUpDown = new NumericUpDown
        {
            AllowSpin = true,
            IsReadOnly = isReadOnly
        };
        var spinner = new ButtonSpinner { ValidSpinDirection = validDirections };
        var raised = false;
        spinner.Spin += (_, _) => raised = true;

        var handled = NumericUpDownAssist.TryHandleSpin(
            numericUpDown,
            spinner,
            SpinDirection.Increase);

        Assert.True(handled);
        Assert.False(raised);
    }
}
