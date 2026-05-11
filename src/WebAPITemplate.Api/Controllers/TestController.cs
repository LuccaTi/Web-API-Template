using WebAPITemplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPITemplate.Application.DTOs.Requests;

namespace APITemplate.Host.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IUserService _service;

        public TestController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var message = await _service.GetAllAsync();
            return Ok(message);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var request = new GetUserByIdRequestDto { Id = id };
            var message = await _service.GetByIdAsync(request);
            return Ok(message);
        }
    }
}
