using System;
using System.Globalization;
using Bare10.Resources;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class GoalToBorderIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                throw new InvalidOperationException("The target must be a ImageSource");
            if (!(value is Content.Goal goal))
                throw new InvalidOperationException("The value must be a Goal");

            switch (goal)
            {
                case Content.Goal.TenMinutes:
                    return Images.GoalBorder10;
                case Content.Goal.TwentyMinutes:
                    return Images.GoalBorder20;
                case Content.Goal.ThirtyMinutes:
                    return Images.GoalBorder30;
                default:
                    return Images.GoalBorder10;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}