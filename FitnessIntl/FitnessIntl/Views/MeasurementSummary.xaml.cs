using System;
using Xamarin.Forms;

namespace FitnessIntl.Views
{
    public partial class MeasurementSummary : ContentView, IDailySummary
    {
        //private MeasurementContext context = new MeasurementContext();

        public MeasurementSummary()
        {
            InitializeComponent();
        }

        #region IDailySummary Members

        public void LoadSummary(int user_id, DateTime summary_date)
        {
//            context.Load<DailyMeasurementSummary>(context.GetDailyMeasurementSummaryQuery(user_id, summary_date),
//            (SummaryLoaded) =>
//            {
//                if (!SummaryLoaded.HasError)
//                {
//                    IEnumerator<DailyMeasurementSummary> enumerator = SummaryLoaded.Entities.GetEnumerator();
//                    enumerator.MoveNext();

//                    this.DataContext = enumerator.Current;

//                    if (enumerator.Current.latest_image != null)
//                    {
//                        BitmapImage updatedImage = null;
//#if DEBUG
//                        updatedImage = new BitmapImage(new Uri(String.Format("http://localhost:1154/UploadedImages/{0}",
//                            enumerator.Current.latest_image), UriKind.Absolute));
//#else
//                                updatedImage = new BitmapImage(new Uri(String.Format("http://fitnesstrackerplus.com/UploadedImages/{0}",
//                                    SummaryLoaded.Value.latest_image), UriKind.Absolute));
//#endif

//                        updatedImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
//                        CurrentImage.Source = updatedImage;
//                    }
//                    else
//                        CurrentImage.Source = new BitmapImage(new Uri("/Images/image_unavailable.png", UriKind.Relative));
//                }

//            }, null);
        }

        #endregion
    }
}
