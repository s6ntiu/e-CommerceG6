using System;
using System.ComponentModel.DataAnnotations;

namespace Notifications.API.Models
{
    public class NotificationDTO
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
    }

    public class SendNotificationRequest
    {
        [Required] public string UsuarioId { get; set; } = string.Empty;
        [Required, MaxLength(500)] public string Mensaje { get; set; } = string.Empty;
        [Required] public string Tipo { get; set; } = string.Empty;
    }
}
