using System;
using System.Globalization;
using Bare10.Content;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class TierToNameConverter : IValueConverter
    {
        private readonly int _offset;
        private readonly bool _toLower;

        public TierToNameConverter(int offset = 0, bool toLower = false)
        {
            _offset = offset;
            _toLower = toLower;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            if (value is Tier tier)
            {
                if (_offset != 0)
                {
                    var o = (int)tier + _offset;
                    if (o >= 0 && o < TierExtensions.GetAll().Length)
                        tier = (Tier) o;
                }
                return _toLower ? tier.ToTierName().ToLower() : tier.ToTierName();
            }

            return value.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}