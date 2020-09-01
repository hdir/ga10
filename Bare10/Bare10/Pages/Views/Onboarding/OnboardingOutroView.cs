using Bare10.Localization;
using Bare10.Pages.Views.Onboarding.Base;
using Bare10.Resources;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Onboarding
{
    public class OnboardingOutroView : OnboardingItemView<OnboardingOutroViewModel>
    {
        public OnboardingOutroView()
        {
            var lblTitle = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                Text = AppText.onboarding_last_title,
                FontSize = Sizes.Title,
            };

            var lblText = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Text = AppText.onboarding_last_text,
                FontSize = Sizes.Title,
            };

            AddAnimatedView(lblTitle);
            AddAnimatedView(lblText);
        }
    }

    public class OnboardingOutroViewModel : OnboardingItemViewModel
    {
        public OnboardingOutroViewModel(OnboardingDelegate controller) : base(controller)
        {
        }

        public override bool IsValid => true;
    }
}
