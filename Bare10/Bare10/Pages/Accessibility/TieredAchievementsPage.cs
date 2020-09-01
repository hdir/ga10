using Bare10.Localization;
using Bare10.Pages.Accessibility.ViewCells;
using Bare10.ViewModels.Accessibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bare10.Pages.Accessibility
{
    public class TieredAchievementsPage : AccessibilityBasePage<TieredAchievementsViewModel>
    {
        public TieredAchievementsPage()
            : base(true)
        {
            var list = Utils.Accessibility.CreateList(
                AppText.achievements_tab_item_title,
                nameof(TieredAchievementsViewModel.Achievements),
                typeof(TieredAchievementViewCell));

            AddContent(list);
        }
    }
}
