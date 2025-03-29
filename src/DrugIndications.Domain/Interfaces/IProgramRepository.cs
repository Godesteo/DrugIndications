using DrugIndications.Domain.Entities;

namespace DrugIndications.Domain.Interfaces;
public interface IProgramRepository
{
    Task<DrugProgram?> GetByIdAsync(int id);
    Task<IEnumerable<DrugProgram>> GetAllAsync();
    Task AddAsync(DrugProgram program);
    Task UpdateAsync(DrugProgram program);
    Task DeleteAsync(int id);
}
