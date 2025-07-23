#nullable disable
using Discord;
using Tony.ServiceExtensions;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;

namespace Tony
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration.AddEnvironmentVariables();
            
            // Configure forwarded headers for Docker/reverse proxy scenarios
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor 
                    | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            
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

            // Configure forwarded headers for reverse proxy scenarios (like Docker)
            app.UseForwardedHeaders();

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            // Configure health check endpoint
            app.MapHealthChecks("/health");

            app.MapControllers();

            app.Run();

        }
    }
}