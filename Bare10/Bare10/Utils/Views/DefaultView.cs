using Bare10.Resources;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace Bare10.Utils.Views
{
    public static class DefaultView
    {
        public static View SectionLine =>  new BoxView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                HeightRequest = 0.8f,
                BackgroundColor = Colors.Border,
                Margin = new Thickness(0, 0, 0, -1f),
            };

        public static View UnderLine => new BoxView()
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.End,
            HeightRequest = 1.5f,
            BackgroundColor = Colors.TextSpecial,
        };

        public static View CloseButton()
        {
            var btnClose = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit,
                HeightRequest = 40,
                WidthRequest = 40,
                Source = Images.PopupClose,
            };
            btnClose.ReplaceColor(Colors.TextSpecial);

            return btnClose;
        }
    }
}
