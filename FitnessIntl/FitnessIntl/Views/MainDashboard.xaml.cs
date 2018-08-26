using System;
using Xamarin.Forms;

namespace FitnessIntl.Views
{
    public partial class MainDashboard : ContentPage//NavigationPage
    {
        public MainDashboard()
        {
            InitializeComponent();
        }

        #region Control Event Handlers

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    // Refresh the site current date, site annoucements, exercise, food, and measurement summary controls

        //    Announcements.LoadAnnouncements();

        //    (FoodSummary as IDailySummary).LoadSummary(Globals.CurrentUser.id, DateTime.Now.Date);
        //    (ExerciseSummary as IDailySummary).LoadSummary(Globals.CurrentUser.id, DateTime.Now.Date);
        //    (MeasurementSummary as IDailySummary).LoadSummary(Globals.CurrentUser.id, DateTime.Now.Date);

        //    UserName.Text = String.Format("Welcome {0}", Globals.CurrentUser.email_address);
        //    CurrentDate.Text = DateTime.Now.ToLongDateString();
        //}

        #endregion
    }
}
