using Microsoft.AspNetCore.Mvc;
using DrugIndications.Application.Services;
using Swashbuckle.AspNetCore.Annotations;
using DrugIndications.Domain.Entities;


namespace DrugIndications.API.Controllers;

[ApiController]
[Route("indications")]
public class IndicationsController : ControllerBase
{
    private readonly IndicationExtractorService _extractor;

    public IndicationsController(IndicationExtractorService extractor)
    {
        _extractor = extractor;
    }

    /// <summary>
    /// Extracts medical conditions from drug label text.
    /// </summary>
    /// <param name="request">The input text containing eligibility criteria in natural language.</param>
    /// <returns>A list of structured eligibility requirements.</returns>
    [HttpPost("extract")]
    [SwaggerOperation(
        Summary = "Extracts medical conditions from drug label text",
        Description = "Uses AI to analyze drug label text and extract structured indications mapped to ICD-10 codes."
    )]
    [ProducesResponseType(typeof(List<DrugIndication>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ExtractFromLabel([FromBody] string labelText)
    {
        if (string.IsNullOrWhiteSpace(labelText))
            return BadRequest("Label text is required.");

        var results = await _extractor.ExtractIndicationsAsync(labelText);
        return Ok(results);
    }
}
