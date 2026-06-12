using UnitConverter.Api.Models;

namespace UnitConverter.Api.Services;

/// <summary>
/// Holds the hardcoded set of units and conversion factors for this version
/// of the API. Every unit converts to/from a "base" unit for its category
/// (meters for length, kilograms for mass, Celsius for temperature), so
/// adding a new unit only requires defining its relationship to that base
/// unit - it doesn't need a conversion path to every other unit.
///
/// If this grows to the "hundreds of units" mentioned in the brief, this
/// is the piece that would move out to a database or a config file and be
/// loaded at startup instead. The <see cref="IUnitCatalog"/> abstraction
/// is there so that swap can happen without touching the service or the
/// controller.
/// </summary>
public interface IUnitCatalog
{
    /// <summary>All categories and the units defined for each of them.</summary>
    IReadOnlyDictionary<UnitCategory, IReadOnlyList<UnitDefinition>> Categories { get; }
}

public sealed class UnitCatalog : IUnitCatalog
{
    public IReadOnlyDictionary<UnitCategory, IReadOnlyList<UnitDefinition>> Categories { get; }

    public UnitCatalog()
    {
        Categories = new Dictionary<UnitCategory, IReadOnlyList<UnitDefinition>>
        {
            [UnitCategory.Length] = BuildLengthUnits(),
            [UnitCategory.Temperature] = BuildTemperatureUnits(),
            [UnitCategory.Mass] = BuildMassUnits()
        };
    }

    // Base unit: meter.
    private static IReadOnlyList<UnitDefinition> BuildLengthUnits() => new List<UnitDefinition>
    {
        Linear("m", "Meter", UnitCategory.Length, 1d),
        Linear("km", "Kilometer", UnitCategory.Length, 1000d),
        Linear("cm", "Centimeter", UnitCategory.Length, 0.01d),
        Linear("mm", "Millimeter", UnitCategory.Length, 0.001d),
        Linear("mi", "Mile", UnitCategory.Length, 1609.344d),
        Linear("yd", "Yard", UnitCategory.Length, 0.9144d),
        Linear("ft", "Foot", UnitCategory.Length, 0.3048d),
        Linear("in", "Inch", UnitCategory.Length, 0.0254d)
    };

    // Base unit: kilogram.
    private static IReadOnlyList<UnitDefinition> BuildMassUnits() => new List<UnitDefinition>
    {
        Linear("kg", "Kilogram", UnitCategory.Mass, 1d),
        Linear("g", "Gram", UnitCategory.Mass, 0.001d),
        Linear("mg", "Milligram", UnitCategory.Mass, 0.000001d),
        Linear("lb", "Pound", UnitCategory.Mass, 0.45359237d),
        Linear("oz", "Ounce", UnitCategory.Mass, 0.028349523125d),
        Linear("st", "Stone", UnitCategory.Mass, 6.35029318d)
    };

    // Base unit: Celsius. Temperature conversions aren't a simple
    // multiplication, so each unit defines its own ToBase/FromBase pair.
    private static IReadOnlyList<UnitDefinition> BuildTemperatureUnits() => new List<UnitDefinition>
    {
        new()
        {
            Code = "celsius",
            Name = "Celsius",
            Category = UnitCategory.Temperature,
            ToBase = value => value,
            FromBase = value => value
        },
        new()
        {
            Code = "fahrenheit",
            Name = "Fahrenheit",
            Category = UnitCategory.Temperature,
            ToBase = value => (value - 32d) * 5d / 9d,
            FromBase = value => value * 9d / 5d + 32d
        },
        new()
        {
            Code = "kelvin",
            Name = "Kelvin",
            Category = UnitCategory.Temperature,
            ToBase = value => value - 273.15d,
            FromBase = value => value + 273.15d
        }
    };

    /// <summary>
    /// Helper for the common case of a unit that's a straight multiple of
    /// its category's base unit (e.g. 1 km = 1000 m).
    /// </summary>
    private static UnitDefinition Linear(string code, string name, UnitCategory category, double factorToBase) => new()
    {
        Code = code,
        Name = name,
        Category = category,
        ToBase = value => value * factorToBase,
        FromBase = value => value / factorToBase
    };
}
