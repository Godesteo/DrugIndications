using DrugIndications.Application.Services;
using DrugIndications.Domain.Entities;

namespace DrugIndications.Infrastructure.Services;
public class EligibilityParser : IEligibilityParser
{
    public List<ProgramEligibilityRequirement> ParseFromText(string eligibilityText)
    {
        var results = new List<ProgramEligibilityRequirement>();

        if (eligibilityText.Contains("commercial insurance", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProgramEligibilityRequirement { Name = "insurance_coverage", Value = "true" });
        }

        if (eligibilityText.Contains("resident of the US", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProgramEligibilityRequirement { Name = "us_residency", Value = "true" });
        }

        if (eligibilityText.Contains("18", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProgramEligibilityRequirement { Name = "minimum_age", Value = "18" });
        }

        if (eligibilityText.Contains("18 months", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProgramEligibilityRequirement { Name = "eligibility_length", Value = "12m" }); // estimación
        }

        return results;
    }
}
