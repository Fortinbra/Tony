using Abstractions.Services;
using Services.API;
using Services.HealthChecks;
using System.Runtime.CompilerServices;

namespace Tony.ServiceExtensions
{
    public static class ServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            
            // Add health check services
            services.AddScoped<IMongoHealthCheck, MongoHealthCheck>();
            services.AddScoped<IDiscordHealthCheck, DiscordHealthCheck>();
        }
    }
}
