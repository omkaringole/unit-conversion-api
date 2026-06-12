using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;

namespace UnitConverter.Api.Services;

public interface IConversionService
{
    /// <summary>
    /// Converts <paramref name="value"/> from <paramref name="fromUnit"/> to
    /// <paramref name="toUnit"/> within the given <paramref name="category"/>.
    /// </summary>
    /// <exception cref="UnknownCategoryException">The category doesn't exist.</exception>
    /// <exception cref="UnknownUnitException">Either unit doesn't exist in that category.</exception>
    double Convert(UnitCategory category, string fromUnit, string toUnit, double value);

    /// <summary>Returns the units available for the given category.</summary>
    IReadOnlyList<UnitInfo> GetUnits(UnitCategory category);

    /// <summary>Returns every category and the units available for each.</summary>
    IReadOnlyDictionary<UnitCategory, IReadOnlyList<UnitInfo>> GetAllUnits();
}

public sealed class ConversionService : IConversionService
{
    private readonly IUnitCatalog _catalog;

    public ConversionService(IUnitCatalog catalog)
    {
        _catalog = catalog;
    }

    public double Convert(UnitCategory category, string fromUnit, string toUnit, double value)
    {
        var units = GetCategoryUnits(category);

        var from = FindUnit(units, category, fromUnit);
        var to = FindUnit(units, category, toUnit);

        // Route every conversion through the category's base unit so we
        // only need N conversion definitions instead of N*N pairs.
        var baseValue = from.ToBase(value);
        return to.FromBase(baseValue);
    }

    public IReadOnlyList<UnitInfo> GetUnits(UnitCategory category)
    {
        return GetCategoryUnits(category)
            .Select(u => new UnitInfo { Code = u.Code, Name = u.Name })
            .ToList();
    }

    public IReadOnlyDictionary<UnitCategory, IReadOnlyList<UnitInfo>> GetAllUnits()
    {
        return _catalog.Categories.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<UnitInfo>)kvp.Value
                .Select(u => new UnitInfo { Code = u.Code, Name = u.Name })
                .ToList());
    }

    private IReadOnlyList<UnitDefinition> GetCategoryUnits(UnitCategory category)
    {
        if (!_catalog.Categories.TryGetValue(category, out var units))
        {
            throw new UnknownCategoryException(category.ToString());
        }

        return units;
    }

    private static UnitDefinition FindUnit(IReadOnlyList<UnitDefinition> units, UnitCategory category, string code)
    {
        var match = units.FirstOrDefault(u => string.Equals(u.Code, code, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            throw new UnknownUnitException(code, category.ToString());
        }

        return match;
    }
}
