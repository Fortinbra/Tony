using Discord.Interactions;
using Services.Discord.SlashCommands;
using Xunit;

namespace Services.Tests.Discord.SlashCommands
{
    public class ColorGrantorTests
    {
        private readonly ColorGrantor _colorGrantorCommand;

        public ColorGrantorTests()
        {
            _colorGrantorCommand = new ColorGrantor();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Constructor_InitializesSuccessfully()
        {
            // Act & Assert
            Assert.NotNull(_colorGrantorCommand);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GrantColorCommand_HasCorrectSlashCommandAttribute()
        {
            // Arrange
            var method = typeof(ColorGrantor).GetMethod(nameof(ColorGrantor.GrantColorAsync));

            // Act
            var attribute = method?.GetCustomAttributes(typeof(SlashCommandAttribute), false)
                .FirstOrDefault() as SlashCommandAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("givecolor", attribute.Name);
            Assert.Equal("So you want a color?", attribute.Description);
        }

        // Note: Testing the actual role assignment logic would require complex mocking
        // of Discord.NET's Context, Guild, User, and Role objects. The command directly
        // accesses Context.User, Context.Guild.Roles which makes it tightly coupled to
        // Discord.NET infrastructure.
        // 
        // Following TDD principles, this command should be refactored to:
        // 1. Accept dependencies through constructor injection
        // 2. Separate role management logic into a service
        // 3. Make the Discord interaction a thin orchestration layer
        //
        // Example refactor:
        // public class ColorGrantor : InteractionModuleBase
        // {
        //     private readonly IRoleService _roleService;
        //     
        //     public ColorGrantor(IRoleService roleService) => _roleService = roleService;
        //     
        //     [SlashCommand("givecolor", "So you want a color?")]
        //     public async Task GrantColorAsync()
        //     {
        //         await _roleService.GrantColorRoleAsync(Context.User, Context.Guild);
        //         await RespondAsync("Be careful what you wish for.");
        //     }
        // }
    }
}
