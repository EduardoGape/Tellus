using TellusWeb.Domain.Entities.Reference;

namespace TellusWeb.Domain.DTOs
{
    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ProfileReference Profile { get; set; } = new ProfileReference();
    }
}