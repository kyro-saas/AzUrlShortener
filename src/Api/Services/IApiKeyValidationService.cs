namespace Cloud5mins.ShortenerTools.Api.Services
{
    public interface IApiKeyValidationService
    {
        bool IsValidApiKey(string apiKey);
    }
}
