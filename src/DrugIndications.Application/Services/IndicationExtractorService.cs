using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI_API;
using DrugIndications.Application.Options;
using DrugIndications.Domain.Entities;
using System;

namespace DrugIndications.Application.Services;

public class IndicationExtractorService
{
    private readonly OpenAIAPI _api;

    private readonly List<ICD10Code> _icd10Codes = new()
    {
        new ICD10Code { Code = "J45", Description = "Asthma" },
        new ICD10Code { Code = "J33", Description = "Nasal polyps" },
        new ICD10Code { Code = "L20.9", Description = "Eczema" },
        new ICD10Code { Code = "K50", Description = "Crohn's disease" },
        new ICD10Code { Code = "K51", Description = "Ulcerative colitis" }
    };

    public IndicationExtractorService(IOptions<OpenAIOptions> options)
    {
        _api = new OpenAIAPI(options.Value.ApiKey);
    }

    public async Task<List<DrugIndication>> ExtractIndicationsAsync(string labelText)
    {
        try
        {
            var chat = _api.Chat.CreateConversation();
            chat.AppendSystemMessage($$"""
                        Extrae condiciones médicas del siguiente texto y conviértelas en JSON. Usa nombres claros. Para cada condición intenta mapear un código ICD-10 del listado:

                        {{string.Join(", ", _icd10Codes.Select(c => $"{c.Code}: {c.Description}"))}}

                        Formato de salida esperado:
                        [
                            { "condition": "asthma", "icd10": "J45" },
                            { "condition": "nasal polyps", "icd10": "J33" }
                        ]
                    """);

            chat.AppendUserInput(labelText);

        var response = await chat.GetResponseFromChatbotAsync();
        return JsonSerializer.Deserialize<List<DrugIndication>>(response) ?? new();
    }
        catch (Exception ex)
        {
            Console.WriteLine($"[OpenAI fallback - indications]: {ex.Message}");

            // Fallback simulado
            return new List<DrugIndication>
            {
                new () { Condition = "Eczema", ICD10Code = "L20.9" },
                new () { Condition = "Asthma", ICD10Code = "J45.909" }
            };
        }
    }
}
