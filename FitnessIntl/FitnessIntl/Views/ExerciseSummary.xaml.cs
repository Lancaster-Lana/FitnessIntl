using System;
using Xamarin.Forms;

namespace FitnessIntl.Views
{
    public partial class ExerciseSummary : ContentView, IDailySummary
    {
        //private ExerciseContext context = new ExerciseContext();

        public ExerciseSummary()
        {
            InitializeComponent();
        }

        #region IDailySummary Members

        public void LoadSummary(int user_id, DateTime summary_date)
        {
            //context.Load<DailyExerciseSummary>(context.GetDailyExerciseSummaryQuery(user_id, summary_date),
            //(SummaryLoaded) =>
            //{
            //    if (!SummaryLoaded.HasError)
            //    {
            //        IEnumerator<DailyExerciseSummary> enumerator = SummaryLoaded.Entities.GetEnumerator();
            //        enumerator.MoveNext();

            //        this.DataContext = enumerator.Current;

            //        // Create a new KeyValuePair list with the data for the Pie chart

            //        List<KeyValuePair<string, double?>> data = new List<KeyValuePair<string, double?>>();

            //        data.Add(new KeyValuePair<string, double?>("Cardio", enumerator.Current.calories_from_cardio));
            //        data.Add(new KeyValuePair<string, double?>("Weight Training", enumerator.Current.calories_from_weight_training));
            //        data.Add(new KeyValuePair<string, double?>("Activities", enumerator.Current.calories_from_activities));

            //        ExerciseSummaryChart.DataContext = data;
            //    }

            //}, null);
        }

        #endregion
    }
}
