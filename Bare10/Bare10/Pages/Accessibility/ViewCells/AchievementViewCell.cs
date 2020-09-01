using Bare10.Localization;
using Bare10.ViewModels.ViewCellsModels;
using Xamarin.Forms;

namespace Bare10.Pages.Accessibility.ViewCells
{
    public class AchievementViewCell : ViewCell
    {
        public AchievementViewCell()
        {
            var label = Utils.Accessibility.CreateLabel(
                AppText.accessibility_achievement_item_label,
                nameof(AchievementViewModel.AccessibilityText));

            View = label;
        }
    }
}
