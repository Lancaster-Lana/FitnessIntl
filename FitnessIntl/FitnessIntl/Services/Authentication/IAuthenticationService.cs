using System.Threading.Tasks;

namespace FitnessIntl.Services.Authentication
{
	public interface IAuthenticationService
	{
		Task InitializeAsync();
		string GetAccessToken();
	}
}
