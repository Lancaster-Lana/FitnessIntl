using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FitnessTrackerPlus.Views.Food
{
    public partial class FoodLog : NavigationPage
    {
        private FoodContext context = new FoodContext();

        public FoodLog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                AutoComplete.ItemFilter += (se, evt) => { return true; };
                AutoComplete.Populating += new PopulatingEventHandler(AutoComplete_Populating);
                AutoComplete.DropDownOpened += (se, ev) => { SearchingText.Visibility = Visibility.Collapsed; };
                AutoComplete.GotFocus += (se, ev) => AutoComplete.Text = "";
                AutoComplete.SelectionChanged += (se, ev) =>
                {
                    if (AutoComplete.SelectedItem != null)
                        CreateFoodLogEntry(AutoComplete.SelectedItem as FitnessTrackerPlus.Web.Data.Food);

                    SearchingText.Visibility = Visibility.Collapsed;
                };

                AutoComplete.TextChanged += (se, ev) =>
                {
                    if (String.IsNullOrEmpty(AutoComplete.Text))
                        SearchingText.Visibility = Visibility.Collapsed;
                    else
                    {
                        SearchingText.Text = "Searching Foods...";
                        SearchingText.Visibility = Visibility.Visible;
                    }
                };

                FoodLogGrid.ItemsSource = context.FoodLogEntries;
                FoodLogGrid.RowEditEnded += new EventHandler<DataGridRowEditEndedEventArgs>(FoodLogGrid_RowEditEnded);
                FoodLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, FoodLogGrid); };

                Calendar.SelectedDatesChanged += (se, ev) =>
                {
                    if (Calendar.SelectedDate.HasValue)
                    {
                        Globals.SelectedDate = Calendar.SelectedDate.Value;
                        LoadFoodLog();
                    }
                };

                CustomFood.Click += new RoutedEventHandler(CustomFood_Click);
                DeleteSelected.Click += new RoutedEventHandler(DeleteSelected_Click);
            };
        }

        #region Control Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadFoodLog();
        }

        private void CheckAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (FoodLogEntry entry in context.FoodLogEntries)
            {
                FoodLogGrid.SelectedItem = entry;
                CheckBox selectItem = FoodLogGrid.Columns[FoodLogGrid.Columns.Count - 1].GetCellContent(FoodLogGrid.SelectedItem) as CheckBox;

                if (selectItem != null)
                    selectItem.IsChecked = (sender as CheckBox).IsChecked;
            }
        }

        private void CustomFood_Click(object sender, RoutedEventArgs e)
        {
            // Show a modal dialog with the create custom food form

            ChildWindow modalWindow = new ChildWindow();
            CustomFood customFood = new CustomFood();

            customFood.CustomFoodCanceled += (s, ev) => { modalWindow.Close(); };
            customFood.CustomFoodCreated += (s, ev) =>
            {
                CreateFoodLogEntry(ev.CreatedFood);
                modalWindow.Close();
            };

            modalWindow.Title = "Add Custom Food";
            modalWindow.Content = customFood;
            modalWindow.Show();
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            List<FoodLogEntry> entries = new List<FoodLogEntry>();
            ProgressBar.Visibility = Visibility.Visible;

            foreach (FoodLogEntry entry in context.FoodLogEntries)
            {
                FoodLogGrid.SelectedItem = entry;

                CheckBox selectItem = FoodLogGrid.Columns[FoodLogGrid.Columns.Count - 1].GetCellContent(FoodLogGrid.SelectedItem) as CheckBox;

                if (selectItem != null)
                    if (selectItem.IsChecked == true)
                        entries.Add(entry);
            }

            foreach (FoodLogEntry entry in entries)
                context.FoodLogEntries.Remove(entry);

            context.SubmitChanges((EntriesRemoved) => { ProgressBar.Visibility = Visibility.Collapsed; }, null);
        }

        private void AutoComplete_Populating(object sender, PopulatingEventArgs e)
        {
            // Search for matching foods and make sure that the ServingSize information is returned

            FoodContext autocompleteContext = new FoodContext();

            AutoComplete.ItemsSource = autocompleteContext.Foods;

            autocompleteContext.Load<FitnessTrackerPlus.Web.Data.Food>(autocompleteContext.SearchFoodsQuery(Globals.CurrentUser.id, Globals.MaxAutoCompleteResults, AutoComplete.Text, true),
                LoadBehavior.RefreshCurrent,
                (FoodsLoaded) =>
                {
                    if (!FoodsLoaded.HasError)
                    {
                        AutoComplete.PopulateComplete();

                        if (FoodsLoaded.TotalEntityCount == 0)
                            SearchingText.Text = "No foods found";
                    }

                }, null);

            e.Cancel = true;
        }

        private void FoodLogGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            // Submit any food log changes and refresh the DataGrid

            if (context.HasChanges)
            {
                ProgressBar.Visibility = Visibility.Visible;
                context.SubmitChanges((ChangesSubmitted) => { ProgressBar.Visibility = Visibility.Collapsed; }, null);
            }
        }

        #endregion

        #region Private Methods

        private void LoadFoodLog()
        {
            ProgressBar.Visibility = Visibility.Visible;

            context = new FoodContext();
            FoodLogGrid.ItemsSource = context.FoodLogEntries;

            context.Load<FoodLogEntry>(context.GetFoodLogEntriesQuery(Globals.CurrentUser.id, Globals.SelectedDate, true),
                (EntriesLoaded) => { ProgressBar.Visibility = Visibility.Collapsed; }, null);
        }

        private void CreateFoodLogEntry(FitnessTrackerPlus.Web.Data.Food food)
        {
            FoodLogEntry entry = new FoodLogEntry();

            ProgressBar.Visibility = Visibility.Visible;

            // Setup new food log entry with selected date, food and current user

            entry.food_id = food.id;
            entry.entry_date = Globals.SelectedDate;
            entry.user_id = Globals.CurrentUser.id;
            entry.servings = 1;

            context.FoodLogEntries.Add(entry);
            context.SubmitChanges((ChangesSubmitted) =>
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                LoadFoodLog();
            }, null);
        }

        #endregion
    }
}
