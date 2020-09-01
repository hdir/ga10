using System;
using System.Globalization;
using Bare10.Content;
using Bare10.ViewModels.ViewCellsModels;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class GoalToSegmentCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(uint))
                throw new InvalidOperationException("The target must be a uint");
            if (!(value is GoalViewModel goal))
                throw new InvalidOperationException("The value must be a Goal");

            return ToSegments(goal.Goal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }

        public static uint ToSegments(Goal goal)
        {
            switch (goal)
            {
                case Goal.TenMinutes:
                    return 1;
                case Goal.TwentyMinutes:
                    return 2;
                case Goal.ThirtyMinutes:
                    return 3;
                default:
#if DEBUG
                    throw new ArgumentOutOfRangeException(nameof(goal), goal, null);
#else
                    return 1;
#endif
            }
        } 
    }

    public class GoalToTotalProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(float))
                throw new InvalidOperationException("The target must be a float");
            if (!(value is GoalViewModel))
                throw new InvalidOperationException("The value must be a Goal");

            return ((GoalViewModel) value).MinutesRequiredToAchieve;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}
