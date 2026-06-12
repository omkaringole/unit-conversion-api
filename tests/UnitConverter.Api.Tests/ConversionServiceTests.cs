using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;
using UnitConverter.Api.Services;
using Xunit;

namespace UnitConverter.Api.Tests;

public class ConversionServiceTests
{
    private readonly IConversionService _sut = new ConversionService(new UnitCatalog());

    [Theory]
    [InlineData(1, "m", "ft", 3.28084)]
    [InlineData(1, "km", "mi", 0.621371)]
    [InlineData(100, "cm", "m", 1)]
    public void Convert_Length_ReturnsExpectedValue(double input, string from, string to, double expected)
    {
        var result = _sut.Convert(UnitCategory.Length, from, to, input);

        Assert.Equal(expected, result, precision: 4);
    }

    [Theory]
    [InlineData(0, "celsius", "fahrenheit", 32)]
    [InlineData(100, "celsius", "fahrenheit", 212)]
    [InlineData(0, "celsius", "kelvin", 273.15)]
    [InlineData(32, "fahrenheit", "celsius", 0)]
    public void Convert_Temperature_ReturnsExpectedValue(double input, string from, string to, double expected)
    {
        var result = _sut.Convert(UnitCategory.Temperature, from, to, input);

        Assert.Equal(expected, result, precision: 4);
    }

    [Theory]
    [InlineData(1, "kg", "lb", 2.20462)]
    [InlineData(1000, "g", "kg", 1)]
    public void Convert_Mass_ReturnsExpectedValue(double input, string from, string to, double expected)
    {
        var result = _sut.Convert(UnitCategory.Mass, from, to, input);

        Assert.Equal(expected, result, precision: 4);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        var result = _sut.Convert(UnitCategory.Length, "m", "m", 42);

        Assert.Equal(42, result, precision: 6);
    }

    [Fact]
    public void Convert_UnknownUnit_ThrowsUnknownUnitException()
    {
        Assert.Throws<UnknownUnitException>(() => _sut.Convert(UnitCategory.Length, "m", "lightyear", 1));
    }

    [Fact]
    public void Convert_UnitFromDifferentCategory_ThrowsUnknownUnitException()
    {
        // "kg" is a valid unit, but not within the Length category.
        Assert.Throws<UnknownUnitException>(() => _sut.Convert(UnitCategory.Length, "m", "kg", 1));
    }

    [Fact]
    public void GetUnits_ReturnsAllUnitsForCategory()
    {
        var units = _sut.GetUnits(UnitCategory.Mass);

        Assert.Contains(units, u => u.Code == "kg");
        Assert.Contains(units, u => u.Code == "lb");
    }

    [Fact]
    public void GetAllUnits_ReturnsEveryCategory()
    {
        var allUnits = _sut.GetAllUnits();

        Assert.Equal(3, allUnits.Count);
        Assert.True(allUnits.ContainsKey(UnitCategory.Length));
        Assert.True(allUnits.ContainsKey(UnitCategory.Temperature));
        Assert.True(allUnits.ContainsKey(UnitCategory.Mass));
    }
}
