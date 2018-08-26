using Android.Views;
using System;
using Xamarin.Forms;

namespace FitnessIntl.Converters
{
    public class ProgressBarVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isLoading = (bool)value;

            return (isLoading == true ? ViewStates.Visible : ViewStates.Invisible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
