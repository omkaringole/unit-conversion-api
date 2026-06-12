namespace UnitConverter.Api.Models;

/// <summary>
/// Describes a single unit and how to convert a value to and from the
/// "base" unit of its category (e.g. meters for length, kilograms for
/// mass, Celsius for temperature).
///
/// Using a pair of conversion functions instead of a plain multiplier
/// lets us support units like Fahrenheit/Kelvin that involve an offset,
/// not just a scale factor, without any special-casing in the service.
/// </summary>
public sealed class UnitDefinition
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required UnitCategory Category { get; init; }
    public required Func<double, double> ToBase { get; init; }
    public required Func<double, double> FromBase { get; init; }
}
