using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using User.API.Data;
using User.API.Models;
using ECommerce.Shared.Exceptions;

namespace User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _repo;

        public UsersController(UserRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Validar si el email ya existe en la base de datos de SQLite
            var existingUser = await _repo.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new BusinessRuleException("USR-001", $"El email '{request.Email}' ya está registrado.");
            }

            // 2. Crear la entidad Usuario con la contraseña hasheada mediante BCrypt
            var newUser = new Usuario
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            // 3. Persistir en la base de datos
            await _repo.CreateAsync(newUser);

            // 4. Devolver respuesta 201 Created con los datos públicos del usuario
            return Created("", new
            {
                id = newUser.Id,
                nombre = newUser.Nombre,
                apellido = newUser.Apellido,
                email = newUser.Email,
                fechaRegistro = newUser.FechaRegistro,
                activo = newUser.Activo
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Buscar al usuario por email
            var user = await _repo.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new UnauthorizedException("USR-003", "Credenciales incorrectas.");
            }

            // 2. Verificar si la cuenta está bloqueada o suspendida
            if (!user.Activo)
            {
                if (user.IntentosFallidos >= 3)
                {
                    throw new ForbiddenException("USR-004", "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte.");
                }
                else
                {
                    throw new ForbiddenException("USR-005", "Su cuenta fue suspendida por razones de seguridad. Contacte a soporte.");
                }
            }

            // 3. Verificar contraseña con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.IntentosFallidos++;
                if (user.IntentosFallidos >= 3)
                {
                    user.Activo = false;
                }

                // Actualizar el contador de intentos y el estado en la base de datos
                await _repo.UpdateIntentosAsync(user.Id, user.IntentosFallidos, user.Activo);

                throw new UnauthorizedException("USR-003", "Credenciales incorrectas.");
            }

            // 4. Si el login es exitoso, resetear los intentos fallidos si tenía alguno
            if (user.IntentosFallidos > 0)
            {
                await _repo.UpdateIntentosAsync(user.Id, 0, true);
            }

            // 5. Devolver datos del usuario autenticado
            return Ok(new
            {
                id = user.Id,
                nombre = user.Nombre,
                apellido = user.Apellido,
                email = user.Email
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("USR-006", "Usuario no encontrado.");
            return Ok(new
            {
                id = user.Id,
                nombre = user.Nombre,
                apellido = user.Apellido,
                email = user.Email
            });
        }
    }
}