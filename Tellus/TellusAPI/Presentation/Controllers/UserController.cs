using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TellusAPI.Application.Interfaces;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Common;
using TellusAPI.Domain.Entities;
using TellusAPI.Application.Filters;
using Microsoft.AspNetCore.Authorization;

namespace TellusAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = "Record not found" });

            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Message = "Name is required" });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { Message = "Email is required" });

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { Message = "Password is required" });

            var user = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Message = "Name is required" });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { Message = "Email is required" });

            var updated = await _userService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { Message = "Record not found" });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { Message = "Record not found" });

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<User>>> Search([FromQuery] UserFilter filter)
        {
            var result = await _userService.SearchAsync(filter);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto.Email, dto.Password);
            if (token == null)
                return Unauthorized(new { Message = "Invalid credentials" });

            return Ok(new { Token = token });
        }
    }
}
