using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;
using Xamarin.Forms;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Commands;
using Bare10.Resources;
using Bare10.Localization;

namespace Bare10.Pages.Accessibility
{
    public abstract class AccessibilityBasePage<T> : MvxContentPage<T>
        where T : class, IMvxViewModel
    {
        private StackLayout layout;
       
        public AccessibilityBasePage(bool withScroll)
        {
            Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(On<Xamarin.Forms.PlatformConfiguration.iOS>(), true);
            NavigationPage.SetHasNavigationBar(this, false);

            BackgroundColor = Colors.Background;

            var backButton = new Button
            {
                Text = AppText.navigation_back,
                Command = new MvxAsyncCommand(async () => await Mvx.IoCProvider.GetSingleton<IMvxNavigationService>().Close(ViewModel)),
            };

            layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = 
                {
                    backButton,
                }
            };

            if(withScroll)
            {
                var scrollView = new ScrollView
                {
                    Content = layout
                };

                Content = scrollView;
            }
            else
            {
                Content = layout;
            }

        }

        protected void AddContent(View content)
        {
            layout.Children.Add(content);
        }


    }
}
