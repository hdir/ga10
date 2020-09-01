using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class BooleanToReplacementStringMapConverter : IValueConverter
    {
        private readonly Color _trueColor;
        private readonly Color _falseColor;
        private readonly string _replaceTitle;
        private readonly string _replaceText;

        public BooleanToReplacementStringMapConverter(Color trueColor, Color falseColor, string replaceTitle = "fill", string replaceText = "\"#000000\"")
        {
            _trueColor = trueColor;
            _falseColor = falseColor;
            _replaceTitle = replaceTitle;
            _replaceText = replaceText;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Dictionary<string, string>))
                throw new InvalidOperationException("The target must be a Dictionary<string, string>");
            if (!(value is bool))
                throw new InvalidOperationException("The value must be a bool");

            return SvgUtils.ReplaceStringMap((bool)value ? _trueColor : _falseColor, _replaceTitle, _replaceText);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        private static string ToHex(Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            return $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";
        }
    }
}
