namespace Models.GitHub
{
    /// <summary>
    /// Contains information about controller firmware including download and release notes URLs
    /// </summary>
    public class ControllerFirmwareInfo
    {
        /// <summary>
        /// Direct download URL for the UF2 firmware file
        /// </summary>
        public string DownloadUrl { get; set; } = string.Empty;

        /// <summary>
        /// URL to the GitHub release page with release notes
        /// </summary>
        public string ReleaseNotesUrl { get; set; } = string.Empty;

        /// <summary>
        /// The controller name
        /// </summary>
        public string ControllerName { get; set; } = string.Empty;

        /// <summary>
        /// The firmware version
        /// </summary>
        public string Version { get; set; } = string.Empty;
    }
}
