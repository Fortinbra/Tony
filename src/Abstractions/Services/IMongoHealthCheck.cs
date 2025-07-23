namespace Abstractions.Services
{
    public interface IMongoHealthCheck
    {
        Task<bool> IsHealthyAsync();
        Task<string> GetHealthDetailsAsync();
    }
}
