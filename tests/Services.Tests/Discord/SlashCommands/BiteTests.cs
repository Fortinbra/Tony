using Discord;
using Discord.Interactions;
using Services.Discord.SlashCommands;
using Xunit;

namespace Services.Tests.Discord.SlashCommands
{
    public class BiteTests
    {
        private readonly Bite _biteCommand;

        public BiteTests()
        {
            _biteCommand = new Bite();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Constructor_InitializesSuccessfully()
        {
            // Act & Assert
            Assert.NotNull(_biteCommand);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void BiteCommand_HasCorrectSlashCommandAttribute()
        {
            // Arrange
            var method = typeof(Bite).GetMethod(nameof(Bite.BiteAsync));

            // Act
            var attribute = method?.GetCustomAttributes(typeof(SlashCommandAttribute), false)
                .FirstOrDefault() as SlashCommandAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("bite", attribute.Name);
            Assert.Equal("Bites a user", attribute.Description);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void BiteAsync_HasRequiredUserParameter()
        {
            // Arrange
            var method = typeof(Bite).GetMethod(nameof(Bite.BiteAsync));
            var parameters = method?.GetParameters();

            // Assert
            Assert.NotNull(parameters);
            Assert.Single(parameters);
            Assert.Equal(typeof(IUser), parameters[0].ParameterType);
            Assert.Equal("target", parameters[0].Name);
        }

        // Note: Testing the actual bite logic with random chance would require 
        // dependency injection for the Random class or making it testable.
        // The current implementation has a side effect (random behavior) that
        // makes it difficult to test deterministically without refactoring.
        // This is a good example of where TDD would have led to better design.
    }
}
