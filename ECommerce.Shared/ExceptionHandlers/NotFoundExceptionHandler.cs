using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ECommerce.Shared.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Shared.ExceptionHandlers
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, System.Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not NotFoundException ex) return false;

            context.Response.StatusCode = StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(new
            {
                type = "[https://tools.ietf.org/html/rfc7231#section-6.5.4](https://tools.ietf.org/html/rfc7231#section-6.5.4)",
                title = "Not Found",
                status = 404,
                detail = "El recurso solicitado no fue encontrado.",
                instance = context.Request.Path.Value,
                errorCode = ex.ErrorCode,
                errorMessage = ex.Message
            }, cancellationToken: cancellationToken);

            return true;
        }
    }
}
