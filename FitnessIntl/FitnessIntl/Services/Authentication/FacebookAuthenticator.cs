using System;
using Xamarin.Auth;

namespace FitnessIntl.Services.Authentication
{
    public class FacebookAuthenticator : IAuthenticator
    {
        private const string AuthorizeUrl = "https://www.facebook.com/dialog/oauth/";
        private const string RedirectUrl = "https://www.facebook.com/connect/login_success.html";
        
        private const bool IsUsingNativeUI = false;
        //private GetUsernameAsyncFunc GetUserName;
        private IAuthenticationDelegate _authenticationDelegate;

        private OAuth2Authenticator _auth;
        public OAuth2Authenticator GetAuthenticator()
        {
            return _auth;
        }

        public FacebookAuthenticator(string clientId, string scope, IAuthenticationDelegate authenticationDelegate)
        {
            _authenticationDelegate = authenticationDelegate;
            //GetUserName = async ( accountProperties) => { return await accountProperties.Keys.ToString() };

            _auth = new OAuth2Authenticator(clientId, scope,
                                            new Uri(AuthorizeUrl),
                                            new Uri(RedirectUrl),
                                            null, //GetUserName, 
                                            IsUsingNativeUI);

            _auth.Completed += OnAuthenticationCompleted;
            _auth.Error += OnAuthenticationFailed;
        }

        public void OnPageLoading(Uri uri)
        {
            _auth.OnPageLoading(uri);
        }

        private void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                var token = new  OAuthToken
                {
                    AccessToken = e.Account.Properties["access_token"]
                };
                _authenticationDelegate.OnAuthenticationCompleted(token);
            }
            else
            {
                _authenticationDelegate.OnAuthenticationCanceled();
            }
        }

        private void OnAuthenticationFailed(object sender, AuthenticatorErrorEventArgs e)
        {
            _authenticationDelegate.OnAuthenticationFailed(e.Message, e.Exception);
        }
    }
}
