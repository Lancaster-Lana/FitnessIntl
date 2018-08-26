using System;
using Xamarin.Forms;

namespace FitnessTrackerPlus.Views.Measurement
{
    public partial class CustomMeasurement : ContentView
    {
        private MeasurementContext context = new MeasurementContext();
        private MeasurementUnit selectedUnit = null;

        public delegate void CustomMeasurementCreatedEventHandler(object sender, CustomMeasurementCreatedEventArgs e);
        public event CustomMeasurementCreatedEventHandler CustomMeasurementCreated;
        public event EventHandler CustomMeasurementCancelled;

        public CustomMeasurement()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                CustomMeasurementForm.CommandButtonsVisibility = DataFormCommandButtonsVisibility.Commit | DataFormCommandButtonsVisibility.Cancel;
                CustomMeasurementForm.EditEnded += CustomMeasurementForm_EditEnded;
                CustomMeasurementForm.ContentLoaded += CustomMeasurementForm_ContentLoaded;
                CustomMeasurementForm.CurrentItem = new Web.Data.Measurement { user_id = Globals.CurrentUser.id };
            };
        }

        #region Control Event Handers

        private void CustomMeasurementForm_ContentLoaded(object sender, DataFormContentLoadEventArgs e)
        {
            ComboBox measurementUnits = CustomMeasurementForm.FindNameInContent("MeasurementUnits") as ComboBox;
            DataField customUnitField = CustomMeasurementForm.FindNameInContent("CustomUnitField") as DataField;

            context.Load<MeasurementUnit>(context.GetMeasurementUnitsQuery(), LoadBehavior.RefreshCurrent,
                (MeasurementUnitsLoaded) =>
                {
                    if (!MeasurementUnitsLoaded.HasError)
                    {
                        measurementUnits.ItemsSource = MeasurementUnitsLoaded.Entities;
                        measurementUnits.SelectedIndex = 0;
                    }

                }, null);

            measurementUnits.SelectionChanged += (sev, eve) =>
            {
                MeasurementUnit selected = measurementUnits.SelectedItem as MeasurementUnit;

                if (selected != null)
                {
                    if (selected.id > 0)
                    {
                        customUnitField.Visibility = Visibility.Collapsed;
                        selectedUnit = selected;
                    }
                    else
                    {
                        customUnitField.Visibility = Visibility.Visible;
                        selectedUnit = null;
                    }
                }
            };
        }

        private void CustomMeasurementForm_EditEnded(object sender, DataFormEditEndedEventArgs e)
        {
            if (e.EditAction == DataFormEditAction.Cancel && CustomMeasurementCancelled != null)
                CustomMeasurementCancelled(this, null);
            else
            {
                if (CustomMeasurementForm.ValidateItem())
                {
                    // If validation succeeds then add the exercise to the database

                    context.Measurements.Add(CustomMeasurementForm.CurrentItem as FitnessTrackerPlus.Web.Data.Measurement);
                    context.SubmitChanges((MeasurementSubmitted) =>
                    {
                        if (!MeasurementSubmitted.HasError)
                        {
                            FitnessTrackerPlus.Web.Data.Measurement customMeasurement = CustomMeasurementForm.CurrentItem as FitnessTrackerPlus.Web.Data.Measurement;

                            // If user has selected an existing unit of measure create a new entry
                            // in the measurements_measurement_units table

                            if (selectedUnit != null)
                            {
                                context.MeasurementsUnits.Add(new MeasurementsUnits { unit_id = selectedUnit.id, measurement_id = customMeasurement.id });
                                context.SubmitChanges((MeasurementsUnitsSubmitted) =>
                                {
                                    if (!MeasurementsUnitsSubmitted.HasError)
                                    {
                                        if (CustomMeasurementCreated != null)
                                            CustomMeasurementCreated(this, new CustomMeasurementCreatedEventArgs(customMeasurement));
                                    }

                                }, null);
                            }
                            else
                            {
                                // Otherwise we also need to add an entry to the measurement_units table

                                TextBox customUnit = CustomMeasurementForm.FindNameInContent("CustomUnit") as TextBox;

                                context.MeasurementUnits.Add(new MeasurementUnit { unit = customUnit.Text, user_id = Globals.CurrentUser.id });
                                context.SubmitChanges((MeasurementUnitSubmitted) =>
                                {
                                    if (!MeasurementUnitSubmitted.HasError)
                                    {
                                        // Need to use the id of the newly created unit of measure

                                        MeasurementUnit createdUnit = MeasurementUnitSubmitted.ChangeSet.AddedEntities[0] as MeasurementUnit;

                                        context.MeasurementsUnits.Add(new MeasurementsUnits { unit_id = createdUnit.id, measurement_id = customMeasurement.id });
                                        context.SubmitChanges((MeasurementsUnitsSubmitted) =>
                                        {
                                            if (!MeasurementsUnitsSubmitted.HasError)
                                            {
                                                if (CustomMeasurementCreated != null)
                                                    CustomMeasurementCreated(this, new CustomMeasurementCreatedEventArgs(customMeasurement));
                                            }

                                        }, null);
                                    }

                                }, null);
                            }
                        }

                    }, null);
                }
            }
        }

        #endregion
    }

    public class CustomMeasurementCreatedEventArgs
    {
        private Web.Data.Measurement custom_measurement = null;

        public CustomMeasurementCreatedEventArgs() { }
        public CustomMeasurementCreatedEventArgs(Web.Data.Measurement custom_measurement)
        {
            this.custom_measurement = custom_measurement;
        }

        public Web.Data.Measurement CreatedMeasurement { get { return custom_measurement; } }
    }
}
