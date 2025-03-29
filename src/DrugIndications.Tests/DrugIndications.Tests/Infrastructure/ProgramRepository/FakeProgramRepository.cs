using DrugIndications.Domain.Entities;
using DrugIndications.Domain.Interfaces;

namespace DrugIndications.Tests.DrugIndications.Tests.Infrastructure.ProgramRepository;
public class FakeProgramRepository : IProgramRepository
{
    private readonly Dictionary<int, DrugProgram> _storage = new();

    public Task AddAsync(DrugProgram program)
    {
        _storage[program.Id] = program;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _storage.Remove(id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<DrugProgram>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<DrugProgram>>(_storage.Values.ToList());
    }

    public Task<DrugProgram?> GetByIdAsync(int id)
    {
        _storage.TryGetValue(id, out var program);
        return Task.FromResult(program);
    }

    public Task UpdateAsync(DrugProgram program)
    {
        _storage[program.Id] = program;
        return Task.CompletedTask;
    }
}
