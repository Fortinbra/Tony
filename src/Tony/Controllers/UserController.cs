using Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tony.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _userService.GetUsers());
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByDiscordIdAsync(ulong Id)
        {
            return Ok(await _userService.GetUser(Id));
        }
    }
}
