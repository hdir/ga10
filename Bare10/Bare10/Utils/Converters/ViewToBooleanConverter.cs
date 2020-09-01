using System;
using System.Globalization;
using Bare10.ViewModels;
using Xamarin.Forms;

namespace Bare10.Utils.Converters
{
    public class ViewToBooleanConverter : IValueConverter
    {
        private readonly HomeViewModel.View _trueView;

        public ViewToBooleanConverter(HomeViewModel.View trueView)
        {
            _trueView = trueView;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(bool))
            //    throw new InvalidOperationException("The target must be a bool");
            //if (!(value is HomeViewModel.View))
            //    throw new InvalidOperationException("The value must be a HomeViewModel.View");

            return _trueView == (HomeViewModel.View)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}