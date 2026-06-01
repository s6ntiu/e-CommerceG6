using System;
using System.Threading.Tasks;
using ECommerce.Shared.Exceptions;
using User.API.DTOs;
using User.API.Models;

namespace User.API.Services
{
    public interface IUserService
    {
        Task<Models.User> RegisterAsync(RegisterUserRequest request);
        Task<Models.User> LoginAsync(LoginRequest request);
    }

    public class UserService : IUserService
    {
        public async Task<Models.User> RegisterAsync(RegisterUserRequest request)
        {
            // Simulación de validación de email duplicado
            bool emailDuplicado = request.Email.Equals("duplicado@uba.ar", StringComparison.OrdinalIgnoreCase);

            if (emailDuplicado)
            {
                throw new BusinessRuleException("USR-001", $"El email '{request.Email}' ya se encuentra registrado.");
            }

            var newUser = new Models.User
            {
                Id = Guid.NewGuid(),
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                PasswordHash = request.Password, // TODO: Hashear con BCrypt
                FechaRegistration = DateTime.UtcNow,
                Activo = true,
                IntentosFallidos = 0
            };

            return await Task.FromResult(newUser);
        }

        public async Task<Models.User> LoginAsync(LoginRequest request)
        {
            var usuarioSimulado = new Models.User
            {
                Id = Guid.NewGuid(),
                Nombre = "Santi",
                Apellido = "Balo",
                Email = "santi@uba.ar",
                PasswordHash = "Password123!",
                Activo = true,
                IntentosFallidos = 0
            };

            if (!request.Email.Equals(usuarioSimulado.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessRuleException("USR-003", "Credenciales incorrectas.");
            }

            if (!usuarioSimulado.Activo && usuarioSimulado.IntentosFallidos >= 3)
            {
                throw new BusinessRuleException("USR-004", "Su cuenta fue bloqueada por alcanzar el máximo de intentos fallidos.");
            }

            bool passwordValida = request.Password == usuarioSimulado.PasswordHash;

            if (!passwordValida)
            {
                usuarioSimulado.IntentosFallidos++;

                if (usuarioSimulado.IntentosFallidos >= 3)
                {
                    usuarioSimulado.Activo = false;
                    throw new BusinessRuleException("USR-004", "Su cuenta ha sido bloqueada tras 3 intentos fallidos.");
                }

                throw new BusinessRuleException("USR-003", "Credenciales incorrectas.");
            }

            usuarioSimulado.IntentosFallidos = 0;
            return await Task.FromResult(usuarioSimulado);
        }
    }
}