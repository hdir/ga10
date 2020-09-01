using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class ProgressToLabelCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int))
                throw new InvalidOperationException("The target must be a int");
            if (!(value is int target))
                throw new InvalidOperationException("The value must be a int");

            if (target <= 10)
                return target + 1;
            if (target == 15)
                return 4;
            if (target == 30)
                return 4;
            if (target == 50)
                return 6;
            if (target == 100)
                return 5;

            return 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}