using Discord.Interactions;
using Services.Discord.SlashCommands;
using Xunit;

namespace Services.Tests.Discord.SlashCommands
{
    public class YeetTests
    {
        private readonly Yeet _yeetCommand;

        public YeetTests()
        {
            _yeetCommand = new Yeet();
        }

        [Fact]
        public void Constructor_InitializesSuccessfully()
        {
            // Act & Assert
            Assert.NotNull(_yeetCommand);
        }

        [Fact]
        public void YeetCommand_HasCorrectSlashCommandAttribute()
        {
            // Arrange
            var method = typeof(Yeet).GetMethod(nameof(Yeet.YeetAsync));

            // Act
            var attribute = method?.GetCustomAttributes(typeof(SlashCommandAttribute), false)
                .FirstOrDefault() as SlashCommandAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("yeet", attribute.Name);
            Assert.Equal("Yeet!", attribute.Description);
        }

        // Note: Testing the actual Discord interaction would require complex mocking
        // of Discord.NET's interaction context. The method itself is simple enough
        // that unit testing the core logic isn't necessary - integration tests would
        // be more valuable for slash commands.
    }
}
