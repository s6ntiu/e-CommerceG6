using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Notifications.API.Data;
using Notifications.API.Models;
using ECommerce.Shared.Exceptions;
using System.Linq;

namespace Notifications.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationRepository _repo;

        public NotificationsController(NotificationRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Mensaje) || string.IsNullOrWhiteSpace(request.Tipo))
                throw new BusinessRuleException("NTF-002", "Los datos de la notificación son inválidos.");

            // TODO: Integración HTTP con Users API para validar que el usuario existe (NTF-001)
            // throw new NotFoundException("NTF-001", "El usuario destinatario no fue encontrado.");

            var newNotif = new NotificationDTO
            {
                Id = Guid.NewGuid().ToString(),
                UsuarioId = request.UsuarioId,
                Mensaje = request.Mensaje,
                Tipo = request.Tipo,
                Estado = "Enviada",
                FechaEnvio = DateTime.UtcNow
            };

            await _repo.CreateAsync(newNotif);

            return Created($"/api/notifications/{newNotif.UsuarioId}", newNotif);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetNotifications(string userId)
        {
            var notifs = await _repo.GetByUserIdAsync(userId);
            
            if (notifs == null || !notifs.Any())
                throw new NotFoundException("NTF-003", "No se encontraron notificaciones para el usuario.");

            return Ok(notifs);
        }
    }
}
