using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ECommerce.Shared.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Shared.ExceptionHandlers
{
    public class ForbiddenExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, System.Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ForbiddenException ex) return false;

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                title = "Forbidden",
                status = 403,
                detail = "Acceso denegado.",
                instance = context.Request.Path.Value,
                errorCode = ex.ErrorCode,
                errorMessage = ex.Message
            }, cancellationToken: cancellationToken);

            return true;
        }
    }
}
