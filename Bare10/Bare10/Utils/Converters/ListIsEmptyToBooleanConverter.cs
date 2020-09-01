using System;
using System.Collections;
using System.Globalization;
using MvvmCross.Binding.Extensions;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class ListIsEmptyToBooleanConverter : IValueConverter
    {
        private readonly bool _trueIfHasItems;

        public ListIsEmptyToBooleanConverter(bool trueIfHasItems = false)
        {
            _trueIfHasItems = trueIfHasItems;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return !_trueIfHasItems;

            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a string");
            if (!(value is IEnumerable))
                throw new InvalidOperationException("The value must be a IEnumerable");

            var hasItems = ((IEnumerable) value).Count() > 0;

            return _trueIfHasItems ? hasItems : !hasItems;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}