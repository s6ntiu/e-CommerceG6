using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notifications.API.Models;
using System.Linq;

namespace Notifications.API.Data
{
    public class NotificationRepository
    {
        private readonly IConfiguration _config;
        public NotificationRepository(IConfiguration config) => _config = config;
        private SqliteConnection CreateConnection() => new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=notifications.db");

        public async Task<List<NotificationDTO>> GetByUserIdAsync(string userId)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryAsync<NotificationDTO>(
                "SELECT id, usuario_id as UsuarioId, mensaje, tipo, estado, fecha_envio as FechaEnvio FROM notifications WHERE usuario_id = @UserId", 
                new { UserId = userId });
            return result.ToList();
        }

        public async Task CreateAsync(NotificationDTO notif)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "INSERT INTO notifications (id, usuario_id, mensaje, tipo, estado, fecha_envio) VALUES (@Id, @UsuarioId, @Mensaje, @Tipo, @Estado, @FechaEnvio)", 
                new { notif.Id, notif.UsuarioId, notif.Mensaje, notif.Tipo, notif.Estado, FechaEnvio = notif.FechaEnvio.ToString("O") });
        }
    }
}
