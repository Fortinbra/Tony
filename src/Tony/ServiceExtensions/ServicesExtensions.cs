using Abstractions.Services;
using Services.API;
using System.Runtime.CompilerServices;

namespace Tony.ServiceExtensions
{
    public static class ServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
        }
    }
}
