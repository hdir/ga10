using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class ColorToReplacementStringMapConverter : IValueConverter
    {
        private readonly string _replacementText;

        public ColorToReplacementStringMapConverter(string replacementText = "\"#000000\"")
        {
            _replacementText = replacementText;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Dictionary<string, string>))
                throw new InvalidOperationException("The target must be a Dictionary<string, string>");
            if (!(value is Color color))
                throw new InvalidOperationException("The value must be a color");

            return SvgUtils.ReplaceStringMap(color, replaceText: _replacementText);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}