namespace UnitConverter.Api.Models;

/// <summary>
/// Response body returned for a successful conversion.
/// </summary>
public sealed class ConversionResponse
{
    public string Category { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public double InputValue { get; set; }
    public double ConvertedValue { get; set; }
}
