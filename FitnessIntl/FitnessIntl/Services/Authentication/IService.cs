using System.Threading.Tasks;

namespace FitnessIntl.Services.Authentication
{
    public interface IService
    {
        Task<string> GetEmailAsync(string tokenType, string accessToken);
    }
}