using System.Collections.Generic;
using Bare10.Pages.Card;
using Bare10.Resources;
using Bare10.Utils.Views;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Bare10.Pages.Popups
{
    public class InfoPopup : PopupPage
    {
        private const uint CircleSize = 150;

        public InfoPopup(string title, string text, IEnumerable<string> list)
        {
            Content = new CardView(ScrollContentView(title, text, list))
            {
                Margin = new Thickness(30),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Colors.InfoPopupBackground,
                CloseButtonClicked = () =>
                {
                    PopupNavigation.Instance.PopAsync();
                }
            };
        }

        private View ScrollContentView(string title, string text, IEnumerable<string> list)
        {
            return new ScrollView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0, 0, CardView.FooterHeight),
                Content = new StackLayout()
                {
                    Padding = new Thickness(30, 35),
                    Spacing = 30,
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
                                    Text = title,
                                },
                                DefaultView.UnderLine,
                            }
                        },
                        new Label()
                        {
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = Colors.InfoPopupText,
                            FontSize = Sizes.TextSmall,
                            Text = text,
                            LineHeight = 1.2f, //TODO: Make LabelRenderer for Android to increase line-height
                        },
                        ItemListView(list),
                    },
                }
            };
        }

        private View ItemListView(IEnumerable<string> list)
        {
            var layout = new StackLayout();

            foreach (var item in list)
            {
                layout.Children.Add(new Grid()
                {
                    ColumnDefinitions = new ColumnDefinitionCollection()
                    {
                        new ColumnDefinition(){Width = 20},
                        new ColumnDefinition(){Width = GridLength.Star},
                    },
                    Children =
                    {
                        {new Label()
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Start,
                                Text = "\u25CF",
                                TextColor = Colors.TextSpecial,
                                FontSize = Sizes.TextSmall,
                            }
                        , 0, 0},
                        {new Label()
                            {
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Start,
                                Text = item,
                                FontSize = Sizes.TextSmall,
                                TextColor = Colors.InfoPopupText,
                            }
                        , 1, 0},
                    }
                });
            }

            return layout;
        }
    }
}
