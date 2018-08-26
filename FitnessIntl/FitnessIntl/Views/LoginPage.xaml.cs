using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using FitnessIntl.Models;
using FitnessIntl.Services.Authentication;
using FitnessIntl.Services;
using Android.Widget;

namespace FitnessIntl.Views
{
    public class AccountController
    {
        private AccountStore store;

        public AccountController()
        {
            store = AccountStore.Create();
        }

        public async Task<bool> CheckAccount(string name, string password)
        {
            var account = await GetAccount(name);
            var isvalid = account != null ? account.Properties["Password"] == password : false;
            return isvalid;
        }

        public async Task<Account> GetAccount(string name)
        {
            var appAccounts = await store.FindAccountsForServiceAsync(App.AppName);
            var account = appAccounts.FirstOrDefault(a => a.Username == name);
            return account;
        }

        /// <summary>
        /// Register new or save old account
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task<bool> SaveCredentials(string userName, string password, string email)
        {
            if (AreDetailsValid(userName, password))
            {
                //Check if exist 
                var account = await GetAccount(userName);
                if (account != null)
                {
                    account.Properties["Password"] = password;
                    account.Properties["Email"] = email;
                }
                else
                {
                    account = new Account { Username = userName };
                    account.Properties.Add("Password", password);
                    account.Properties.Add("Email", email);
                }

                store.Save(account, App.AppName);

                return true;
            }

            return false;
        }

        public async void DeleteCredentials(string userName)
        {
            var account = await GetAccount(userName);
            if (account != null)
            {
                store.Delete(account, App.AppName);
            }
        }

        bool AreDetailsValid(string username, string password)
        {
            return (!string.IsNullOrWhiteSpace(username)
                && !string.IsNullOrWhiteSpace(password)
                //&& !string.IsNullOrWhiteSpace(email) && user.Email.Contains("@")
                );
        }

        /*
        public string UserName
        {
            get
            {
                var account = store.FindAccountsForService(App.AppName).FirstOrDefault();
                return (account != null) ? account.Username : null;
            }
        }

        public string Password
        {
            get
            {
                var account = store.FindAccountsForService(App.AppName).FirstOrDefault();
                return (account != null) ? account.Properties["Password"] : null;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            bool success = false;
            try
            {
                if (user != null)
                {
                    CookieManager.Instance.RemoveAllCookie();
                    await TodoItemManager.DefaultManager.CurrentClient.LogoutAsync();
                    CreateAndShowDialog(string.Format("You are now logged out - {0}", user.UserId), "Logged out!");
                }
                user = null;
                success = true;
            }
            catch (Exception ex)
            {
                CreateAndShowDialog(ex.Message, "Logout failed");
            }

            return success;
        } */
    }

    public partial class LoginPage : ContentPage, IAuthenticationDelegate
    {
        bool authenticated = false;

        AccountController accountController;

        public LoginPage()
        {
            InitializeComponent();

            accountController = new AccountController();

            //loginButton.Click += OnLoginButtonClicked;
            //gLoginButton.Click += OnGoogleLoginButtonClicked;
            //fLoginButton.Click += OnFacebookLoginButtonClicked;
        }

        #region Credentials store


        /// <summary>
        /// Check user in DB
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async Task<bool> AuthenticateAsync(string userName, string password)
        {
            bool success = false;
            try
            {
                success = await accountController.CheckAccount(userName, password);
                if (success)
                {
                    //CreateAndShowDialog(string.Format("You are now logged in - {0}", userName), "Succss!");
                }
            }
            catch (Exception ex)
            {
                //CreateAndShowDialog(ex.Message, "Authentication failed");
            }
            return success;
        }

        #endregion


        /// <summary>
        /// Login by password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var user = new User
            {
                Username = usernameEntry.Text,
                Password = passwordEntry.Text
            };

            authenticated = await AuthenticateAsync(user.Username, user.Password);
            LoadMainPage(authenticated);
        }

        void OnGoogleLoginButtonClicked(object sender, EventArgs e)
        {
            var auth = new GoogleAuthenticator(GoogleConfiguration.ClientId, GoogleConfiguration.Scope, this);

            // Display the activity handling the authentication
            StartSocialAuth(auth);
        }

        void OnFacebookLoginButtonClicked(object sender, EventArgs e)
        {
            var auth = new FacebookAuthenticator(FacebookConfiguration.ClientId, FacebookConfiguration.Scope, this);

            // Display the activity handling the authentication
            StartSocialAuth(auth);
        }

        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }


        #region


        private void StartSocialAuth(IAuthenticator auth)
        {
            var authenticator = auth.GetAuthenticator();
            var intent = authenticator.GetUI(Android.App.Application.Context);
            Forms.Context.StartActivity(intent);

            //var intent = new Intent();
            //intent.SetType("image/*");
            //intent.SetAction(Intent.ActionGetContent);
            //Forms.Context.StartActivity(Intent.CreateChooser(intent, "Select Picture"));

            //var intent = new Intent(Intent.ActionSend);
            //intent.SetType("message/rfc822");
            //intent.PutExtra(Intent.ExtraEmail, new[] { emailAddress });
            //Forms.Context.StartActivity(Intent.CreateChooser(intent, "Send email"));
        }

        public void OnAuthenticationCompleted(OAuthToken token)
        {
            authenticated = true;

            App.SaveToken(token.AccessToken);

            // Retrieve the user's email address
            //var service = new GoogleService();
            //var email = await service.GetEmailAsync(token.TokenType, token.AccessToken);
            LoadMainPageSocial(authenticated);
        }

        public void OnAuthenticationFailed(string message, Exception exception)
        {
            authenticated = false;
            ShowMessage(message, exception.Message);
        }

        public void OnAuthenticationCanceled()
        {
            authenticated = false;
            ShowMessage("Authentication canceled", "You cancelled the authentication process.");
        }

        #endregion

        async void LoadMainPage(bool isAuthenticated)
        {
            App.IsUserLoggedIn = isAuthenticated;

            if (isAuthenticated)
            {
                var page = new MenuPage();
                var rootPage = Navigation.NavigationStack.FirstOrDefault() ?? this;
                if (rootPage != null)
                {
                    Navigation.InsertPageBefore(page, rootPage);
                    await Navigation.PopToRootAsync(); //await Navigation.PopAsync(); 
                }
            }
            else
            {
                messageLabel.Text = "Login failed";
                passwordEntry.Text = string.Empty;

                ShowMessage("Login failed");
            }
        }

        async void LoadMainPageSocial(bool isAuthenticated)
        {
            App.IsUserLoggedIn = isAuthenticated;
            if (isAuthenticated)
            {

                App.SetMainPage(isAuthenticated);

                //await Navigation.PushAsync(new MenuPage()); //go to main page

                //Navigation.InsertPageBefore(new MenuPage(), this);
                //await Navigation.PopAsync();

                //DismissViewController(true, null);
                //NavigationController.PopViewControllerAnimated(true);

                //UIViewController authView = auth.GetUI() as UIViewController;
                //PresentViewController((authView, true, null);

            }
            else
            {
                //messageLabel.Text = "Login failed";
                //passwordEntry.Text = string.Empty;
                ShowMessage("Login failed");
            }
        }

        void ShowMessage(string title, string message = "")
        {
            //Toast.MakeText(this, message, ToastLength.Long).Show();
            this.DisplayAlert(message, message, "Cancel");
        }
    }
}
