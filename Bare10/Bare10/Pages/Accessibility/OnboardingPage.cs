using Bare10.Localization;
using Bare10.Resources;
using Bare10.ViewModels.Accessibility;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Accessibility
{
    [MvxContentPagePresentation(Animated = false, NoHistory = true, WrapInNavigationPage = false)]
    public class OnboardingPage : MvxContentPage<OnboardingViewModel>
    {
        public OnboardingPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            BackgroundColor = Colors.Background;

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    FirstViewTitle,
                    FirstViewText,
                    NameEntryTitle,
                    NameEntry,
                    TermsTitle,
                    TermsText,
                    Term1,
                    Term2,
                    Term3,
                    TermsSwitch,
                    CompleteButton(),
                },
            };
            //root object is not part of accessibility navigation
            Utils.Accessibility.Unused(layout);

            var scrollLayout = new ScrollView
            {
                Content = layout,
            };
            Utils.Accessibility.Unused(scrollLayout);

            Content = scrollLayout;
        }

        private View FirstViewTitle => Utils.Accessibility.CreateLabel( AppText.onboarding_first_title);
        private View FirstViewText => Utils.Accessibility.CreateLabel( AppText.onboarding_first_text);

        private View NameEntryTitle => Utils.Accessibility.CreateLabel( AppText.onboarding_entry_title);
        private View NameEntry => Utils.Accessibility.CreateEntry(AppText.onboarding_entry_text, nameof(OnboardingViewModel.UserName));

        private View TermsTitle => Utils.Accessibility.CreateLabel( AppText.onboarding_terms_title);
        private View TermsText => Utils.Accessibility.CreateLabel( AppText.onboarding_terms_text);

        private View Term1 => Utils.Accessibility.CreateLabel(AppText.onboarding_term_1);
        private View Term2 => Utils.Accessibility.CreateLabel(AppText.onboarding_term_2);
        private View Term3 => Utils.Accessibility.CreateLabel(AppText.onboarding_term_3);

        private View TermsSwitch => Utils.Accessibility.CreateLabelledSwitch(
            AppText.onboarding_terms_accept + AppText.onboarding_terms_accept_terms,
            nameof(OnboardingViewModel.HasAcceptedTerms));

        private View CompleteButton()
        {
            View button = Utils.Accessibility.CreateButton(
                AppText.accessibility_onboarding_complete_button_text,
                nameof(OnboardingViewModel.OnComplete));

            button.SetBinding(IsEnabledProperty, nameof(OnboardingViewModel.ReadyToProceed));

            return button;
        }
    }
}
