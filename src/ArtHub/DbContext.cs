using System.Data;
using ArtHub.Entities;
using Npgsql;
namespace ArtHub;

public class DbContext
{
    //TODO: убрать в переменные окружения
    private const string _connectionString = "Host=localhost;Port=5555;Username=postgres;Password=1029384756u;Database=arthub";
    private readonly NpgsqlConnection _dbConnection = new(_connectionString);
    
    public async Task<User> CreateUserAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        await _dbConnection.OpenAsync(cancellationToken);
        
        try
        {
            const string sqlQuery = @"INSERT INTO users (login, password, role) VALUES (@login, @password, @role) 
                RETURNING user_id;";
            var cmd = new NpgsqlCommand(sqlQuery, _dbConnection);
            cmd.Parameters.AddWithValue("login", login);
            cmd.Parameters.AddWithValue("password", password);
            cmd.Parameters.AddWithValue("role", "user");
            
            var result = await cmd.ExecuteScalarAsync(cancellationToken);

            return new User
            {
                UserId = Convert.ToInt32(result),
                Login = login,
                Password = password,
                Role = "user"
            };
        }
        finally
        {
            await _dbConnection.CloseAsync();
        }
    }
    
    public async Task<User> GetUserAsync(string login, CancellationToken cancellationToken = default)
    {
        await _dbConnection.OpenAsync(cancellationToken);

        try
        {
            const string sqlQuery = "SELECT * FROM users WHERE login = @login";
            var cmd = new NpgsqlCommand(sqlQuery, _dbConnection);
            cmd.Parameters.AddWithValue("login", login);

            var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return new User
                {
                    UserId = reader.GetInt64("user_id"),
                    Login = reader.GetString("login"),
                    Password = reader.GetString("password"),
                    Role = reader.GetString("role")
                };
            }
        }
        finally
        {
            await _dbConnection.CloseAsync();
        }

        return null;
    }
    

}