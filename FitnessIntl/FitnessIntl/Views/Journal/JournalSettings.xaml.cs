
using Xamarin.Forms;

namespace FitnessTrackerPlus.Views.Journal
{
    public partial class JournalSettings : NavigationPage
    {
        public JournalSettings()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                ShareJournal.Checked += (se, ev) =>
                {
                    ShareFoods.IsChecked = true;
                    ShareExercises.IsChecked = true;
                    ShareMeasurements.IsChecked = true;
                    ShareImages.IsChecked = true;
                    EnableComments.IsChecked = true;
                };

                ShareJournal.Unchecked += (se, ev) =>
                {
                    ShareFoods.IsChecked = false;
                    ShareExercises.IsChecked = false;
                    ShareMeasurements.IsChecked = false;
                    ShareImages.IsChecked = false;
                    EnableComments.IsChecked = false;
                };

                ShareFoods.Checked += (se, ev) => { ShareJournal.IsChecked = true; };
                ShareExercises.Checked += (se, ev) => { ShareJournal.IsChecked = true; };
                ShareMeasurements.Checked += (se, ev) => { ShareJournal.IsChecked = true; };
                ShareImages.Checked += (se, ev) => { ShareJournal.IsChecked = true; };
                EnableComments.Checked += (se, ev) => { ShareJournal.IsChecked = true; };

                SaveChanges.Click += new RoutedEventHandler(SaveChanges_Click);
            };
        }

        #region Control Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            JournalSettingsPanel.DataContext = WebContext.Current.User;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            WebContext.Current.User.AboutText = AboutText.Text;
            WebContext.Current.User.ShareJournal = ShareJournal.IsChecked.Value;
            WebContext.Current.User.ShareFoods = ShareFoods.IsChecked.Value;
            WebContext.Current.User.ShareExercises = ShareExercises.IsChecked.Value;
            WebContext.Current.User.ShareMeasurements = ShareMeasurements.IsChecked.Value;
            WebContext.Current.User.ShareImages = ShareImages.IsChecked.Value;
            WebContext.Current.User.EnableComments = EnableComments.IsChecked.Value;
            WebContext.Current.User.EnableMySpace = EnableMySpace.IsChecked.Value;
            WebContext.Current.User.MySpaceName = MySpaceName.Text;

            WebContext.Current.Authentication.SaveUser((SettingsSaved) =>
            {
                if (!SettingsSaved.HasError)
                    MessageBox.Show("Your public journal settings have been successfully updated");

            }, null);
        }

        #endregion
    }
}
