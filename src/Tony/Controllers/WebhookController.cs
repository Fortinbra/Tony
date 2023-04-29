using Abstractions.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.GitHub;

namespace Tony.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IRepository<Root> _repository;
        public WebhookController(IRepository<Root> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Root root)
        {
            await _repository.CreateAsync(root);
            return Ok();
        }
    }
}
