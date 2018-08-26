using System;
using Xamarin.Auth;

namespace FitnessIntl.Services.Authentication
{
    public class GoogleAuthenticator : IAuthenticator
    {
        private const string AuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";

        private const string RedirectUrl = GoogleConfiguration.RedirectUrl;

        private const bool IsUsingNativeUI = false;

        private IAuthenticationDelegate _authenticationDelegate;

        private static OAuth2Authenticator _auth;
        public OAuth2Authenticator GetAuthenticator()
        {
            return _auth;
        }

        public GoogleAuthenticator(string clientId, string scope, IAuthenticationDelegate authenticationDelegate)
        {
            _authenticationDelegate = authenticationDelegate;

            //_auth = new OAuth2Authenticator(clientId, string.Empty, scope,
            //                                new Uri(AuthorizeUrl),
            //                                new Uri(RedirectUrl),
            //                                new Uri(AccessTokenUrl),
            //                                null, IsUsingNativeUI);
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
                var token = new OAuthToken
                {
                    TokenType = e.Account.Properties["token_type"],
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
