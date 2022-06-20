using Microsoft.AspNetCore.Mvc;

namespace Barriot.Application.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync()
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
