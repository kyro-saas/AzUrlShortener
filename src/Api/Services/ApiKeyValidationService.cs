namespace Cloud5mins.ShortenerTools.Api.Services
{
    public class ApiKeyValidationService : IApiKeyValidationService
    {
        private readonly ILogger<ApiKeyValidationService> _logger;
        private readonly string? _expectedApiKey;

        public ApiKeyValidationService(ILogger<ApiKeyValidationService> logger)
        {
            _logger = logger;
            _expectedApiKey = Environment.GetEnvironmentVariable("API_KEY");
            
            if (string.IsNullOrEmpty(_expectedApiKey))
            {
                _logger.LogWarning("API_KEY environment variable is not configured. API key authentication will fail.");
            }
        }

        public bool IsValidApiKey(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            if (string.IsNullOrEmpty(_expectedApiKey))
            {
                _logger.LogError("API_KEY environment variable is not configured");
                return false;
            }

            return string.Equals(apiKey, _expectedApiKey, StringComparison.Ordinal);
        }
    }
}
