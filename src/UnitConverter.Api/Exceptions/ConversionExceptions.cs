namespace UnitConverter.Api.Exceptions;

/// <summary>
/// Base type for conversion-related errors that should be surfaced to the
/// caller as a 400 Bad Request rather than a generic 500.
/// </summary>
public abstract class ConversionException : Exception
{
    protected ConversionException(string message) : base(message)
    {
    }
}

/// <summary>
/// Thrown when the requested category doesn't exist (e.g. "lenght").
/// </summary>
public sealed class UnknownCategoryException : ConversionException
{
    public UnknownCategoryException(string category)
        : base($"Unknown unit category '{category}'.")
    {
    }
}

/// <summary>
/// Thrown when a unit code isn't recognised within a given category.
/// </summary>
public sealed class UnknownUnitException : ConversionException
{
    public UnknownUnitException(string unit, string category)
        : base($"Unknown unit '{unit}' for category '{category}'.")
    {
    }
}
