using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreLvl2Demo.Middlewares
{
    public class EndpointsLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EndpointsLoggingMiddleware> _logger;

        public EndpointsLoggingMiddleware(RequestDelegate next, ILogger<EndpointsLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint is null)
            {
                return;
            }

            _logger.LogInformation($"Endpoint: {endpoint.DisplayName}");

            if (endpoint is RouteEndpoint routeEndpoint)
            {
                _logger.LogInformation("Endpoint has route pattern: " +
                                      routeEndpoint.RoutePattern.RawText);
            }

            foreach (var metadata in endpoint.Metadata)
            {
                _logger.LogInformation($"Endpoint has metadata: {metadata}");
            }

            await _next(context);
        }
    }
}