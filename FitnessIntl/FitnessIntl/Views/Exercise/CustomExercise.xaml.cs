using System;
using Xamarin.Forms;

namespace FitnessTrackerPlus.Views.Exercise
{
    public partial class CustomExercise : ContentView
    {
        private ExerciseContext context = new ExerciseContext();
        private MuscleGroup selectedGroup = null;

        public delegate void CustomFoodCreatedEventHandler(object sender, CustomExerciseCreatedEventArgs e);
        public event CustomFoodCreatedEventHandler CustomExerciseCreated;
        public event EventHandler CustomExerciseCanceled;

        public CustomExercise()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                CustomExerciseForm.CommandButtonsVisibility = DataFormCommandButtonsVisibility.Commit | DataFormCommandButtonsVisibility.Cancel;
                CustomExerciseForm.EditEnded += new EventHandler<DataFormEditEndedEventArgs>(CustomExerciseForm_EditEnded);
                CustomExerciseForm.ContentLoaded += new EventHandler<DataFormContentLoadEventArgs>(CustomExerciseForm_ContentLoaded);
                CustomExerciseForm.CurrentItem = new FitnessTrackerPlus.Web.Data.Exercise { user_id = Globals.CurrentUser.id };
            };
        }

        #region Control Event Handers

        private void CustomExerciseForm_ContentLoaded(object sender, DataFormContentLoadEventArgs e)
        {
            ComboBox exerciseTypes = CustomExerciseForm.FindNameInContent("ExerciseTypes") as ComboBox;
            ComboBox muscleGroups = CustomExerciseForm.FindNameInContent("MuscleGroups") as ComboBox;

            context.Load<ExerciseType>(context.GetExerciseTypesQuery(), LoadBehavior.RefreshCurrent, (ExerciseTypesLoaded) =>
            {
                if (!ExerciseTypesLoaded.HasError)
                {
                    exerciseTypes.ItemsSource = ExerciseTypesLoaded.Entities;
                    exerciseTypes.SelectedIndex = 0;
                }

            }, null);

            context.Load<MuscleGroup>(context.GetMuscleGroupsQuery(), LoadBehavior.RefreshCurrent, (MuscleGroupsLoaded) =>
            {
                if (!MuscleGroupsLoaded.HasError)
                {
                    muscleGroups.ItemsSource = MuscleGroupsLoaded.Entities;
                    muscleGroups.SelectedIndex = 0;
                }

            }, null);

            exerciseTypes.SelectionChanged += (sev, eve) =>
            {
                ExerciseType selected = exerciseTypes.SelectedItem as ExerciseType;

                if (selected != null)
                {
                    if (selected.id > 0)
                    {
                        if (selected.type_name == "Weight Training")
                            muscleGroups.Visibility = Visibility.Visible;
                        else
                            muscleGroups.Visibility = Visibility.Collapsed;
                    }
                    else
                        muscleGroups.Visibility = Visibility.Collapsed;

                    // The exercise_type field is required so you must set this in order to pass validation

                    if (selected.id > 0)
                        (CustomExerciseForm.CurrentItem as FitnessTrackerPlus.Web.Data.Exercise).exercise_type = selected.id;
                }
            };

            muscleGroups.SelectionChanged += (sev, eve) => { selectedGroup = muscleGroups.SelectedItem as MuscleGroup; };
        }

        private void CustomExerciseForm_EditEnded(object sender, DataFormEditEndedEventArgs e)
        {
            if (e.EditAction == DataFormEditAction.Cancel && CustomExerciseCanceled != null)
                CustomExerciseCanceled(this, null);
            else
            {
                if (CustomExerciseForm.ValidateItem())
                {
                    // If validation succeeds then add the exercise to the database

                    context.Exercises.Add(CustomExerciseForm.CurrentItem as FitnessTrackerPlus.Web.Data.Exercise);
                    context.SubmitChanges((ExerciseSubmitted) =>
                    {
                        if (!ExerciseSubmitted.HasError)
                        {
                            // If the exercise was a weight training exercise we need to add an entry
                            // to the exercises_muscle_groups table

                            FitnessTrackerPlus.Web.Data.Exercise customExercise = CustomExerciseForm.CurrentItem as FitnessTrackerPlus.Web.Data.Exercise;

                            if (customExercise.ExerciseType.type_name == "Weight Training")
                            {
                                ExerciseMuscleGroup exerciseMuscleGroup = new ExerciseMuscleGroup { muscle_group_id = selectedGroup.id, exercise_id = customExercise.id };

                                context.ExerciseMuscleGroups.Add(exerciseMuscleGroup);
                                context.SubmitChanges((ExerciseMuscleGroupSubmitted) =>
                                {
                                    if (!ExerciseMuscleGroupSubmitted.HasError)
                                    {
                                        if (CustomExerciseCreated != null)
                                            CustomExerciseCreated(this, new CustomExerciseCreatedEventArgs(customExercise));
                                    }

                                }, null);
                            }
                            else
                            {
                                if (CustomExerciseCreated != null)
                                    CustomExerciseCreated(this, new CustomExerciseCreatedEventArgs(customExercise));
                            }
                        }

                    }, null);
                }
            }
        }

        #endregion
    }

    public class CustomExerciseCreatedEventArgs
    {
        private FitnessTrackerPlus.Web.Data.Exercise custom_exercise = null;

        public CustomExerciseCreatedEventArgs() { }
        public CustomExerciseCreatedEventArgs(FitnessTrackerPlus.Web.Data.Exercise custom_exercise)
        {
            this.custom_exercise = custom_exercise;
        }

        public FitnessTrackerPlus.Web.Data.Exercise CreatedExercise { get { return custom_exercise; } }
    }
}
