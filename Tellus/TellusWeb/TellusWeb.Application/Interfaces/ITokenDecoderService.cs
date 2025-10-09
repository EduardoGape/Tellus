using TellusWeb.Domain.Entities;

namespace TellusWeb.Application.Interfaces
{
    public interface ITokenDecoderService
    {
        CurrentUser? GetCurrentUser();
        int? GetUserId();
        string? GetUserName();
        int? GetProfileId();
        string? GetProfileName();
    }

    public class CurrentUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
    }
}