using Abstractions.Services;
using Microsoft.Extensions.Logging;
using Models.GitHub;
using System.Net.Http;
using System.Text.Json;

namespace Services.API
{
    /// <summary>
    /// Service for interacting with GitHub repositories and releases
    /// </summary>
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GitHubService> _logger;

        private static readonly IReadOnlyList<string> _availableControllers = new List<string>
        {
            "ARCController",
            "Blank",
            "Pico",
            "PicoW",
            "Pico2",
            "BentoBox",
            "DuelPadZen",
            "FlatboxRev4",
            "FlatboxRev5",
            "FlatboxRev5RGB",
            "FlatboxRev5USBPassthrough",
            "FlatboxRev5Southpaw",
            "FlatboxRev8",
            "Granola",
            "KB2040",
            "KeyboardConverter",
            "Haute42COSMOX",
            "Haute42COSMOXMLite",
            "Haute42COSMOXMUltra",
            "Haute42COSMOXXAnalog",
            "Liatris",
            "MavercadeRev1",
            "MavercadeRev2",
            "MiSTercadeV2",
            "OpenCore0",
            "OpenCore0MIXUP",
            "OpenCore0WASD",
            "OSUMGP-RP2040",
            "PicoAnn",
            "PicoFightingBoard",
            "PXPGamepad",
            "ReflexCtrlGenesis6",
            "ReflexCtrlNES",
            "ReflexCtrlSaturn",
            "ReflexCtrlSNES",
            "ReflexCtrlVB",
            "ReflexEncodeV1.2",
            "ReflexEncodeV2.0",
            "RP2040AdvancedBreakoutBoard",
            "RP2040AdvancedBreakoutBoardUSBPassthrough",
            "RP2040MiniBreakoutBoard",
            "RP2040MiniBreakoutBoardUSBPassthrough",
            "SeeedXIAORP2040",
            "SparkFunProMicro",
            "SparkFunProMicroRP2350",
            "WaveshareZero",
            "Stress",
            "SGFDevices",
            "ZeroRhythm"
        }.AsReadOnly();

        private const string GitHubApiUrl = "https://api.github.com/repos/OpenStickCommunity/GP2040-CE/releases/tags/{0}";

        public GitHubService(HttpClient httpClient, ILogger<GitHubService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Set User-Agent header required by GitHub API
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Tony-Bot/1.0");
        }

        public async Task<string?> GetControllerUF2UrlAsync(string controllerName, string tag = "v0.7.11")
        {
            try
            {
                _logger.LogInformation("Fetching UF2 URL for controller: {ControllerName} with tag: {Tag}", controllerName, tag);

                var url = string.Format(GitHubApiUrl, tag);
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch GitHub release. Status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var release = JsonSerializer.Deserialize<GitHubRelease>(content);

                if (release?.Assets == null)
                {
                    _logger.LogWarning("No assets found in GitHub release for tag: {Tag}", tag);
                    return null;
                }

                // Look for the UF2 file for the specified controller
                var expectedFileName = $"GP2040-CE_{tag}_{controllerName}.uf2";
                var asset = release.Assets.FirstOrDefault(a => 
                    string.Equals(a.Name, expectedFileName, StringComparison.OrdinalIgnoreCase));

                if (asset == null)
                {
                    _logger.LogWarning("UF2 file not found for controller: {ControllerName}. Expected filename: {ExpectedFileName}", 
                        controllerName, expectedFileName);
                    return null;
                }

                _logger.LogInformation("Found UF2 file for {ControllerName}: {FileName}", controllerName, asset.Name);
                return asset.BrowserDownloadUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching UF2 URL for controller: {ControllerName}", controllerName);
                return null;
            }
        }

        public IReadOnlyList<string> GetAvailableControllers()
        {
            return _availableControllers;
        }
    }
}
