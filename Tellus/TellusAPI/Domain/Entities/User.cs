using TellusAPI.Domain.Entities.Reference;

namespace TellusAPI.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Profile como JSON
        public ProfileReference Profile { get; set; } = new ProfileReference();
    }
}
