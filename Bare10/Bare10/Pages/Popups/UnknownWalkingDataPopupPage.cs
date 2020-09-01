using System.Linq;
using Bare10.Pages.Card;
using Bare10.Resources;
using Bare10.Utils.Views;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using XFShapeView;

namespace Bare10.Pages.Popups
{
    public class UnknownWalkingDataPopupPage : PopupPage
    {
        public UnknownWalkingDataPopupPage()
        {
            BackgroundColor = Colors.BulletinPopupBackground;

            var content = new ScrollView()
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(40, 0),
                Content =  new StackLayout()
                {
                    Spacing = 20,
                    Children =
                    {
                        new StackLayout()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            Spacing = 0,
                            Children =
                            {
                                new Label()
                                {
                                    HorizontalOptions = LayoutOptions.Center,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    TextColor = Colors.TextSpecial,
                                    FontAttributes = FontAttributes.Bold,
                                    FontSize = Sizes.TextLarge,
                                    Text = "\u26A0\n\nUkjent Gange"
                                },
                                DefaultView.UnderLine,
                            }
                        },
                        new Label()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = Colors.TextInverted,
                            FontAttributes = FontAttributes.Italic,
                            FontSize = Sizes.TextSmall,
                            Text = "Vi måler mye gange med ukjent hastighet. Det registreres som rolig gange. " +
                                   "Dette tyder på at pedometeret i telefonen ikke er skrudd på. " +
                                   "Som regel skyldes dette strømsparingsmoduser."
                        },
                        ListItemView("Skru av strømsparing"),
                        ListItemView("Sørg for at strømsparing ikke er på når du er ute å går"),
                        ListItemView("Gi Gå10 egen tillatelse i strømsparingsstyring om telefonen din har dette"),
                    }
                }
            };

            Content = new CardView(content)
            {
                Scale = 0.80f,
                //AnchorY = 1, // anchor to bottom
                CloseButtonClicked = async () =>
                {
                    if (PopupNavigation.Instance.PopupStack.Any())
                        await PopupNavigation.Instance.PopAllAsync();
                }
            };
        }

        private View ListItemView(string text)
        {
            var padding = (Sizes.TextSmall / 2) - 1;
            var circle = new ShapeView()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                ShapeType = ShapeType.Circle,
                Color = Color.Black,
                HeightRequest = Sizes.TextSmall - padding,
                WidthRequest = Sizes.TextSmall - padding,
                Margin = new Thickness(0, padding, 0, 0),
            };

            var lblText = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Start,
                TextColor = Colors.TextInverted,
                FontSize = Sizes.TextSmall,
                Text = text,
            };

            return new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                ColumnSpacing = 10,
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition(){ Width = GridLength.Auto },
                    new ColumnDefinition(){ Width = GridLength.Star },
                },
                Children =
                {
                    {circle, 0, 0},
                    {lblText, 1, 0},
                }
            };
        }
    }
}