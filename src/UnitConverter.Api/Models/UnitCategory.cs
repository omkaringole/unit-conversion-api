namespace UnitConverter.Api.Models;

/// <summary>
/// The measurement categories supported by the API. A conversion can only
/// happen between two units that belong to the same category.
/// </summary>
public enum UnitCategory
{
    Length,
    Temperature,
    Mass
}
