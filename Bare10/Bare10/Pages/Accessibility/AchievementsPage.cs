using Bare10.Localization;
using Bare10.ViewModels.Accessibility;
using Bare10.Pages.Accessibility.ViewCells;

namespace Bare10.Pages.Accessibility
{
    public class AchievementsPage : AccessibilityBasePage<AchievementsViewModel>
    {
        public AchievementsPage()
            : base(true)
        {
            var list = Utils.Accessibility.CreateList(
                AppText.achievements_tab_item_title,
                nameof(AchievementsViewModel.Achievements),
                typeof(AchievementViewCell));

            AddContent(list);
        }
    }
}
