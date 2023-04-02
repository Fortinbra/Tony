using Abstractions.Repositories;
using Models.Users;
using MongoDB.Driver;
using Repositories.Mongo;

namespace Tony.ServiceExtensions
{
    public static class RepositoryExtension
    {
        public static void RegisterMongoDB(this IServiceCollection services, IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            services.AddSingleton(sp => client.GetDatabase(config["MongoSettings:DatabaseName"]));
        }
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IRepository<User>, Repository<User>>();
        }
    }
}
