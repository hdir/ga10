using Bare10.Localization;
using Bare10.Pages.Accessibility.ViewCells;
using Bare10.ViewModels.Accessibility;

namespace Bare10.Pages.Accessibility
{
    public class GoalsPage : AccessibilityBasePage<GoalsViewModel>
    {
        public GoalsPage()
            : base(true)
        {
            var list = Utils.Accessibility.CreateList(
                AppText.goals_tab_item_title,
                nameof(GoalsViewModel.Goals),
                typeof(GoalViewCell));

            AddContent(list);
        }
    }
}
