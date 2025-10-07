using Microsoft.AspNetCore.Mvc;
using TellusAPI.Application.Interfaces;
using TellusAPI.Application.DTOs;

namespace TellusAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FunctionController : ControllerBase
    {
        private readonly IFunctionService _functionService;

        public FunctionController(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var functions = await _functionService.GetAllAsync();
            return Ok(functions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var function = await _functionService.GetByIdAsync(id);
            if (function == null)
                return NotFound(new { Message = "Record not found" });

            return Ok(function);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFunctionDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
                return BadRequest(new { Message = "Name is required" });

            var created = await _functionService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFunctionDto updateDto)
        {
            if (string.IsNullOrWhiteSpace(updateDto.Name))
                return BadRequest(new { Message = "Name is required" });

            var updated = await _functionService.UpdateAsync(id, updateDto);
            if (updated == null)
                return NotFound(new { Message = "Record not found" });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _functionService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { Message = "Record not found" });

            return NoContent();
        }
    }
}
