using Microsoft.AspNetCore.Builder;

namespace ASPNETCoreLvl2Demo.Middlewares
{
    public static class EndpointsLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseEndpointsLogging(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EndpointsLoggingMiddleware>();
        }
    }
}