using Microsoft.AspNetCore.Mvc;
using DrugIndications.Domain.Interfaces;
using DrugIndications.Domain.Entities;
using DrugIndications.Application.Services;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Newtonsoft.Json;

namespace DrugIndications.API.Controllers;

[ApiController]
[Route("programs")]
public class ProgramsController : ControllerBase
{
    private readonly IProgramRepository _repository;
    private readonly EligibilityParserService _eligibilityParser;

    public ProgramsController(
        IProgramRepository repository, 
        EligibilityParserService eligibilityParser)
    {
        _repository = repository;
        _eligibilityParser = eligibilityParser;
    }

    [HttpPost("import-dupixent")]
    [SwaggerOperation(
    Summary = "Import Dupixent program from JSON file",
    Description = "Reads the dupixent.json file, extracts structured requirements using AI if needed, and stores the program in the database."
)]
    [ProducesResponseType(typeof(DrugProgram), 201)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ImportDupixentProgram()
    {
        try
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Seed", "dupixent.json");
            if (!System.IO.File.Exists(jsonPath))
                return NotFound("dupixent.json file not found.");

            var rawJson = await System.IO.File.ReadAllTextAsync(jsonPath);
            var program = JsonConvert.DeserializeObject<DrugProgram>(rawJson);

            if (program == null)
                return BadRequest("Failed to deserialize dupixent.json");

            // Usar EligibilityDetails si Requirements no existen
            if ((program.Requirements == null || !program.Requirements.Any()) &&
                !string.IsNullOrWhiteSpace(program.EligibilityDetails))
            {
                var parsed = await _eligibilityParser.ParseFromTextAsync(program.EligibilityDetails);
                program.Requirements = parsed;
            }

            await _repository.AddAsync(program);
            return CreatedAtAction(nameof(GetById), new { id = program.Id }, program);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Import failed: {ex.Message}");
        }
    }



    /// <summary>
    /// Retrieves a copay program by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the copay program.</param>
    /// <returns>The program details if found, otherwise 404.</returns>
    [Authorize]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get a program by ID",
        Description = "Retrieves a specific copay program and its structured details by program ID."
    )]
    [ProducesResponseType(typeof(DrugProgram), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var program = await _repository.GetByIdAsync(id);
        if (program == null)
            return NotFound();

        return Ok(program);
    }

    /// <summary>
    /// Creates a new drug copay program.
    /// </summary>
    /// <param name="program">Program object to create.</param>
    /// <returns>The created program with its ID.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new copay program",
        Description = "Creates a new drug copay program. If eligibility requirements are not provided, the service will try to extract them using generative AI."
    )]
    [ProducesResponseType(typeof(DrugProgram), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] DrugProgram program)
    {
        if ((program.Requirements == null || program.Requirements.Count == 0) &&
            program.Details?.FirstOrDefault()?.Eligibility is string eligibilityText &&
            !string.IsNullOrWhiteSpace(eligibilityText))
        {
            var parsed = await _eligibilityParser.ParseFromTextAsync(eligibilityText);
            program.Requirements = parsed;
        }

        await _repository.AddAsync(program);
        return CreatedAtAction(nameof(GetById), new { id = program.Id }, program);
    }

    /// <summary>
    /// Updates an existing copay program.
    /// </summary>
    /// <param name="id">Program ID to update.</param>
    /// <param name="program">Program object with new data.</param>
    /// <returns>No content if successful.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update an existing program",
        Description = "Updates an existing copay program by its ID. The ID in the URL must match the ID in the request body."
    )]
    public async Task<IActionResult> Update(int id, [FromBody] DrugProgram program)
    {
        if (id != program.Id)
            return BadRequest("ID in URL and body must match.");

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _repository.UpdateAsync(program);
        return NoContent();
    }

    /// <summary>
    /// Deletes a program by its ID.
    /// </summary>
    /// <param name="id">The program ID to delete.</param>
    /// <returns>No content if deleted.</returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete a program",
        Description = "Deletes a copay program by its ID."
    )]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
