
using System;
using FitnessIntl.ViewModels;
using Xamarin.Forms;

namespace FitnessIntl.Views
{
    public partial class Announcements : ContentPage
    {
        //private readonly AnnouncementContext context = new AnnouncementContext();

        public AnnouncementsViewModel AnnouncementsViewModel { get; set; }// = new List<Announcement>();

        public Announcements()
        {
            InitializeComponent();
        }

        public Announcements(AnnouncementsViewModel announcementsViewModel)
        {
            AnnouncementsViewModel = announcementsViewModel;
            InitializeComponent();
            AnnouncementsList.ItemsSource = AnnouncementsViewModel.List;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //btnViewAnnoncement.Click += Announcement_Click;
        }

        #region Control Handlers

        public void Announcement_Click(object sender, EventArgs e)
        {
            //var announcement = ((sender as HyperlinkButton).DataContext) as Announcement;
            //var window = new ChildWindow();
            //var announcementText = new TextBlock();

            //if (announcement != null)
            //{
            //    // Display the annoucement in a modal child window
            //    // Ensure that the title is displayed using a Bold font

            //    announcementText.MaxWidth = 400;
            //    announcementText.TextWrapping = TextWrapping.Wrap;
            //    announcementText.Text = announcement.content;
            //    announcementText.Margin = new Thickness(0, 10, 0, 10);

            //    var title = new TextBlock();
            //    title.FontWeight = FontWeights.Bold;
            //    title.Text = "Important Site Announcement";

            //    window.Title = title;
            //    window.Content = announcementText;
            //    window.Show();
            //}
        }

        #endregion
    }
}
