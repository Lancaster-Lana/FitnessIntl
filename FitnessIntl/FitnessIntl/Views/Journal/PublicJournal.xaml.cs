using System;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using FitnessTrackerPlus.Web.Data;
using FitnessTrackerPlus.Web.Services;
using FitnessTrackerPlus.Utility;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;

namespace FitnessTrackerPlus.Views.Journal
{
    public partial class PublicJournal : Page
    {
        private JournalContext journalContext = new JournalContext();
        private FoodContext foodContext = new FoodContext();
        private ExerciseContext exerciseContext = new ExerciseContext();
        private MeasurementContext measurementContext = new MeasurementContext();

        private PublicJournalSettings settings = null;

        public PublicJournal()
        {
            InitializeComponent();

            Globals.MainScroll.LayoutUpdated += (se, ev) => { PositionCommentControls(); };

            Calendar.SelectedDate = DateTime.Now;
            Calendar.SelectedDatesChanged += (se, ev) => { LoadPublicJournal(); };

            AboutText.Text = "";

            FoodLogGrid.ItemsSource = foodContext.FoodLogEntries;
            MeasurementLogGrid.ItemsSource = measurementContext.MeasurementLogEntries;

            Submit.Click += new RoutedEventHandler(Submit_Click);

            FoodLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, FoodLogGrid); };
            CardioLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, CardioLogGrid); };
            WeightTrainingLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, WeightTrainingLogGrid); };
            ActivityLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, ActivityLogGrid); };
            MeasurementLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, MeasurementLogGrid); };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                string user = NavigationContext.QueryString["user"];

                // Load the public journal settings for the requested user
                // if the public journal settings instance is null than that user has not elected
                // to share thier journal

                if (!String.IsNullOrEmpty(user))
                {
                    Title = String.Format("{0}'s Fitness Journal", user);

                    journalContext.Load<PublicJournalSettings>(journalContext.GetPublicJournalSettingsQuery(user),
                    (JournalLoaded) =>
                    {
                        if (!JournalLoaded.HasError)
                        {

                            IEnumerator<PublicJournalSettings> enumerator = JournalLoaded.Entities.GetEnumerator();
                            enumerator.MoveNext();

                            if (enumerator.Current != null)
                            {
                                // At this point we have a valid PublicJournal object so the user
                                // is currently sharing journal information now we need to look at 
                                // what options have been enabled

                                settings = enumerator.Current;
                                LoadPublicJournal();
                            }
                            else
                                NavigationService.Navigate(new Uri("Home", UriKind.Relative));
                        }
                        else
                            NavigationService.Navigate(new Uri("Home", UriKind.Relative));

                    }, null);
                }
                else
                    NavigationService.Navigate(new Uri("Home", UriKind.Relative));
            }
            catch (Exception)
            {
                NavigationService.Navigate(new Uri("Home", UriKind.Relative));
            }
        }

        #region Private Methods

        private void PositionCommentControls()
        {
            UIElement root = Application.Current.RootVisual as UIElement;
            GeneralTransform gt = Comment.TransformToVisual(root);
            Point pos = gt.Transform(new Point(0, 0));

            HtmlElement editor = HtmlPage.Document.GetElementById("comment_editor");
            HtmlElement comments = HtmlPage.Document.GetElementById("comment_area");

            editor.SetStyleAttribute("top", pos.Y.ToString() + "px");
            editor.SetStyleAttribute("left", pos.X.ToString() + "px");
            editor.SetStyleAttribute("width", Comment.ActualWidth.ToString() + "px");
            editor.SetStyleAttribute("height", Comment.ActualHeight.ToString() + "px");

            gt = CommentArea.TransformToVisual(root);
            pos = gt.Transform(new Point(0, 0));

            comments.SetStyleAttribute("top", pos.Y.ToString() + "px");
            comments.SetStyleAttribute("left", pos.X.ToString() + "px");
        }

        private void LoadPublicJournal()
        {
            if (settings != null)
            {
                if (settings.share_foods)
                {
                    foodContext.Load<FoodLogEntry>(foodContext.GetFoodLogEntriesQuery(settings.user_id, Calendar.SelectedDate.Value, false));
                    FoodLogGrid.Visibility = Visibility.Visible;
                }
                else
                    FoodLogGrid.Visibility = Visibility.Collapsed;

                if (settings.share_exercises)
                {
                    exerciseContext.Load<ExerciseLogEntry>(exerciseContext.GetExerciseLogEntriesQuery(Calendar.SelectedDate.Value, settings.user_id),
                        LoadBehavior.RefreshCurrent, (EntriesLoaded) =>
                        {
                            if (!EntriesLoaded.HasError)
                            {
                                CardioLogGrid.ItemsSource = EntriesLoaded.Entities.Where(ev => ev.Exercise.exercise_type == 1);
                                WeightTrainingLogGrid.ItemsSource = EntriesLoaded.Entities.Where(ev => ev.Exercise.exercise_type == 2);
                                ActivityLogGrid.ItemsSource = EntriesLoaded.Entities.Where(ev => ev.Exercise.exercise_type == 3);
                            }

                        }, null);

                    CardioLogGrid.Visibility = Visibility.Visible;
                    WeightTrainingLogGrid.Visibility = Visibility.Visible;
                    ActivityLogGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    CardioLogGrid.Visibility = Visibility.Collapsed;
                    WeightTrainingLogGrid.Visibility = Visibility.Collapsed;
                    ActivityLogGrid.Visibility = Visibility.Collapsed;
                }

                if (settings.share_measurements)
                {
                    measurementContext.Load<MeasurementLogEntry>(measurementContext.GetMeasurementLogEntriesQuery(settings.user_id, Calendar.SelectedDate.Value));
                    MeasurementLogGrid.Visibility = Visibility.Visible;
                }
                else
                    MeasurementLogGrid.Visibility = Visibility.Collapsed;

                if (settings.share_images)
                {
                    measurementContext.Load<MeasurementImage>(measurementContext.GetMeasurementImageQuery(settings.user_id, Calendar.SelectedDate.Value),
                        LoadBehavior.RefreshCurrent, (ImageLoaded) =>
                        {
                            if (!ImageLoaded.HasError)
                            {
                                BitmapImage updatedImage = null;
                                MeasurementImage currentImage = ImageLoaded.Entities.FirstOrDefault<MeasurementImage>();

                                if (currentImage != null)
                                {
#if DEBUG
                                    updatedImage = new BitmapImage(new Uri(String.Format("http://localhost:32490/UploadedImages/{0}",
                                        ImageLoaded.Entities.First<MeasurementImage>()), UriKind.Absolute));

#else
                                updatedImage = new BitmapImage(new Uri(String.Format("http://fitnesstrackerplus.com/UploadedImages/{0}",
                                    finalFileName), UriKind.Absolute));
#endif
                                    // This is necessary to ensure that Silverlight refreshes the image even though the file name remains the same

                                    updatedImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                                    CurrentImage.Source = updatedImage;
                                }
                                else
                                    CurrentImage.Source = new BitmapImage(new Uri("/Images/image_unavailable.png", UriKind.Relative));
                            }

                        }, null);

                    CurrentImage.Visibility = Visibility.Visible;
                }
                else
                    CurrentImage.Visibility = Visibility.Collapsed;

                if (settings.enable_comments)
                {
                    HtmlPage.Document.GetElementById("comment_area").SetStyleAttribute("display", "");
                    HtmlPage.Document.GetElementById("comment_editor").SetStyleAttribute("display", "");

                    LoadComments();

                    CommentForm.Visibility = Visibility.Visible;
                }
                else
                {
                    HtmlPage.Document.GetElementById("comment_area").SetStyleAttribute("display", "none");
                    HtmlPage.Document.GetElementById("comment_editor").SetStyleAttribute("display", "none");

                    CommentForm.Visibility = Visibility.Collapsed;
                }

                AboutText.Text = settings.about_text;
            }
        }

        private void CreateComment(JournalComment comment)
        {
            string headerText = "";

            StringBuilder builder = new StringBuilder();

            if (String.IsNullOrEmpty(comment.website))
                headerText = String.Format("Posted By: <a href='#'>{0}</a>", comment.name);
            else
            {
                if (comment.website.IndexOf("http://") >= 0)
                    headerText = String.Format("Posted By: <a href='{0}' target='_blank'>{1}</a>", comment.website, comment.name);
                else
                    headerText = String.Format("Posted By: <a href='http://{0}' target='_blank'>{1}</a>", comment.website, comment.name);
            }

            HtmlElement headerDiv = HtmlPage.Document.CreateElement("div");
            HtmlElement commentDiv = HtmlPage.Document.CreateElement("div");
            HtmlElement footerDiv = HtmlPage.Document.CreateElement("div");

            headerDiv.CssClass = "comment_header";
            commentDiv.CssClass = "comment_text";
            footerDiv.CssClass = "comment_footer";

            headerDiv.SetProperty("innerHTML", headerText);
            commentDiv.SetProperty("innerHTML", HttpUtility.HtmlDecode(comment.comment));
            footerDiv.SetProperty("innerHTML", String.Format("Posted on {0}", comment.entry_date.ToShortDateString()));

            HtmlElement comment_area = HtmlPage.Document.GetElementById("comment_area");

            comment_area.AppendChild(headerDiv);
            comment_area.AppendChild(commentDiv);
            comment_area.AppendChild(footerDiv);

            CommentArea.Width = Convert.ToInt32(HtmlPage.Document.GetElementById("comment_area").GetProperty("offsetWidth"));
            CommentArea.Height = Convert.ToInt32(HtmlPage.Document.GetElementById("comment_area").GetProperty("offsetHeight"));
        }

        private void LoadComments()
        {
            journalContext.Load<JournalComment>(journalContext.GetJournalCommentsQuery(settings.user_id), LoadBehavior.RefreshCurrent, (CommentsLoaded) =>
            {
                if (!CommentsLoaded.HasError)
                {
                    HtmlPage.Document.GetElementById("comment_area").SetProperty("innerHTML", "");

                    foreach (JournalComment comment in CommentsLoaded.Entities)
                        CreateComment(comment);
                }

            }, null);
        }

        #endregion

        #region Control Handlers

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            JournalComment comment = new JournalComment();

            comment.name = Name.Text;
            comment.website = Website.Text;

            comment.comment = HttpUtility.HtmlEncode(HtmlPage.Window.Invoke("getEditorContent", null) as string);
            comment.user_id = settings.user_id;
            comment.entry_date = DateTime.Now;

            journalContext.JournalComments.Add(comment);
            journalContext.SubmitChanges((CommentSubmitted) =>
            {
                if (!CommentSubmitted.HasError)
                {
                    LoadComments();

                    Name.Text = "";
                    Website.Text = "";
                    HtmlPage.Window.Invoke("setEditorContent", "");
                }

            }, null);
        }

        #endregion
    }
}
