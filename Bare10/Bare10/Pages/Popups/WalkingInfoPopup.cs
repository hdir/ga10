using Bare10.Pages.Card;
using Bare10.Resources;
using Bare10.Utils.Views;
using FFImageLoading.Svg.Forms;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using XFShapeView;

namespace Bare10.Pages.Popups
{
    public class WalkingInfoPopup : PopupPage
    {
        private const uint CircleSize = 150;

        private const uint InternalPadding = 20;

        public WalkingInfoPopup(ImageSource icon, string title, string text, Color circleColor)
        {
            Padding = 30;

            Content = new CardView(ScrollContentView(icon, title, text, circleColor))
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = App.ScreenHeight * 0.7f,
                BackgroundColor = Colors.InfoPopupBackground,
                Padding = new Thickness(InternalPadding, 0, InternalPadding, InternalPadding),
                CloseButtonClicked = () =>
                {
                    PopupNavigation.Instance.PopAsync();
                }
            };
        }

        private View ScrollContentView(ImageSource icon, string title, string text, Color circleColor)
        {
            return new ScrollView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never,
                Padding = new Thickness(0, 0, 0, CardView.FooterHeight),
                Content = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 30,
                    Children =
                    {
                        new ShapeView()
                        {
                            WidthRequest = CircleSize,
                            HeightRequest = CircleSize,
                            ShapeType = ShapeType.Circle,
                            Color = circleColor,
                            Margin = new Thickness(0, InternalPadding, 0, 0),
                            Content = new SvgCachedImage()
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                WidthRequest = CircleSize / 2f,
                                HeightRequest = CircleSize / 2f,
                                Aspect = Aspect.AspectFit,
                                Source = icon,
                            },
                        },
                        new StackLayout()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Start,
                            Spacing = 0,
                            Children =
                            {
                                new Label()
                                {
                                    VerticalOptions = LayoutOptions.Start,
                                    HorizontalOptions = LayoutOptions.Center,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    TextColor = Colors.TextSpecial,
                                    FontAttributes = FontAttributes.Bold,
                                    FontSize = Sizes.TextLarge,
                                    Text = title,
                                },
                                DefaultView.UnderLine,
                            }
                        },
                        new Label()
                        {
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Start,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = Colors.InfoPopupText,
                            FontSize = Sizes.TextSmall,
                            Text = text,
                            LineHeight = 1.2f, //TODO: Make LabelRenderer for Android to increase line-height
                        },
                    },
                }
            };
        }
    }
}
