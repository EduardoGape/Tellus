using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TellusAPI.Application.Interfaces;
using TellusAPI.Application.DTOs;

namespace TellusAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _profileService.GetAllAsync();
            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var profile = await _profileService.GetByIdAsync(id);
            if (profile == null)
                return NotFound(new { Message = "Record not found" });

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Message = "Name is required" });

            var profile = await _profileService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = profile.Id }, profile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProfileDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Message = "Name is required" });

            var updated = await _profileService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { Message = "Record not found" });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _profileService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { Message = "Record not found" });

            return NoContent();
        }
    }
}
