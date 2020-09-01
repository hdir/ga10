using Bare10.Localization;
using Bare10.ViewModels.ViewCellsModels;
using Xamarin.Forms;

namespace Bare10.Pages.Accessibility.ViewCells
{
    public class GoalViewCell : ViewCell
    {
        public GoalViewCell()
        {
            var label = Utils.Accessibility.CreateLabel(
                AppText.goals_tab_item_title,
                nameof(GoalViewModel.AccessibleText));

            var selectButton = Utils.Accessibility.CreateButton(
               AppText.goals_page_confirm_goal,
               nameof(GoalViewModel.ChooseCommand),
               nameof(GoalViewModel.AccessibleSelectButtonText));

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    label,
                    selectButton,
                }
            };
            View = layout;
        }
    }
}
