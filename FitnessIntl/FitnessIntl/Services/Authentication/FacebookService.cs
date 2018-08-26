using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FitnessIntl.Services.Authentication
{
    public class FacebookService : IService
    {
        public async Task<string> GetEmailAsync(string tokenType, string accessToken)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email&access_token={accessToken}");
            var email = JsonConvert.DeserializeObject<FacebookEmail>(json);
            return email.Email;
        }
    }
}
