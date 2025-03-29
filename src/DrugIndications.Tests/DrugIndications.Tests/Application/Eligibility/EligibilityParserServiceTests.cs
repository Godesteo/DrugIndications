using DrugIndications.API.Controllers;
using DrugIndications.Application.DTOs;
using DrugIndications.Application.Services;
using DrugIndications.Application.Options;
using DrugIndications.Domain.Entities;
using DrugIndications.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;


namespace DrugIndications.Tests.DrugIndications.Tests.Application.Eligibility;
public class EligibilityParserServiceTests
{
    private readonly EligibilityParserService _service;

    public EligibilityParserServiceTests()
    {
        var mockOptions = Options.Create(new OpenAIOptions
        {
            ApiKey = "fake-key"
        });

        _service = new EligibilityParserService(mockOptions);
    }

    [Fact]
    public async Task ParseFromTextAsync_ReturnsRequirements_WhenInputIsValid()
    {
        // Arrange
        var text = "Must be a US resident over 18 with commercial insurance";

        // Act
        var result = await _service.ParseFromTextAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, r => r.Name == "us_residency");
        Assert.Contains(result, r => r.Name == "minimum_age");
        Assert.Contains(result, r => r.Name == "insurance_coverage");
    }

    [Fact]
    public async Task ParseFromTextAsync_ReturnsFallback_WhenErrorOccurs()
    {
        // Arrange
        var text = ""; // Cause fallback logic

        // Act
        var result = await _service.ParseFromTextAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }
}