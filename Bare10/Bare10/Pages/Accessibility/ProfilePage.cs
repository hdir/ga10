using Bare10.Localization;
using Bare10.ViewModels.Accessibility;
using Xamarin.Forms;

namespace Bare10.Pages.Accessibility
{
    public class ProfilePage : AccessibilityBasePage<ProfileViewModel>
    {
        public ProfilePage()
            : base(true)
        {
            var nameEntry = Utils.Accessibility.CreateEntry(
                AppText.profile_name_header,
                nameof(ProfileViewModel.Name));

            AddContent(nameEntry);

            var notificationsSwitch = Utils.Accessibility.CreateLabelledSwitch(
                AppText.profile_setting_notification_title,
                nameof(ProfileViewModel.WillSendNotifications));

            AddContent(notificationsSwitch);
            AddContent(AboutView());
            AddContent(TermsView());
        }

        private View AboutView()
        {
            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 10,
                Children =
                {
                    Utils.Accessibility.CreateLabel(AppText.about_1_title),
                    Utils.Accessibility.CreateLabel(AppText.about_1_text),
                    Utils.Accessibility.CreateLabel(AppText.about_2_title),
                    Utils.Accessibility.CreateLabel(AppText.about_2_text),
                    Utils.Accessibility.CreateLabel(AppText.about_3_title),
                    Utils.Accessibility.CreateLabel(Device.RuntimePlatform == Device.Android
                        ? AppText.about_3_text_android
                        : AppText.about_3_text_ios),
                    Utils.Accessibility.CreateLabel(AppText.about_4_title),
                    Utils.Accessibility.CreateLabel(AppText.about_4_text),
                },
            };
            Utils.Accessibility.InUse(layout, AppText.profile_setting_about_title, "");

            return layout;
        }

        private View TermsView()
        {
            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 10,
                Children =
                {
                    Utils.Accessibility.CreateLabel(AppText.terms_1_0_title),
                    Utils.Accessibility.CreateLabel(AppText.terms_1_0_text),
                    Utils.Accessibility.CreateLabel(AppText.terms_2_0_title),
                    Utils.Accessibility.CreateLabel(AppText.terms_2_0_text),
                    Utils.Accessibility.CreateLabel(AppText.terms_3_0_title),
                    Utils.Accessibility.CreateLabel(AppText.terms_3_0_text),
                    Utils.Accessibility.CreateLabel(AppText.terms_3_1_title),
                    Utils.Accessibility.CreateLabel(AppText.terms_3_1_text),
                    Utils.Accessibility.CreateLabel(AppText.terms_3_2_title),
                    Utils.Accessibility.CreateLabel(AppText.terms_3_2_text),
                    Utils.Accessibility.CreateLabel(AppText.terms_3_3_title),
                    Utils.Accessibility.CreateLabel(AppText.terms_3_3_text),
                },
            };
            Utils.Accessibility.InUse(layout, AppText.profile_setting_terms_title, "");

            return layout;
        }
    }
}
