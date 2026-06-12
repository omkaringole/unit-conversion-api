namespace UnitConverter.Api.Models;

/// <summary>
/// Lightweight DTO describing a unit, used by the "list available units"
/// endpoint. We don't expose UnitDefinition directly because it carries
/// the conversion delegates, which obviously aren't serializable.
/// </summary>
public sealed class UnitInfo
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
