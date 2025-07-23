using Microsoft.AspNetCore.Mvc;
using Services.HealthChecks;
using Abstractions.Services;

namespace Tony.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IMongoHealthCheck _mongoHealthCheck;
    private readonly IDiscordHealthCheck _discordHealthCheck;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        IMongoHealthCheck mongoHealthCheck,
        IDiscordHealthCheck discordHealthCheck,
        ILogger<HealthController> logger)
    {
        _mongoHealthCheck = mongoHealthCheck;
        _discordHealthCheck = discordHealthCheck;
        _logger = logger;
    }

    /// <summary>
    /// Gets detailed health status of all system components
    /// </summary>
    /// <returns>Detailed health information for MongoDB and Discord connections</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailedHealthAsync()
    {
        try
        {
            var mongoHealthy = await _mongoHealthCheck.IsHealthyAsync();
            var mongoDetails = await _mongoHealthCheck.GetHealthDetailsAsync();
            
            var discordHealthy = await _discordHealthCheck.IsHealthyAsync();
            var discordDetails = await _discordHealthCheck.GetHealthDetailsAsync();

            var healthInfo = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Components = new
                {
                    MongoDB = new
                    {
                        Status = mongoHealthy ? "Healthy" : "Unhealthy",
                        Details = mongoDetails
                    },
                    Discord = new
                    {
                        Status = discordHealthy ? "Healthy" : "Unhealthy",
                        Details = discordDetails
                    }
                }
            };

            var overallHealthy = mongoHealthy && discordHealthy;
            
            return overallHealthy 
                ? Ok(healthInfo) 
                : StatusCode(503, healthInfo with { Status = "Degraded" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking system health");
            return StatusCode(500, new { Status = "Error", Message = "Unable to check system health" });
        }
    }

    /// <summary>
    /// Gets MongoDB connection health status
    /// </summary>
    /// <returns>MongoDB health information</returns>
    [HttpGet("mongodb")]
    public async Task<IActionResult> GetMongoHealthAsync()
    {
        try
        {
            var isHealthy = await _mongoHealthCheck.IsHealthyAsync();
            var details = await _mongoHealthCheck.GetHealthDetailsAsync();
            
            var healthInfo = new
            {
                Status = isHealthy ? "Healthy" : "Unhealthy",
                Details = details
            };

            return isHealthy ? Ok(healthInfo) : StatusCode(503, healthInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking MongoDB health");
            return StatusCode(500, new { Status = "Error", Message = "Unable to check MongoDB health" });
        }
    }

    /// <summary>
    /// Gets Discord connection health status
    /// </summary>
    /// <returns>Discord health information</returns>
    [HttpGet("discord")]
    public async Task<IActionResult> GetDiscordHealthAsync()
    {
        try
        {
            var isHealthy = await _discordHealthCheck.IsHealthyAsync();
            var details = await _discordHealthCheck.GetHealthDetailsAsync();
            
            var healthInfo = new
            {
                Status = isHealthy ? "Healthy" : "Unhealthy",
                Details = details
            };

            return isHealthy ? Ok(healthInfo) : StatusCode(503, healthInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Discord health");
            return StatusCode(500, new { Status = "Error", Message = "Unable to check Discord health" });
        }
    }
}
