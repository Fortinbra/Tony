using Abstractions.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Services.HealthChecks
{
    public class DiscordHealthCheck : IHealthCheck, IDiscordHealthCheck
    {
        private readonly DiscordSocketClient _client;

        public DiscordHealthCheck(DiscordSocketClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var isHealthy = await IsHealthyAsync();
                var details = await GetHealthDetailsAsync();

                return isHealthy 
                    ? HealthCheckResult.Healthy($"Discord is connected. {details}")
                    : HealthCheckResult.Unhealthy($"Discord connection failed. {details}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Discord health check failed: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsHealthyAsync()
        {
            await Task.CompletedTask; // Make method async
            return _client.ConnectionState == ConnectionState.Connected && 
                   _client.LoginState == LoginState.LoggedIn;
        }

        public async Task<string> GetHealthDetailsAsync()
        {
            await Task.CompletedTask; // Make method async
            return $"Connection State: {_client.ConnectionState}, " +
                   $"Login State: {_client.LoginState}, " +
                   $"Latency: {_client.Latency}ms, " +
                   $"Guilds: {_client.Guilds.Count}";
        }
    }
}
