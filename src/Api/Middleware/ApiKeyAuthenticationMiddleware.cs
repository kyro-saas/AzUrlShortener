using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Cloud5mins.ShortenerTools.Api.Middleware
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
        private const string API_KEY_HEADER_NAME = "X-API-Key";

        public ApiKeyAuthenticationMiddleware(RequestDelegate next, ILogger<ApiKeyAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply authentication to specific protected endpoints
            var protectedEndpoints = new[]
            {
                "/api/UrlList",
                "/api/UrlCreate", 
                "/api/UrlUpdate",
                "/api/UrlArchive",
                "/api/UrlClickStatsByDay",
                "/api/UrlDataImport",
                "/api/UrlClickStatsImport"
            };

            if (protectedEndpoints.Any(endpoint => context.Request.Path.StartsWithSegments(endpoint)))
            {
                if (!IsValidApiKey(context))
                {
                    _logger.LogWarning("Unauthorized API request from {RemoteIpAddress} to {Path}", 
                        context.Connection.RemoteIpAddress, context.Request.Path);
                    
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: Invalid or missing API key");
                    return;
                }
            }

            await _next(context);
        }

        private bool IsValidApiKey(HttpContext context)
        {
            var providedApiKey = context.Request.Headers[API_KEY_HEADER_NAME].FirstOrDefault();
            
            if (string.IsNullOrEmpty(providedApiKey))
            {
                return false;
            }

            var expectedApiKey = Environment.GetEnvironmentVariable("API_KEY");
            
            if (string.IsNullOrEmpty(expectedApiKey))
            {
                _logger.LogError("API_KEY environment variable is not configured");
                return false;
            }

            return string.Equals(providedApiKey, expectedApiKey, StringComparison.Ordinal);
        }
    }
}
