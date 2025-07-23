using Abstractions.Services;
using Discord;
using Discord.Interactions;

namespace Services.Discord.SlashCommands
{
    /// <summary>
    /// Autocomplete handler for firmware controller selection
    /// </summary>
    public class FirmwareControllerAutocompleteHandler : AutocompleteHandler
    {
        private readonly IGitHubService _gitHubService;

        public FirmwareControllerAutocompleteHandler(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
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
