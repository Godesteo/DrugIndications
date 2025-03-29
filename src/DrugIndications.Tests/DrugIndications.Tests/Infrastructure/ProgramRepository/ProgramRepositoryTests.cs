using DrugIndications.Domain.Entities;

namespace DrugIndications.Tests.DrugIndications.Tests.Infrastructure.ProgramRepository;
public class ProgramRepositoryTests
{
    private readonly FakeProgramRepository _repository;

    public ProgramRepositoryTests()
    {
        _repository = new FakeProgramRepository();
    }

    [Fact]
    public async Task AddAndGetById_Works()
    {
        var program = new DrugProgram { Id = 1, ProgramName = "Test Program" };

        await _repository.AddAsync(program);
        var fetched = await _repository.GetByIdAsync(1);

        Assert.NotNull(fetched);
        Assert.Equal("Test Program", fetched.ProgramName);
    }

    [Fact]
    public async Task Update_Works()
    {
        var program = new DrugProgram { Id = 2, ProgramName = "Initial" };
        await _repository.AddAsync(program);

        program.ProgramName = "Updated";
        await _repository.UpdateAsync(program);

        var updated = await _repository.GetByIdAsync(2);
        Assert.Equal("Updated", updated.ProgramName);
    }

    [Fact]
    public async Task Delete_Works()
    {
        var program = new DrugProgram { Id = 3, ProgramName = "To be deleted" };
        await _repository.AddAsync(program);
        await _repository.DeleteAsync(3);

        var deleted = await _repository.GetByIdAsync(3);
        Assert.Null(deleted);
    }
}

