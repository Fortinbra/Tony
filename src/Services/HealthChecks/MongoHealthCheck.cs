using Abstractions.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Services.HealthChecks
{
    public class MongoHealthCheck : IHealthCheck, IMongoHealthCheck
    {
        private readonly IMongoDatabase _database;

        public MongoHealthCheck(IMongoDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var isHealthy = await IsHealthyAsync();
                var details = await GetHealthDetailsAsync();

                return isHealthy 
                    ? HealthCheckResult.Healthy($"MongoDB is connected. {details}")
                    : HealthCheckResult.Unhealthy($"MongoDB connection failed. {details}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"MongoDB health check failed: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                // Try to ping the database
                await _database.RunCommandAsync<object>("{ping:1}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetHealthDetailsAsync()
        {
            try
            {
                var result = await _database.RunCommandAsync<object>("{serverStatus:1}");
                return $"Database: {_database.DatabaseNamespace.DatabaseName}, Server responsive";
            }
            catch (Exception ex)
            {
                return $"Database: {_database.DatabaseNamespace.DatabaseName}, Error: {ex.Message}";
            }
        }
    }
}
