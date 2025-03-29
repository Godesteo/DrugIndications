using DrugIndications.Application.Options;
using DrugIndications.Application.Services;
using Microsoft.Extensions.Options;
using DrugIndications.Domain.Entities;

namespace DrugIndications.Tests.DrugIndications.Tests.Application.Indication;
public class IndicationExtractorServiceTests
{
    private readonly IndicationExtractorService _service;

    public IndicationExtractorServiceTests()
    {
        var options = Options.Create(new OpenAIOptions { ApiKey = "fake-key" });
        _service = new IndicationExtractorService(options);
    }

    [Fact]
    public async Task ExtractIndicationsAsync_ReturnsFallback_WhenApiFails()
    {
        // Arrange
        string input = "The patient has eczema and asthma.";

        // Act
        var result = await _service.ExtractIndicationsAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<DrugIndication>>(result);
        Assert.Contains(result, i => i.Condition.ToLower().Contains("eczema"));
        Assert.Contains(result, i => i.ICD10Code == "L20.9");
    }
}