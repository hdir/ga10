using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class IntToBooleanConverter : IValueConverter
    {
        private readonly int _trueInt;

        public IntToBooleanConverter(int trueInt)
        {
            _trueInt = trueInt;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");
            if (!(value is int))
                throw new InvalidOperationException("The value must be a int");

            return (int)value == _trueInt;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}