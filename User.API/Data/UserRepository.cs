using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using User.API.Models;

namespace User.API.Data
{
    public class UserRepository
    {
        private readonly IConfiguration _config;

        public UserRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqliteConnection CreateConnection() =>
            new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=user.db");

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            using var conn = CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Usuario>(
                "SELECT id as Id, nombre as Nombre, apellido as Apellido, email as Email, password_hash as PasswordHash, fecha_registro as FechaRegistro, activo as Activo, intentos_fallidos as IntentosFallidos FROM users WHERE email = @Email",
                new { Email = email });
        }

        public async Task<bool> CreateAsync(Usuario user)
        {
            using var conn = CreateConnection();
            var result = await conn.ExecuteAsync(
                "INSERT INTO users (id, nombre, apellido, email, password_hash, fecha_registro, activo, intentos_fallidos) VALUES (@Id, @Nombre, @Apellido, @Email, @PasswordHash, @FechaRegistro, @Activo, @IntentosFallidos)",
                user);
            return result > 0;
        }

        public async Task UpdateIntentosAsync(string id, int intentos, bool activo)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "UPDATE users SET intentos_fallidos = @Intentos, activo = @Activo, updated_at = datetime('now') WHERE id = @Id",
                new { Intentos = intentos, Activo = activo ? 1 : 0, Id = id });
        }
    }
}