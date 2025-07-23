namespace Abstractions.Services
{
    public interface IDiscordHealthCheck
    {
        Task<bool> IsHealthyAsync();
        Task<string> GetHealthDetailsAsync();
    }
}
