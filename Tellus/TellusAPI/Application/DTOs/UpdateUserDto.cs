using TellusAPI.Domain.Entities.Reference;

namespace TellusAPI.Application.DTOs
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ProfileReference Profile { get; set; } = new ProfileReference();
    }
}