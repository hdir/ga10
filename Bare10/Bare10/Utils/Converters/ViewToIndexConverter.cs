using System;
using System.Globalization;
using Bare10.ViewModels;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class ViewToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int))
                throw new InvalidOperationException("The target must be a int");
            if (!(value is HomeViewModel.View))
                throw new InvalidOperationException("The value must be a HomeViewModel.View");

            return (int) (HomeViewModel.View)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(HomeViewModel.View))
                throw new InvalidOperationException("The value must be a HomeViewModel.View");
            if (!(value is int))
                throw new InvalidOperationException("The target must be a int");

            return (HomeViewModel.View)value;
        }
    }
}
