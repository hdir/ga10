using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        private readonly Color _trueColor;
        private readonly Color _falseColor;

        public BooleanToColorConverter(Color trueColor, Color falseColor)
        {
            _trueColor = trueColor;
            _falseColor = falseColor;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(Color))
            //    throw new InvalidOperationException("The target must be a Color");
            //if (!(value is bool))
            //    throw new InvalidOperationException("The value must be a bool");

            return (bool)value ? _trueColor : _falseColor;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}
