using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class BooleanToDoubleConverter : IValueConverter
    {
        private readonly double _trueValue;
        private readonly double _falseValue;

        public BooleanToDoubleConverter(double trueValue, double falseValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new InvalidOperationException("The value must be a bool");

            return (bool)value ? _trueValue : _falseValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}