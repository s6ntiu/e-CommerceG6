using Microsoft.AspNetCore.Mvc;
using Notifications.API.Data;
using Notifications.API.Models;
using ECommerce.Shared.Exceptions;
using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace Notifications.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationRepository _repository;

    public NotificationsController(NotificationRepository repository) { _repository = repository; } 

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(string userId) 
    {
        var notifs = await _repository.GetByUserIdAsync(userId);
        bool hasItems = false;
        foreach (var item in notifs)
        {
            hasItems = true;
            break;
        }

        if (!hasItems)
            throw new NotFoundException("NTF-003", "No se encontraron notificaciones para el usuario.");

        return Ok(notifs);
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendNotificationRequest request, [FromServices] IHttpClientFactory httpClientFactory)
    {
        if (string.IsNullOrWhiteSpace(request.Mensaje))
            throw new BusinessRuleException("NTF-002", "Los datos de la notificacion son invalidos.");

        // Validar UsuarioId con Users.API (NTF-001)
        var usersClient = httpClientFactory.CreateClient("UsersAPI");
        var userRes = await usersClient.GetAsync($"/api/users/{request.UsuarioId}");

        if (!userRes.IsSuccessStatusCode)
            throw new NotFoundException("NTF-001", "Usuario no encontrado.");

        var notification = new Notification
        {
            UsuarioId = request.UsuarioId,
            Mensaje = request.Mensaje,
            Tipo = request.Tipo,
            Estado = "Enviada"
        };

        await _repository.CreateAsync(notification);
        return StatusCode(201, notification);
    }
}