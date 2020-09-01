using System;
using System.Globalization;
using Bare10.Pages.Custom;
using Bare10.Resources;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class ObjectToBooleanConverter : IValueConverter
    {
        private readonly object _desired;

        public ObjectToBooleanConverter(object desired)
        {
            _desired = desired;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");

            return value.Equals(_desired);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }

    public class InfoLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Color))
                throw new InvalidOperationException("The target must be a Color");
            if (!(value is NotifyCenter.InfoType type))
                throw new InvalidOperationException("The value must be a InfoLevelType");

            switch (type)
            {
                case NotifyCenter.InfoType.Loading:
                    return Colors.InfoLoadingMessageBackground;
                case NotifyCenter.InfoType.Info:
                    return Colors.InfoMessageBackground;
                case NotifyCenter.InfoType.Error:
                    return Colors.InfoErrorMessageBackground;
                case NotifyCenter.InfoType.Warning:
                    return Colors.InfoWarningBackground;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}