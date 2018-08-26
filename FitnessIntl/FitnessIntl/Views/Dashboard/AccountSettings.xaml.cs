using RevolutionarySHOP.ViewModels;
using System;
using Xamarin.Forms;

namespace FitnessIntl.Views
{
    public partial class AccountSettings : ContentPage//NavigationPage
    {
        //private readonly UserInformationContext context = new UserInformationContext();

        public AccountSettings()
        {
            var r  = new AccountSettingsViewModel()
            {
                Email = "TODO",
                Password = "",
                SecurityQuestion = "",
                SecurityAnswer = ""
            };

            InitializeComponent();

           // SaveChanges.Click += new RoutedEventHandler(SaveChanges_Click);
        }

        #region Control Event Handlers

        protected void OnNavigatedTo(NavigationEventArgs e)
        {
            // Setting the DataContext of the Parent panel control will ensure
            // that databinding works for all children using the current user

            //AccountSettingsPanel.BindingContext = Globals.CurrentUser;
        }

        private void SaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                //Globals.CurrentUser.email_address = Email.Text;
                //Globals.CurrentUser.security_question = SecurityQuestion.Text;

                //// Only update password and security answer if the fields have been changed

                //if (!String.IsNullOrEmpty(Password.Password))
                //    Globals.CurrentUser.password = Password.Password;

                //if (!String.IsNullOrEmpty(ConfirmPassword.Password))
                //    Globals.CurrentUser.confirm_password = ConfirmPassword.Password;

                //if (!String.IsNullOrEmpty(SecurityAnswer.Password))
                //    Globals.CurrentUser.security_answer = SecurityAnswer.Password;

                //context.UpdateUser(Globals.CurrentUser, updateUserCallback =>
                //    {
                //        // Clear any of the password fields upon completion

                //        Password.Password = "";
                //        ConfirmPassword.Password = "";
                //        SecurityAnswer.Password = "";

                //        MessageBox.Show("Your account settings have been successfully updated");

                //    }, null);
            }
            catch (Exception ex)
            {
                //Toast.MakeText(this,  ex.Message, ToastLength.Long);
            }
        }

        #endregion
    }
}
