using Microsoft.AspNetCore.Mvc;
using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;
using UnitConverter.Api.Services;

namespace UnitConverter.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class ConversionsController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConversionsController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    /// <summary>
    /// Converts a value from one unit to another within the same category.
    /// </summary>
    /// <response code="200">The conversion was performed successfully.</response>
    /// <response code="400">The request was invalid, or the category/unit isn't recognised.</response>
    [HttpPost("conversions")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ConversionResponse> Convert([FromBody] ConversionRequest request)
    {
        if (!Enum.TryParse<UnitCategory>(request.Category, ignoreCase: true, out var category))
        {
            throw new UnknownCategoryException(request.Category);
        }

        var convertedValue = _conversionService.Convert(category, request.From, request.To, request.Value);

        var response = new ConversionResponse
        {
            Category = category.ToString(),
            From = request.From,
            To = request.To,
            InputValue = request.Value,
            ConvertedValue = convertedValue
        };

        return Ok(response);
    }

    /// <summary>
    /// Lists every supported category along with the units available in it.
    /// Useful for clients that want to build a dynamic "from"/"to" picker.
    /// </summary>
    [HttpGet("units")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyDictionary<UnitCategory, IReadOnlyList<UnitInfo>>> GetAllUnits()
    {
        return Ok(_conversionService.GetAllUnits());
    }

    /// <summary>
    /// Lists the units available for a single category.
    /// </summary>
    [HttpGet("units/{category}")]
    [ProducesResponseType(typeof(IReadOnlyList<UnitInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IReadOnlyList<UnitInfo>> GetUnitsForCategory(string category)
    {
        if (!Enum.TryParse<UnitCategory>(category, ignoreCase: true, out var parsedCategory))
        {
            throw new UnknownCategoryException(category);
        }

        return Ok(_conversionService.GetUnits(parsedCategory));
    }
}
