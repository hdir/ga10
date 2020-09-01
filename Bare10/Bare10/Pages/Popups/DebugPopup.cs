using Bare10.Pages.Views;
using Bare10.Resources;
using Bare10.Utils.Views;
using Bare10.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Bare10.Pages.Popups
{
    public class DebugPopup : PopupPage
    {
        public DebugPopup(TestingViewModel testingViewModel)
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5f);

            var btnClose = DefaultView.CloseButton();
            btnClose.HorizontalOptions = LayoutOptions.Center;
            btnClose.VerticalOptions = LayoutOptions.End;
            btnClose.Margin = new Thickness(20);

            btnClose.AddTouch((sender, args) => { PopupNavigation.Instance.PopAsync(); });


            var scrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
                Content = new TestingView { BindingContext = testingViewModel },
            };

            Content = new Grid()
            {
                BackgroundColor = Colors.Background,
                Children =
                {
                    scrollView,
                    btnClose,
                }
            };
        }
    }
}
