using Abstractions.Repositories;
using Models;
using Models.GitHub;
using Models.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Repositories.Mongo;

namespace Tony.ServiceExtensions
{
    public static class RepositoryExtension
    {
        public static void RegisterMongoDB(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["MongoDB:ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured. Please set 'MongoDB:ConnectionString' in user secrets or appsettings.");
            }

            var databaseName = config["MongoDB:DatabaseName"];
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("MongoDB database name is not configured. Please set 'MongoDB:DatabaseName' in user secrets or appsettings.");
            }

            // Configure MongoDB serialization to handle BSON types properly
            ConfigureBsonSerialization();

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
            // Configure client settings to handle problematic BSON types
            clientSettings.ApplicationName = "TonyBot";
            
            var client = new MongoClient(clientSettings);
            services.AddSingleton(sp => client.GetDatabase(databaseName));
        }

        private static void ConfigureBsonSerialization()
        {
            // Only configure once to avoid duplicate registration errors
            if (!BsonClassMap.IsClassMapRegistered(typeof(Base)))
            {
                // Configure conventions for better serialization
                var conventionPack = new ConventionPack
                {
                    new CamelCaseElementNameConvention(),
                    new IgnoreExtraElementsConvention(true), // Ignore extra fields in MongoDB documents
                    new EnumRepresentationConvention(BsonType.String) // Store enums as strings
                };
                ConventionRegistry.Register("TonyBotConventions", conventionPack, t => true);

                // Configure class maps for better control - ignore extra elements including timestamps
                BsonClassMap.RegisterClassMap<Base>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });

                BsonClassMap.RegisterClassMap<User>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IRepository<User>, Repository<User>>();
            services.AddTransient<IRepository<Root>, Repository<Root>>();
        }
    }
}
