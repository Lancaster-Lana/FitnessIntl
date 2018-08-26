using Xamarin.Auth;

namespace FitnessIntl.Services.Authentication
{
    public interface IAuthenticator
    {
        OAuth2Authenticator GetAuthenticator();
    }
}