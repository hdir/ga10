using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class ProgressToDoubleConverter : IValueConverter
    {
        private readonly bool _displayWhenComplete;
        private readonly bool _displayWhenEmpty;

        public ProgressToDoubleConverter(bool displayWhenEmpty = true, bool displayWhenComplete = true)
        {
            _displayWhenComplete = displayWhenComplete;
            _displayWhenEmpty = displayWhenEmpty;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double v)
            {
                return v > 0 
                    ? (v < 1f 
                        ? 1f 
                        : (_displayWhenComplete 
                            ? 1f 
                            : 0)) 
                    : (_displayWhenEmpty 
                        ? 1f 
                        : 0
                    );
            }
            else
            {
                throw new InvalidOperationException("The value must be a double");
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}