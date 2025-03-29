using DrugIndications.Domain.Entities;
using DrugIndications.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using DrugIndications.Application.Services;
using Newtonsoft.Json;

namespace DrugIndications.API.Seed;

public static class DatabaseSeeder
{
    //public static async Task SeedAsync(IProgramRepository repository, ILogger logger, EligibilityParserService eligibilityParser)
    //{
    //    var existing = await repository.GetByIdAsync(11757);
    //    if (existing != null)
    //    {
    //        logger.LogInformation("Seed skipped: Program with ID 11757 already exists.");
    //        return;
    //    }

    //    var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Seed", "dupixent.json");
    //    var rawJson = await File.ReadAllTextAsync(jsonPath);

    //    var program = JsonConvert.DeserializeObject<DrugProgram>(rawJson);

    //    if (program?.Details?.Any() == true)
    //    {
    //        var eligibilityText = program.Details[0].Eligibility;
    //        var parsedRequirements = eligibilityParser.ParseFromTextAsync(eligibilityText);

    //        // Si no vienen requisitos ya, los agregamos
    //        if (program.Requirements == null || !program.Requirements.Any())
    //            program.Requirements = await parsedRequirements;
    //    }

    //    await repository.AddAsync(program);
    //    logger.LogInformation("Seeded program with ID 11757");
    //}
}
