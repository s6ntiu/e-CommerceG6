using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notifications.API.Models;

namespace Notifications.API.Data
{
    public class NotificationRepository
    {
        private readonly IConfiguration _config;

        public NotificationRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqliteConnection CreateConnection() {
            return new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=notifications.db");
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId) // <-- string aquí
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<Notification>(
                "SELECT id as Id, usuario_id as UsuarioId, mensaje as Mensaje, tipo as Tipo, estado as Estado, fecha_creacion as FechaCreacion FROM notifications WHERE usuario_id = @UsuarioId",
                new { UsuarioId = userId });
        }

        public async Task<bool> CreateAsync(Notification notification)
        {
            using var conn = CreateConnection();
            var result = await conn.ExecuteAsync(
                "INSERT INTO notifications (id, usuario_id, mensaje, tipo, estado, fecha_creacion) VALUES (@Id, @UsuarioId, @Mensaje, @Tipo, @Estado, @FechaCreacion)",
                new
                {
                    Id = notification.Id,
                    UsuarioId = notification.UsuarioId,
                    Mensaje = notification.Mensaje,
                    Tipo = notification.Tipo,
                    Estado = notification.Estado,
                    FechaCreacion = notification.FechaCreacion.ToString("o")
                });
            return result > 0;
        }
    }
}