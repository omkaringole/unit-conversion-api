using System.ComponentModel.DataAnnotations;

namespace UnitConverter.Api.Models;

/// <summary>
/// Request body for POST /api/v1/conversions.
/// </summary>
public sealed class ConversionRequest
{
    /// <summary>Category name, e.g. "length", "temperature", "mass".</summary>
    [Required]
    public string Category { get; set; } = string.Empty;

    /// <summary>Unit code to convert from, e.g. "m", "celsius", "kg".</summary>
    [Required]
    public string From { get; set; } = string.Empty;

    /// <summary>Unit code to convert to.</summary>
    [Required]
    public string To { get; set; } = string.Empty;

    /// <summary>The numeric value to convert.</summary>
    public double Value { get; set; }
}
