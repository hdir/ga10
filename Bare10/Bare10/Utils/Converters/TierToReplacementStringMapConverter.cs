using System;
using System.Collections.Generic;
using System.Globalization;
using Bare10.Content;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class TierToReplacementStringMapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Dictionary<string, string>))
                throw new InvalidOperationException("The target must be a Dictionary<string, string>");
            if (!(value is Tier tier))
                throw new InvalidOperationException("The value must be a Tier");

            return SvgUtils.ReplaceStringMap(tier.ToTierColor(), replaceText: "\"#00ff00\"");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}
