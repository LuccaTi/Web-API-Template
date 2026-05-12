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
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var message = await _service.GetAllAsync(cancellationToken);
            return Ok(message);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            var request = new GetUserByIdRequestDto { Id = id };
            var message = await _service.GetByIdAsync(request, cancellationToken);
            return Ok(message);
        }
    }
}
