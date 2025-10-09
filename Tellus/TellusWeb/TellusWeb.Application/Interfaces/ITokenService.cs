namespace TellusWeb.Application.Interfaces
{
    public interface ITokenService
    {
        string GetToken();
        void SetToken(string token);
        void ClearToken();
    }
}