using System;

namespace FitnessIntl.Services.Authentication
{
    public interface IAuthenticationDelegate
    {
        void OnAuthenticationCompleted(OAuthToken token);
        void OnAuthenticationFailed(string message, Exception exception);
        void OnAuthenticationCanceled();
    }
}
