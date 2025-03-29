using Xunit;
using Moq;
using DrugIndications.API.Controllers;
using DrugIndications.Domain.Interfaces;
using DrugIndications.Domain.Entities;
using DrugIndications.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DrugIndications.Application.Options;

namespace DrugIndications.Tests.DrugIndications.Tests.API.Controllers.Programs;
public class ProgramsControllerTests
{
    private readonly Mock<IProgramRepository> _repoMock;
    private readonly EligibilityParserService _parserService;
    private readonly ProgramsController _controller;

    public ProgramsControllerTests()
    {
        _repoMock = new Mock<IProgramRepository>();
        var options = Options.Create(new OpenAIOptions { ApiKey = "fake-key" });
        _parserService = new EligibilityParserService(options);
        _controller = new ProgramsController(_repoMock.Object, _parserService);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var program = new DrugProgram { Id = 1, ProgramName = "Test" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(program);

        var result = await _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<DrugProgram>(ok.Value);
        Assert.Equal(1, data.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DrugProgram)null);

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        var program = new DrugProgram { Id = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(program);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DrugProgram)null);

        var result = await _controller.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenUpdated()
    {
        var program = new DrugProgram { Id = 1, ProgramName = "Test" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(program);

        var result = await _controller.Update(1, program);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        var program = new DrugProgram { Id = 2 };

        var result = await _controller.Update(1, program);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID in URL and body must match.", bad.Value);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenProgramMissing()
    {
        var program = new DrugProgram { Id = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DrugProgram)null);

        var result = await _controller.Update(1, program);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var program = new DrugProgram
        {
            Id = 1,
            ProgramName = "Test Program",
            Details = new List<ProgramDetail> { new ProgramDetail { Eligibility = "Must be US resident over 18" } }
        };

        var result = await _controller.Create(program);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var returned = Assert.IsType<DrugProgram>(created.Value);
        Assert.Equal(1, returned.Id);
    }
}
