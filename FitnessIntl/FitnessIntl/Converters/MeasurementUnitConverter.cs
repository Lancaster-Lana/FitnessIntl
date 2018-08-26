using System;
using Xamarin.Forms;

namespace FitnessTrackerPlus.Converters
{
    public class MeasurementUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var selected = value as MeasurementsUnits;

            return selected.MeasurementUnit;
        }
    }
}
