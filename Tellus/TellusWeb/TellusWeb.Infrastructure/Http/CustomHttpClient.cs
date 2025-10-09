using System.Net.Http.Headers;

namespace TellusWeb.Infrastructure.Http
{
    public class CustomHttpClient : HttpClient
    {
        private readonly string _baseUrl;

        public CustomHttpClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            BaseAddress = new Uri(baseUrl);
            DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public void SetBearerToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                DefaultRequestHeaders.Authorization = null;
            }
        }
    }
}