using VectorAi.MarkdownToPdf;

namespace MarkdownToPdf.Tests;


public class DimensionTests
{
    private const double DefaultFontSize = 12.0;
    private const double DefaultWidth = 500.0;
    private const int Precision = 4;

    [Fact]
    public void Constructor_Empty_IsEmptyIsTrue()
    {
        var dim = new Dimension();
        Assert.True(dim.IsEmpty);
    }

    [Theory]
    [InlineData(10, 10)] // Points
    [InlineData(72, 72)] // 1 inch in points
    public void FromPoints_ReturnsCorrectValue(double input, double expected)
    {
        var dim = Dimension.FromPoints(input);
        Assert.Equal(expected, dim.Eval(DefaultFontSize, DefaultWidth).Point, Precision);
    }

    [Fact]
    public void FromInches_ReturnsCorrectPoints()
    {
        var dim = Dimension.FromInches(1); // 1 inch = 72 points
        Assert.Equal(72.0, dim.Eval(DefaultFontSize, DefaultWidth).Point, Precision);
    }

    [Fact]
    public void FromCentimeters_ReturnsCorrectPoints()
    {
        // 1 cm = 72 / 2.54 points
        var dim = Dimension.FromCentimeters(1);
        var expected = 72 / 2.54;
        Assert.Equal(expected, dim.Eval(DefaultFontSize, DefaultWidth).Point, Precision);
    }

    [Fact]
    public void FromFontSize_CalculatesRelativeSize()
    {
        // 2em at 12pt font = 24pt
        var dim = Dimension.FromFontSize(2.0);
        Assert.Equal(24.0, dim.Eval(12.0, DefaultWidth).Point, Precision);
    }

    [Theory]
    [InlineData(50, 250.0)]  // 50% of 500
    [InlineData(150, 500.0)] // Clamped to 100% (based on FromContainerWidth logic)
    [InlineData(-150, -500.0)] // Clamped to -100%
    public void FromContainerWidth_CalculatesRelativeSizeAndClamps(double percent, double expected)
    {
        var dim = Dimension.FromContainerWidth(percent);
        Assert.Equal(expected, dim.Eval(DefaultFontSize, 500.0).Point, Precision);
    }

    [Theory]
    [InlineData("10", 10.0)]
    [InlineData("10pt", 10.0)]
    [InlineData("1in", 72.0)]
    [InlineData("2.5cm", 70.8661)]
    [InlineData("10mm", 28.3465)]
    [InlineData("2em", 24.0)]
    [InlineData("50%", 250.0)]
    public void Parse_ValidStrings_ReturnsCorrectDimensions(string input, double expectedPoints)
    {
        var dim = Dimension.Parse(input);
        Assert.Equal(expectedPoints, dim.Eval(12.0, 500.0).Point, Precision);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("10px")] // px is not supported in the switch
    [InlineData("")]
    public void Parse_InvalidStrings_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => Dimension.Parse(input));
    }

    [Fact]
    public void AdditionOperator_CombinesDimensions()
    {
        var dim1 = Dimension.FromPoints(10);
        var dim2 = Dimension.FromFontSize(1); // 12pt

        var result = dim1 + dim2;

        // 10 + 12 = 22
        Assert.Equal(22.0, result.Eval(12.0, DefaultWidth).Point, Precision);
    }

    [Fact]
    public void SubtractionOperator_SubtractsDimensions()
    {
        var dim1 = Dimension.FromPoints(50);
        var dim2 = Dimension.FromPoints(10);

        var result = dim1 - dim2;

        Assert.Equal(40.0, result.Eval(DefaultFontSize, DefaultWidth).Point, Precision);
    }

    [Fact]
    public void Eval_InvalidArguments_ThrowsArgumentException()
    {
        var dim = Dimension.FromPoints(10);
        Assert.Throws<ArgumentException>(() => dim.Eval(0, 100));
        Assert.Throws<ArgumentException>(() => dim.Eval(12, 0));
    }

    [Fact]
    public void ImplicitConversion_FromDouble_Works()
    {
        Dimension dim = 15.5; // Implicitly points
        Assert.Equal(15.5, dim.Eval(DefaultFontSize, DefaultWidth).Point);
    }

    [Fact]
    public void ImplicitConversion_FromString_Works()
    {
        Dimension dim = "1in";
        Assert.Equal(72.0, dim.Eval(DefaultFontSize, DefaultWidth).Point);
    }

    [Fact]
    public void IsEmptyOrZero_ReturnsTrueCorrectly()
    {
        var emptyDim = new Dimension();
        var zeroDim = Dimension.FromPoints(0);
        var calculatedZero = Dimension.FromPoints(10) - Dimension.FromPoints(10);

        Assert.True(emptyDim.IsEmptyOrZero(12, 500));
        Assert.True(zeroDim.IsEmptyOrZero(12, 500));
        Assert.True(calculatedZero.IsEmptyOrZero(12, 500));
    }
}