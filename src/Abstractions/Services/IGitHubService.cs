using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Services
{
    /// <summary>
    /// Service for interacting with GitHub repositories and releases
    /// </summary>
    public interface IGitHubService
    {
        /// <summary>
        /// Gets the download URL for a UF2 file from the GP2040-CE releases
        /// </summary>
        /// <param name="controllerName">The name of the controller</param>
        /// <param name="tag">The release tag (default: v0.7.11)</param>
        /// <returns>The download URL for the UF2 file, or null if not found</returns>
        Task<string?> GetControllerUF2UrlAsync(string controllerName, string tag = "v0.7.11");

        /// <summary>
        /// Gets all available controller names for GP2040-CE
        /// </summary>
        /// <returns>List of available controller names</returns>
        IReadOnlyList<string> GetAvailableControllers();
    }
}
