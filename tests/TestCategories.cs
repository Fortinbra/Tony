namespace Tests
{
    /// <summary>
    /// Constants for test categories used with xUnit Trait attributes.
    /// Use these constants to categorize tests for filtering and organization.
    /// </summary>
    public static class TestCategories
    {
        /// <summary>
        /// Unit tests - Fast running tests that test individual components in isolation
        /// with minimal dependencies (usually mocked).
        /// </summary>
        public const string Unit = "Unit";

        /// <summary>
        /// Integration tests - Tests that verify interactions between multiple components
        /// or systems, may include database connections, external APIs, etc.
        /// </summary>
        public const string Integration = "Integration";

        /// <summary>
        /// End-to-End tests - Full application tests that exercise the complete system
        /// from user interface to database, including external dependencies.
        /// </summary>
        public const string E2E = "E2E";

        /// <summary>
        /// Performance tests - Tests that measure and validate system performance,
        /// response times, throughput, etc.
        /// </summary>
        public const string Performance = "Performance";

        /// <summary>
        /// Smoke tests - Basic tests that verify core functionality is working
        /// after deployment or major changes.
        /// </summary>
        public const string Smoke = "Smoke";
    }
}
