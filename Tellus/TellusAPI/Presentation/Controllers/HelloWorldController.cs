using Microsoft.AspNetCore.Mvc;
using TellusAPI.Application.Interfaces;
using TellusAPI.Application.DTOs;

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

        [HttpGet("message")]
        public async Task<IActionResult> GetMessage()
        {
            var message = await _helloWorldService.GetHelloWorldMessage();
            return Ok(new { Message = message });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _helloWorldService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _helloWorldService.GetByIdAsync(id);
            if (item == null)
                return NotFound(new { Message = "Registro não encontrado" });
            
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHelloWorldDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
                return BadRequest(new { Message = "Nome é obrigatório" });

            var createdItem = await _helloWorldService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHelloWorldDto updateDto)
        {
            if (string.IsNullOrWhiteSpace(updateDto.Name))
                return BadRequest(new { Message = "Nome é obrigatório" });

            var updatedItem = await _helloWorldService.UpdateAsync(id, updateDto);
            if (updatedItem == null)
                return NotFound(new { Message = "Registro não encontrado" });

            return Ok(updatedItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _helloWorldService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { Message = "Registro não encontrado" });

            return NoContent();
        }
    }
}