using Abstractions.Services;
using Services.HealthChecks;
using System.Runtime.CompilerServices;

namespace Tony.ServiceExtensions
{
    public static class ServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            // Add health check services
            services.AddScoped<IMongoHealthCheck, MongoHealthCheck>();
            services.AddScoped<IDiscordHealthCheck, DiscordHealthCheck>();
        }
    }
}
