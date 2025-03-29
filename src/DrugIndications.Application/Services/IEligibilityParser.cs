using DrugIndications.Domain.Entities;

namespace DrugIndications.Application.Services;
public interface IEligibilityParser
{
    List<ProgramEligibilityRequirement> ParseFromText(string eligibilityText);
}
