using FitnessIntl.Models;
using System;
using System.Linq;
using Xamarin.Forms;

namespace FitnessIntl.Views
{
    public partial class SignUpPage : ContentPage
    {
        AccountController accountController;

        public SignUpPage()
        {
            InitializeComponent();

            accountController = new AccountController();
        }

        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            var user = new User()
            {
                Username = usernameEntry.Text,
                Password = passwordEntry.Text,
                Email = emailEntry.Text
            };

            bool isRegistered = await accountController.SaveCredentials(user.Username, user.Password, user.Email);

            if (isRegistered)
            {
                var rootPage = Navigation.NavigationStack.FirstOrDefault();
                if (rootPage != null)
                {
                    App.IsUserLoggedIn = true;

                    var page = new MenuPage();
                    Navigation.InsertPageBefore(page, rootPage);
                    await Navigation.PopToRootAsync();
                }
            }
            else
            {
                messageLabel.Text = "Registration failed !";
            }
        }

        bool AreDetailsValid(User user)
        {
            return (!string.IsNullOrWhiteSpace(user.Username)
                && !string.IsNullOrWhiteSpace(user.Password)
                && !string.IsNullOrWhiteSpace(user.Email)
                && user.Email.Contains("@"));
        }
    }
}
