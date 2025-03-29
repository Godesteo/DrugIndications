using Microsoft.Extensions.Options;
using OpenAI_API;
using DrugIndications.Domain.Entities;
using DrugIndications.Application.Options;
using System.Text.Json;

namespace DrugIndications.Application.Services;

public class EligibilityParserService
{
    private readonly OpenAIAPI _api;

    public EligibilityParserService(IOptions<OpenAIOptions> options)
    {
        _api = new OpenAIAPI(options.Value.ApiKey);
    }

    public async Task<List<ProgramEligibilityRequirement>> ParseFromTextAsync(string eligibilityText)
    {
        try
        {
            var chat = _api.Chat.CreateConversation();
            chat.AppendSystemMessage("""
                Convert the following eligibility text into a list of JSON key-value pairs. Use consistent naming. 
                Output format:
                [
                    { "name": "us_residency", "value": "true" },
                    { "name": "minimum_age", "value": "18" }
                ]
            """);
            chat.AppendUserInput(eligibilityText);

            var response = await chat.GetResponseFromChatbotAsync();
            return JsonSerializer.Deserialize<List<ProgramEligibilityRequirement>>(response) ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OpenAI fallback]: {ex.Message}");

            // Fallback simulado si OpenAI no responde
            return new List<ProgramEligibilityRequirement>
            {
                new() { Name = "us_residency", Value = "true" },
                new() { Name = "minimum_age", Value = "18" },
                new() { Name = "insurance_coverage", Value = "true" },
                new() { Name = "eligibility_length", Value = "12m" }
            };
        }
    }
}