using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FitnessTrackerPlus.Views.Exercise
{
    public partial class ExerciseLog : NavigationPage
    {
        private ExerciseContext context = new ExerciseContext();

        public ExerciseLog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                CardioLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, CardioLogGrid); };
                CardioLogGrid.RowEditEnded += new EventHandler<DataGridRowEditEndedEventArgs>(ExerciseLogGrid_RowEditEnded);
                CardioLogGrid.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(ExerciseLogGrid_PreparingCellForEdit);

                ActivityLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, ActivityLogGrid); };
                ActivityLogGrid.RowEditEnded += new EventHandler<DataGridRowEditEndedEventArgs>(ExerciseLogGrid_RowEditEnded);
                ActivityLogGrid.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(ExerciseLogGrid_PreparingCellForEdit);

                WeightTrainingLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, WeightTrainingLogGrid); };
                WeightTrainingLogGrid.RowEditEnded += new EventHandler<DataGridRowEditEndedEventArgs>(ExerciseLogGrid_RowEditEnded);

                Calendar.SelectedDate = Globals.SelectedDate;
                Calendar.SelectedDatesChanged += (se, ev) =>
                {
                    if (Calendar.SelectedDate.HasValue)
                        Globals.SelectedDate = Calendar.SelectedDate.Value;
                };

                ExerciseTypes.SelectionChanged += (se, ev) =>
                {
                    ExerciseType selectedType = ExerciseTypes.SelectedItem as ExerciseType;

                    if (selectedType.id > 0)
                    {
                        if (selectedType.type_name == "Weight Training")
                        {
                            MuscleGroups.Visibility = Visibility.Visible;
                            Exercises.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            context.Load<FitnessTrackerPlus.Web.Data.Exercise>(context.GetExercisesByTypeQuery(Globals.CurrentUser.id, selectedType.id),
                                LoadBehavior.RefreshCurrent, (ExercisesLoaded) =>
                                {
                                    if (!ExercisesLoaded.HasError)
                                        Exercises.Visibility = Visibility.Visible;

                                }, null);

                            MuscleGroups.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        MuscleGroups.Visibility = Visibility.Collapsed;
                        Exercises.Visibility = Visibility.Collapsed;
                    }
                };

                MuscleGroups.SelectionChanged += (se, ev) =>
                {
                    MuscleGroup selectedGroup = MuscleGroups.SelectedItem as MuscleGroup;

                    if (selectedGroup.id > 0)
                    {
                        context.Load<FitnessTrackerPlus.Web.Data.Exercise>(context.GetExercisesByMuscleGroupQuery(Globals.CurrentUser.id, selectedGroup.id),
                                    LoadBehavior.RefreshCurrent, (ExercisesLoaded) =>
                                    {
                                        if (!ExercisesLoaded.HasError)
                                            Exercises.Visibility = Visibility.Visible;

                                    }, null);
                    }
                    else
                        Exercises.Visibility = Visibility.Collapsed;
                };

                Exercises.SelectionChanged += (se, ev) =>
                {
                    FitnessTrackerPlus.Web.Data.Exercise selectedExercise = Exercises.SelectedItem as FitnessTrackerPlus.Web.Data.Exercise;

                    if (selectedExercise.id > 0)
                        CreateExerciseLogEntry(selectedExercise);
                };

                CustomExercise.Click += new RoutedEventHandler(CustomExercise_Click);
                DeleteSelected.Click += new RoutedEventHandler(DeleteSelected_Click);

                // Hold onto a copy of the ExerciseContext contained in the XAML

                context = this.Resources["ExerciseContext"] as ExerciseContext;

                Parameter user_id = new Parameter();
                user_id.ParameterName = "user_id";
                user_id.Value = Globals.CurrentUser.id;

                // Ensure that the user_id parameter is set for all DomainDataSource controls before the query is executed

                CardioData.QueryParameters.Add(user_id);
                WeightTrainingData.QueryParameters.Add(user_id);
                ActivityData.QueryParameters.Add(user_id);

                // Ensure that whenever data is being loaded into the DomainContext the ProgressBar control is visible
                // The ProgressBarVisibilityConverter will ensure that when IsLoading is true the Visibility is set to visible, false otherwise

                Binding binding = new Binding();
                binding.Source = context;
                binding.Path = new PropertyPath("IsLoading");
                binding.Converter = new ProgressBarVisibilityConverter();

                ProgressBar.SetBinding(ProgressBar.VisibilityProperty, binding);

                // Setup the ComboBox controls now that the ExerciseContext is available

                context.Load<ExerciseType>(context.GetExerciseTypesQuery(), LoadBehavior.RefreshCurrent, (ExerciseTypesLoaded) =>
                {
                    if (!ExerciseTypesLoaded.HasError)
                    {
                        ExerciseTypes.ItemsSource = ExerciseTypesLoaded.Entities;
                        ExerciseTypes.SelectedIndex = 0;
                    }

                }, null);

                context.Load<MuscleGroup>(context.GetMuscleGroupsQuery(), LoadBehavior.RefreshCurrent, (MuscleGroupsLoaded) =>
                {
                    if (!MuscleGroupsLoaded.HasError)
                    {
                        MuscleGroups.ItemsSource = MuscleGroupsLoaded.Entities;
                        MuscleGroups.SelectedIndex = 0;
                    }

                }, null);

                Exercises.ItemsSource = context.Exercises;
            };
        }

        #region Control Event Handlers

        private void ExerciseLogGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            ExerciseLogEntry entry = e.Row.DataContext as ExerciseLogEntry;
            TimePicker duration = grid.Columns[e.Column.DisplayIndex].GetCellContent(e.Row) as TimePicker;

            if (duration != null)
            {
                duration.Minimum = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                duration.Maximum = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                duration.PopupSecondsInterval = 1;
                duration.PopupMinutesInterval = 1;
                duration.Format = new CustomTimeFormat("HH:mm:ss");

                // As entries are created they will default to a duration of null, lets initialize the TimePicker to 00:00:00

                if (entry.duration == null)
                    duration.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                // Adjust the column width to fit the TimePicker control

                grid.Columns[e.Column.DisplayIndex].Width = new DataGridLength(duration.ActualWidth);
            }
        }

        private void CheckAll_Checked(object sender, RoutedEventArgs e)
        {
            DataGrid grid = DataGridHelper.GetParentGrid(sender as CheckBox);

            foreach (ExerciseLogEntry entry in grid.ItemsSource)
            {
                grid.SelectedItem = entry;
                CheckBox selectItem = grid.Columns[grid.Columns.Count - 1].GetCellContent(grid.SelectedItem) as CheckBox;

                if (selectItem != null)
                    selectItem.IsChecked = (sender as CheckBox).IsChecked;
            }
        }

        private void CustomExercise_Click(object sender, RoutedEventArgs e)
        {
            // Show a modal dialog with the create custom exercise form

            ChildWindow modalWindow = new ChildWindow();
            CustomExercise customExercise = new CustomExercise();

            customExercise.CustomExerciseCanceled += (s, ev) => { modalWindow.Close(); };
            customExercise.CustomExerciseCreated += (s, ev) =>
            {
                CreateExerciseLogEntry(ev.CreatedExercise);
                modalWindow.Close();
            };

            customExercise.DataContext = new FitnessTrackerPlus.Web.Data.Exercise();

            modalWindow.Title = "Add Custom Exercise";
            modalWindow.Content = customExercise;
            modalWindow.Show();
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            List<ExerciseLogEntry> entries = new List<ExerciseLogEntry>();

            // First check the cardio grid, then weight training and activities

            foreach (ExerciseLogEntry entry in context.ExerciseLogEntries)
            {
                CheckBox selectItem = null;

                if (entry.Exercise.ExerciseType.type_name == "Cardio")
                {
                    CardioLogGrid.SelectedItem = entry;
                    selectItem = CardioLogGrid.Columns[CardioLogGrid.Columns.Count - 1].GetCellContent(CardioLogGrid.SelectedItem) as CheckBox;
                }
                else if (entry.Exercise.ExerciseType.type_name == "Weight Training")
                {
                    WeightTrainingLogGrid.SelectedItem = entry;
                    selectItem = WeightTrainingLogGrid.Columns[WeightTrainingLogGrid.Columns.Count - 1].GetCellContent(WeightTrainingLogGrid.SelectedItem) as CheckBox;
                }
                else
                {
                    ActivityLogGrid.SelectedItem = entry;
                    selectItem = ActivityLogGrid.Columns[ActivityLogGrid.Columns.Count - 1].GetCellContent(ActivityLogGrid.SelectedItem) as CheckBox;
                }

                if (selectItem != null)
                    if (selectItem.IsChecked == true)
                        entries.Add(entry);
            }

            foreach (ExerciseLogEntry entry in entries)
                context.ExerciseLogEntries.Remove(entry);

            context.SubmitChanges((EntriesRemoved) =>
            {
                ProgressBar.Visibility = Visibility.Collapsed;

                CardioData.Load();
                WeightTrainingData.Load();
                ActivityData.Load();

            }, null);
        }

        private void ExerciseLogGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            // Submit any exercise log changes and refresh the DataGrid

            if (context.HasChanges)
                context.SubmitChanges();
        }

        #endregion

        #region Private Methods

        private void CreateExerciseLogEntry(FitnessTrackerPlus.Web.Data.Exercise exercise)
        {
            ExerciseLogEntry entry = new ExerciseLogEntry();

            // Setup new exercise log entry with selected date, exercise and current user

            entry.exercise_id = exercise.id;
            entry.entry_date = Globals.SelectedDate;
            entry.user_id = Globals.CurrentUser.id;

            context.ExerciseLogEntries.Add(entry);
            context.SubmitChanges((ChangesSubmitted) =>
            {
                if (!ChangesSubmitted.HasError)
                {
                    if (exercise.ExerciseType.type_name == "Cardio")
                        CardioData.Load();
                    else if (exercise.ExerciseType.type_name == "Weight Training")
                        WeightTrainingData.Load();
                    else
                        ActivityData.Load();
                }

            }, null);
        }

        #endregion
    }
}
