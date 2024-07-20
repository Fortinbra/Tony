using Discord.Interactions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Services.Discord.SlashCommands
{
    public class GetLatest : InteractionModuleBase
    {
        private readonly HttpClient _httpClient;

        public GetLatest()
        {
            _httpClient = new HttpClient();
            // GitHub API versioning
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
            // Optional: If you need to authenticate e.g., for private repos
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your_github_token");
        }

        [SlashCommand("getlatest", "Get the latest release!")]
        public async Task GetLatestAsync()
        {
            string owner = "OpenStickCommunity"; // Repository owner's name
            string repo = "GP2040-CE"; // Repository name
            string url = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using (var jsonDoc = JsonDocument.Parse(content))
                {
                    var releaseUrl = jsonDoc.RootElement.GetProperty("html_url").GetString();
                    await RespondAsync(releaseUrl);
                }
            }
            else
            {
                await RespondAsync("Failed to fetch the latest release.");
            }
        }
    }
}
