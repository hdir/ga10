using Bare10.Localization;
using Bare10.ViewModels.Base;

namespace Bare10.ViewModels
{
    public abstract class SettingViewModel : ToolbarViewModelBase
    {
        public enum SettingType
        {
            About,
            Terms,
            Feedback,
            Testing,
        }

        public abstract SettingType Type { get; }
    }

    public class AboutViewModel : SettingViewModel
    {
        public override SettingType Type { get; } = SettingType.About;

        public override ToolbarState ToolbarState { get; } = new ToolbarState()
        {
            Title = AppText.profile_setting_about_title,
        };
    }

    public class TermsViewModel : SettingViewModel
    {
        public override SettingType Type { get; } = SettingType.Terms;

        public override ToolbarState ToolbarState { get; } = new ToolbarState()
        {
            Title = AppText.profile_setting_terms_title,
        };
    }

    public class FeedbackViewModel : SettingViewModel
    {
        public override SettingType Type { get; } = SettingType.Feedback;

        public override ToolbarState ToolbarState { get; } = new ToolbarState()
        {
            Title = AppText.profile_setting_give_feedback,
        };
    }
}