using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServConViz.Services;

namespace ServConViz
{
    public class IncomingRequestTrackerMiddleWare
    {
        private readonly RequestDelegate _next;

        public IncomingRequestTrackerMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        // IMessageWriter is injected into InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext, ILogger<IncomingRequestTrackerMiddleWare> logger, HttpRequestObserverService obsService)
        {
            if (obsService.options.ShowIncoming)
            {
                if (obsService.options.ShowInLog)
                    logger.LogInformation($"IncomingRequest {httpContext.Connection.RemoteIpAddress}:{httpContext.Connection.RemotePort} {httpContext.Request.Path}");

                var builder = new UriBuilder();
                builder.Host = httpContext.Connection.RemoteIpAddress?.ToString();
                builder.Port = httpContext.Connection.RemotePort;
                obsService.LogIncomingAsync(builder.Uri, new HttpMethod(httpContext.Request.Method));
            }

            await _next(httpContext);
        }
    }

    public static class IncomingRequestTrackerMiddleWareExtensions
    {
        public static IApplicationBuilder UseIncomingRequestTrackerMiddleWare(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<IncomingRequestTrackerMiddleWare>();
        }
    }
}
