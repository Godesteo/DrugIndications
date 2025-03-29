using Microsoft.Data.SqlClient;
using DrugIndications.Domain.Entities;
using DrugIndications.Domain.Interfaces;

namespace DrugIndications.Infrastructure.Repositories;
public class ProgramRepository : IProgramRepository
{
    private readonly string _connectionString;

    public ProgramRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<DrugProgram?> GetByIdAsync(int id)
    {
        var query = "SELECT JsonData FROM Programs WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var json = reader.GetString(0);
            return System.Text.Json.JsonSerializer.Deserialize<DrugProgram>(json);
        }

        return null;
    }

    public async Task<IEnumerable<DrugProgram>> GetAllAsync()
    {
        var query = "SELECT JsonData FROM Programs";
        var results = new List<DrugProgram>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var json = reader.GetString(0);
            var program = System.Text.Json.JsonSerializer.Deserialize<DrugProgram>(json);
            if (program != null)
                results.Add(program);
        }

        return results;
    }

    public async Task AddAsync(DrugProgram program)
    {
        var query = "INSERT INTO Programs (JsonData) VALUES (@JsonData)";
        var json = System.Text.Json.JsonSerializer.Serialize(program);

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@JsonData", json);

        await command.ExecuteNonQueryAsync();
    }
    public async Task UpdateAsync(DrugProgram program)
    {
        var query = "UPDATE Programs SET JsonData = @JsonData WHERE Id = @Id";
        var json = System.Text.Json.JsonSerializer.Serialize(program);

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", program.Id);
        command.Parameters.AddWithValue("@JsonData", json);

        await command.ExecuteNonQueryAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var query = "DELETE FROM Programs WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync();
    }
}
