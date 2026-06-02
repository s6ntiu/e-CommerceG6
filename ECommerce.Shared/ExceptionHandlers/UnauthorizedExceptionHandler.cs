using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ECommerce.Shared.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Shared.ExceptionHandlers
{
    public class UnauthorizedExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, System.Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not UnauthorizedException ex) return false;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                title = "Unauthorized",
                status = 401,
                detail = "Autenticación requerida.",
                instance = context.Request.Path.Value,
                errorCode = ex.ErrorCode,
                errorMessage = ex.Message
            }, cancellationToken: cancellationToken);

            return true;
        }
    }
}
