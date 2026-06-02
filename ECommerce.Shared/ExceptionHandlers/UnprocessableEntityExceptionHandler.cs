using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ECommerce.Shared.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Shared.ExceptionHandlers
{
    public class UnprocessableEntityExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, System.Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not UnprocessableEntityException ex) return false;

            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                title = "Unprocessable Entity",
                status = 422,
                detail = "No se puede procesar la solicitud.",
                instance = context.Request.Path.Value,
                errorCode = ex.ErrorCode,
                errorMessage = ex.Message
            }, cancellationToken: cancellationToken);

            return true;
        }
    }
}
