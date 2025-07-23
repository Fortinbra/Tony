using Abstractions.Services;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Services.Discord.SlashCommands
{
    /// <summary>
    /// Slash command for downloading GP2040-CE controller firmware files
    /// </summary>
    public class DownloadController : InteractionModuleBase
    {
        private readonly IGitHubService _gitHubService;
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(IGitHubService gitHubService, ILogger<DownloadController> logger)
        {
            _gitHubService = gitHubService;
            _logger = logger;
        }

        [SlashCommand("download-controller", "Download a UF2 firmware file for a GP2040-CE compatible controller")]
        public async Task DownloadControllerAsync(
            [Summary("controller", "Select the controller type"), Autocomplete(typeof(ControllerAutocompleteHandler))] 
            string controller,
            [Summary("version", "Firmware version (default: v0.7.11)")] 
            string version = "v0.7.11")
        {
            await DeferAsync(); // This operation might take a moment

            try
            {
                _logger.LogInformation("User {UserId} requested download for controller: {Controller}, version: {Version}", 
                    Context.User.Id, controller, version);

                // Validate controller name
                var availableControllers = _gitHubService.GetAvailableControllers();
                if (!availableControllers.Contains(controller, StringComparer.OrdinalIgnoreCase))
                {
                    await FollowupAsync(embed: new EmbedBuilder()
                        .WithTitle("❌ Invalid Controller")
                        .WithDescription($"Controller '{controller}' is not available. Use autocomplete to see valid options.")
                        .WithColor(Color.Red)
                        .Build());
                    return;
                }

                // Get the download URL
                var downloadUrl = await _gitHubService.GetControllerUF2UrlAsync(controller, version);

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    await FollowupAsync(embed: new EmbedBuilder()
                        .WithTitle("❌ File Not Found")
                        .WithDescription($"Could not find UF2 file for controller '{controller}' version '{version}'.\n\nThis might mean:\n• The version doesn't exist\n• The controller isn't supported in this version\n• There was a problem accessing GitHub")
                        .WithColor(Color.Red)
                        .Build());
                    return;
                }

                // Create success embed with download information
                var embed = new EmbedBuilder()
                    .WithTitle("🎮 Controller Firmware Download")
                    .WithDescription($"**Controller:** {controller}\n**Version:** {version}")
                    .WithColor(Color.Green)
                    .WithThumbnailUrl("https://raw.githubusercontent.com/OpenStickCommunity/GP2040-CE/main/docs/assets/images/gp2040-ce-logo.png")
                    .AddField("📥 Download Link", $"[Click here to download the UF2 file]({downloadUrl})", false)
                    .AddField("📋 Installation Instructions", 
                        "1. Hold the BOOTSEL button while connecting your controller to your computer\n" +
                        "2. Your controller should appear as a USB drive\n" +
                        "3. Drag and drop the downloaded UF2 file onto the USB drive\n" +
                        "4. The controller will automatically reboot with the new firmware", false)
                    .WithFooter("GP2040-CE - Open Source Gamepad Firmware")
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    .Build();

                await FollowupAsync(embed: embed);

                _logger.LogInformation("Successfully provided download link for {Controller} version {Version} to user {UserId}", 
                    controller, version, Context.User.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing download request for controller: {Controller}, version: {Version}", 
                    controller, version);

                await FollowupAsync(embed: new EmbedBuilder()
                    .WithTitle("❌ Error")
                    .WithDescription("An error occurred while processing your request. Please try again later.")
                    .WithColor(Color.Red)
                    .Build());
            }
        }
    }

    /// <summary>
    /// Autocomplete handler for controller selection
    /// </summary>
    public class ControllerAutocompleteHandler : AutocompleteHandler
    {
        private readonly IGitHubService _gitHubService;

        public ControllerAutocompleteHandler(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context, 
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter, 
            IServiceProvider services)
        {
            var userInput = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            var availableControllers = _gitHubService.GetAvailableControllers();

            // Filter controllers based on user input
            var suggestions = availableControllers
                .Where(controller => controller.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(25) // Discord limits autocomplete to 25 options
                .Select(controller => new AutocompleteResult(controller, controller))
                .ToList();

            return Task.FromResult(AutocompletionResult.FromSuccess(suggestions));
        }
    }
}
