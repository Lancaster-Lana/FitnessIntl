using System;
using Xamarin.Forms;

namespace FitnessTrackerPlus.Views.Food
{
    public partial class CustomFood : ContentView
    {
        private FoodContext foodContext = new FoodContext();

        public delegate void CustomFoodCreatedEventHandler(object sender, CustomFoodCreatedEventArgs e);
        public event CustomFoodCreatedEventHandler CustomFoodCreated;
        public event EventHandler CustomFoodCanceled;

        public CustomFood()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                CustomFoodForm.CommandButtonsVisibility = DataFormCommandButtonsVisibility.Commit | DataFormCommandButtonsVisibility.Cancel;
                CustomFoodForm.EditEnded += new EventHandler<DataFormEditEndedEventArgs>(CustomFoodForm_EditEnded);
                CustomFoodForm.CurrentItem = new FitnessTrackerPlus.Web.Data.Food
                {
                    user_id = Globals.CurrentUser.id
                };
            };
        }

        #region Control Event Handlers

        private void CustomFoodForm_EditEnded(object sender, DataFormEditEndedEventArgs e)
        {
            if (e.EditAction == DataFormEditAction.Cancel && CustomFoodCanceled != null)
                CustomFoodCanceled(this, null);
            else
            {
                if (CustomFoodForm.ValidateItem())
                {
                    // If validation succeeds then add the food to the database

                    foodContext.Foods.Add(CustomFoodForm.CurrentItem as FitnessTrackerPlus.Web.Data.Food);
                    foodContext.SubmitChanges((FoodSubmitted) =>
                    {
                        if (!FoodSubmitted.HasError && CustomFoodCreated != null)
                        {
                            CustomFoodCreated(this, new CustomFoodCreatedEventArgs(CustomFoodForm.CurrentItem as FitnessTrackerPlus.Web.Data.Food));
                        }

                    }, null);
                }
            }
        }

        #endregion

        public class CustomFoodCreatedEventArgs
        {
            private FitnessTrackerPlus.Web.Data.Food custom_food = null;

            public CustomFoodCreatedEventArgs() { }
            public CustomFoodCreatedEventArgs(FitnessTrackerPlus.Web.Data.Food custom_food)
            {
                this.custom_food = custom_food;
            }

            public FitnessTrackerPlus.Web.Data.Food CreatedFood { get { return custom_food; } }
        }
    }
}
