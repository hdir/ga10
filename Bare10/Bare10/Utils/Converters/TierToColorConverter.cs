using System;
using System.Globalization;
using Bare10.Content;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class TierToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Color))
                throw new InvalidOperationException("The target must be a Color");
            if (!(value is Tier tier))
                throw new InvalidOperationException("The value must be a Tier");

            return tier.ToTierColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}