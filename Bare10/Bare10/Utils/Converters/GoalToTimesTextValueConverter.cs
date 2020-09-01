using System;
using System.Globalization;
using Bare10.Content;
using Bare10.ViewModels.ViewCellsModels;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class GoalToTimesTextValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            if (!(value is GoalViewModel goal))
                throw new InvalidOperationException("The value must be a Goal");

            return ToText(goal.Goal);
        }

        public static string ToText(Goal goal)
        {
            switch (goal)
            {
                case Goal.TenMinutes:
                    return null;
                case Goal.TwentyMinutes:
                    return "2x";
                case Goal.ThirtyMinutes:
                    return "3x";
                default:
#if DEBUG
                    throw new ArgumentOutOfRangeException(nameof(goal), goal, null);
#else
                    return null;
#endif
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}
