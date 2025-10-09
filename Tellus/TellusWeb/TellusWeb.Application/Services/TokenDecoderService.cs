using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.Entities;

namespace TellusWeb.Application.Services
{
    public class TokenDecoderService : ITokenDecoderService
    {
        private readonly ITokenService _tokenService;

        public TokenDecoderService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public CurrentUser? GetCurrentUser()
        {
            var token = _tokenService.GetToken();
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
                var profileId = jwtToken.Claims.FirstOrDefault(c => c.Type == "ProfileId")?.Value;
                var profileName = jwtToken.Claims.FirstOrDefault(c => c.Type == "ProfileName")?.Value;

                if (userId == null || profileId == null)
                    return null;

                return new CurrentUser
                {
                    Id = int.Parse(userId),
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "",
                    Name = userName ?? "",
                    ProfileId = int.Parse(profileId),
                    ProfileName = profileName ?? ""
                };
            }
            catch
            {
                return null;
            }
        }

        public int? GetUserId()
        {
            return GetCurrentUser()?.Id;
        }

        public string? GetUserName()
        {
            return GetCurrentUser()?.Name;
        }

        public int? GetProfileId()
        {
            return GetCurrentUser()?.ProfileId;
        }

        public string? GetProfileName()
        {
            return GetCurrentUser()?.ProfileName;
        }
    }
}