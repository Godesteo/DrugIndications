using DrugIndications.API.Controllers;
using DrugIndications.Application.DTOs;
using DrugIndications.Application.Services;
using DrugIndications.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DrugIndications.Tests.DrugIndications.Tests.API.Controllers.Elegibilities;
public class EligibilityControllerTests
{
    [Fact]
    public void Parse_ReturnsBadRequest_WhenTextIsEmpty()
    {
        var parserMock = new Mock<IEligibilityParser>();
        var controller = new EligibilityController(parserMock.Object);

        var result = controller.Parse(new EligibilityTextRequest { Text = "" });

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Text cannot be empty.", bad.Value);
    }

    [Fact]
    public void Parse_ReturnsOk_WithParsedRequirements()
    {
        var parserMock = new Mock<IEligibilityParser>();
        parserMock.Setup(p => p.ParseFromText("Valid text")).Returns(new List<ProgramEligibilityRequirement>
            {
                new ProgramEligibilityRequirement { Name = "us_residency", Value = "true" }
            });

        var controller = new EligibilityController(parserMock.Object);

        var result = controller.Parse(new EligibilityTextRequest { Text = "Valid text" });

        var actionResult = Assert.IsType<ActionResult<List<ProgramEligibilityRequirement>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var list = Assert.IsType<List<ProgramEligibilityRequirement>>(okResult.Value);
        Assert.Single(list);
        Assert.Equal("us_residency", list[0].Name);
    }
}
