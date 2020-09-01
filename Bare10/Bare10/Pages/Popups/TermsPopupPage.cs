using Bare10.Pages.Custom;
using Bare10.Pages.Views.Items;
using Bare10.Resources;
using Bare10.Utils.Views;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Bare10.Pages.Popups
{
    public class TermsPopupPage : PopupPage
    {
        private const uint FooterHeight = 100;

        public TermsPopupPage()
        {
            BackgroundColor = Colors.BulletinPopupBackground;

            var btnClose = DefaultView.CloseButton();
            btnClose.HorizontalOptions = LayoutOptions.Center;
            btnClose.VerticalOptions = LayoutOptions.End;
            btnClose.Margin = new Thickness(20);

            btnClose.AddTouch((sender, args) => { PopupNavigation.Instance.PopAsync(); });

            Content = new Grid()
            {
                BackgroundColor = Colors.Background,
                Children =
                {
                    new TermsScrollView()
                    {
                        Padding = new Thickness(0, 0, 0, FooterHeight)
                    },
                    new GradientView(Colors.Background)
                    {
                        VerticalOptions = LayoutOptions.End,
                        Orientation = StackOrientation.Vertical,
                        HeightRequest = FooterHeight,
                        InputTransparent = true,
                        ScaleY = -1,
                    },
                    btnClose,
                }
            };
        }
    }
}
