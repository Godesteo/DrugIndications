using Microsoft.AspNetCore.Mvc;
using DrugIndications.Application.DTOs;
using DrugIndications.Application.Services;
using DrugIndications.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace DrugIndications.API.Controllers;

[ApiController]
[Route("eligibility")]
public class EligibilityController : ControllerBase
{
    private readonly IEligibilityParser _parser;

    public EligibilityController(IEligibilityParser parser)
    {
        _parser = parser;
    }

    /// <summary>
    /// Parses free-text eligibility into structured key-value pairs.
    /// </summary>
    /// <param name="request">The input text containing eligibility criteria in natural language.</param>
    /// <returns>A list of structured eligibility requirements.</returns>
    [HttpPost("parse")]
    [SwaggerOperation(
        Summary = "Parse eligibility text",
        Description = "Uses rule-based logic to extract structured key-value eligibility requirements from unstructured free-text input."
    )]
    [ProducesResponseType(typeof(List<ProgramEligibilityRequirement>), 200)]
    [ProducesResponseType(400)]
    public ActionResult<List<ProgramEligibilityRequirement>> Parse([FromBody] EligibilityTextRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text cannot be empty.");

        var requirements = _parser.ParseFromText(request.Text);
        return Ok(requirements);
    }
}
