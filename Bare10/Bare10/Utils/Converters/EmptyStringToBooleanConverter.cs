using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class EmptyStringToBooleanConverter : IValueConverter
    {
        private readonly bool _trueIfEmpty;

        public EmptyStringToBooleanConverter(bool trueIfEmpty = true)
        {
            _trueIfEmpty = trueIfEmpty;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");

            if (value is string str)
                return _trueIfEmpty ? string.IsNullOrEmpty(str) : !string.IsNullOrEmpty(str);
            return !_trueIfEmpty ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}
