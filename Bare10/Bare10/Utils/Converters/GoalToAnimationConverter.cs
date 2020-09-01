using System;
using System.Globalization;
using Bare10.ViewModels.ViewCellsModels;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class GoalToAnimationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            if (!(value is GoalViewModel))
                throw new InvalidOperationException("The value must be a Goal");

            switch (((GoalViewModel) value).Goal)
            {
                case Content.Goal.TenMinutes:
                    return "1segments.json";
                case Content.Goal.TwentyMinutes:
                    return "2segments.json";
                case Content.Goal.ThirtyMinutes:
                    return "3segments.json";
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