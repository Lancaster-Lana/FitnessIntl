using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FitnessIntl.Views
{
    public partial class FoodSummary : ContentView, IDailySummary
    {
        //public FoodContext context = new FoodContext();

        public FoodSummary()
        {
            InitializeComponent();
        }

        #region IDailySummary Members

        public void LoadSummary(int user_id, DateTime summary_date)
        {
            //context.Load<DailyFoodSummary>(context.GetDailyFoodSummaryQuery(user_id, summary_date),
            //(SummaryLoaded) =>
            //{
            //    if (!SummaryLoaded.HasError)
            //    {
            //        IEnumerator<DailyFoodSummary> enumerator = SummaryLoaded.Entities.GetEnumerator();
            //        enumerator.MoveNext();

            //        this.DataContext = enumerator.Current;

            //        // Load the food summary data for the pie chart

            //        context.Load<FoodSummaryData>(context.GetDailyFoodSummaryDataQuery(Globals.CurrentUser.id, Globals.SelectedDate),
            //            LoadBehavior.RefreshCurrent, (FoodSummaryLoaded) =>
            //            {
            //                if (!FoodSummaryLoaded.HasError)
            //                {
            //                    // Create a new KeyValuePair list with the data for the Pie chart

            //                    List<KeyValuePair<string, double?>> data = new List<KeyValuePair<string, double?>>();

            //                    foreach (FoodSummaryData food in FoodSummaryLoaded.Entities)
            //                        data.Add(new KeyValuePair<string, double?>(food.food_name, food.calories));

            //                    FoodSummaryChart.DataContext = data;
            //                }

            //            }, null);
            //    }

            //}, null);
        }

        #endregion
    }
}
