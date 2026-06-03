using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace ECommerce.Shared.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeaderName = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId;

            if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out StringValues headerCorrelationId))
            {
                correlationId = headerCorrelationId.ToString();
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Append(CorrelationIdHeaderName, correlationId);
            }

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
                {
                    context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);
                }
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
