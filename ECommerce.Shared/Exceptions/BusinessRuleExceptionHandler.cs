using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Products.API.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace Products.API.ExceptionHandlers
{
    public class BusinessRuleExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, System.Exception exception, CancellationToken cancellationToken)
        {
            // 1. Pregunta si la alarma es del tipo BusinessRuleException
            if (exception is not BusinessRuleException ex) return false;

            // 2. Si es, asigna el código de error HTTP 409 (Conflicto)
            context.Response.StatusCode = StatusCodes.Status409Conflict;

            // 3. Arma la caja de respuesta JSON con el formato que pide el TP
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.9",
                title = "Conflict",
                status = 409,
                detail = "No se puede procesar la solicitud debido a un conflicto de negocio.",
                instance = context.Request.Path.Value,
                errorCode = ex.ErrorCode,
                errorMessage = ex.Message
            }, cancellationToken: cancellationToken);

            return true;
        }
    }
}