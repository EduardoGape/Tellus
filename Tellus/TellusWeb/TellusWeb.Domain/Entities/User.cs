using TellusWeb.Domain.Entities.Reference;

namespace TellusWeb.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ProfileReference Profile { get; set; } = new ProfileReference();
    }
}