using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class NullToBooleanConverter : IValueConverter
    {
        private readonly bool _trueIfNull;

        public NullToBooleanConverter(bool trueIfNull = false)
        {
            _trueIfNull = trueIfNull;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");

            return _trueIfNull ? value == null : value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}