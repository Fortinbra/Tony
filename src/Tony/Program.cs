#nullable disable
using Discord;
using Tony.ServiceExtensions;
using Scalar.AspNetCore;

namespace Tony
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration.AddEnvironmentVariables();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI with Scalar at https://github.com/scalar/scalar
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();
            builder.Services.RegisterMongoDB(builder.Configuration);
            builder.Services.AddRepositories();
            builder.Services.AddServices();
            builder.Services.AddDiscordBot(builder.Configuration);
            
            // Add health checks
            builder.Services.AddHealthChecks()
                .AddCheck<Services.HealthChecks.MongoHealthCheck>("mongodb")
                .AddCheck<Services.HealthChecks.DiscordHealthCheck>("discord");
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            // Configure health check endpoint
            app.MapHealthChecks("/health");

            app.MapControllers();

            app.Run();

        }
    }
}