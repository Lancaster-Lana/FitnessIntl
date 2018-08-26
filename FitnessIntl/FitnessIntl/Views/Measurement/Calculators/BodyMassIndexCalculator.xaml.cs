using System;
using Xamarin.Forms;

namespace FitnessTrackerPlus.Views.Measurement.Calculators
{
    public partial class BodyMassIndexCalculator : ContentView, IMeasurementCalculator
    {
        public BodyMassIndexCalculator()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                BodyMassIndexForm.CommandButtonsVisibility = DataFormCommandButtonsVisibility.Commit | DataFormCommandButtonsVisibility.Cancel;
                BodyMassIndexForm.EditEnded += BodyMassIndexForm_EditEnded;
                BodyMassIndexForm.CurrentItem = new BodyMassIndexParams { Height = null, Weight = null };
            };
        }

        #region Control Event Handers

        private void BodyMassIndexForm_EditEnded(object sender, DataFormEditEndedEventArgs e)
        {
            var units = BodyMassIndexForm.FindNameInContent("Units") as ComboBox;
            var heightText = BodyMassIndexForm.FindNameInContent("HeightText") as TextBox;
            var weightText = BodyMassIndexForm.FindNameInContent("WeightText") as TextBox;

            if (e.EditAction == DataFormEditAction.Cancel && CalculationCancelled != null)
                CalculationCancelled(this, null);
            else
            {
                // Calculate the approx body mass index formula is kg / m2
                // convert lbs to kg and ft to m if necessary first

                if (weightText != null)
                {
                    double weightValue = Convert.ToDouble(weightText.Text);
                    if (heightText != null)
                    {
                        double heightValue = Convert.ToDouble(heightText.Text);

                        if (units != null && (units.SelectedItem as ComboBoxItem).Content.ToString() == "Standard (lbs, in)")
                        {
                            heightValue *= 0.305;
                            weightValue *= 0.454;
                        }

                        // Body Mass Index is usually represented as an integer so cast the result
                        CalculatedValue = (int)(weightValue / Math.Pow(heightValue, 2));
                    }
                }

                if (CalculationComplete != null)
                    CalculationComplete(this, null);
            }
        }

        #endregion

        #region IMeasurementCalculator Members

        public event EventHandler CalculationComplete;
        public event EventHandler CalculationCancelled;

        public double CalculatedValue { get; set; }

        #endregion
    }

    public class BodyMassIndexParams
    {
        [Required]
        [Display(Name = "Height")]
        public double? Height { get; set; }

        [Required]
        [Display(Name = "Weight")]
        public double? Weight { get; set; }
    }
}
