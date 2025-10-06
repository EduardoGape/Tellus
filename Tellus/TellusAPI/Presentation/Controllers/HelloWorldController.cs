using Microsoft.AspNetCore.Mvc;
using TellusAPI.Application.Interfaces;

namespace TellusAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloWorldController : ControllerBase
    {
        private readonly IHelloWorldService _helloWorldService;

        public HelloWorldController(IHelloWorldService helloWorldService)
        {
            _helloWorldService = helloWorldService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var message = _helloWorldService.GetHelloWorldMessage();
            return Ok(new { Message = message });
        }
    }
}