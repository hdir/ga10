using Bare10.Localization;
using Bare10.Pages.Accessibility.Items;
using Bare10.ViewModels.Accessibility.ViewCellModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Bare10.Pages.Accessibility.ViewCells
{
    public class TieredAchievementViewCell : ViewCell
    {
        public TieredAchievementViewCell()
        {
            var label = Utils.Accessibility.CreateLabel(
                AppText.accessibility_achievement_item_label,
                nameof(TieredAchievementViewModel.Text));

            ////TODO: Apptext
            var tierList = Utils.Accessibility.CreateList(
                "Medaljer",
                nameof(TieredAchievementViewModel.Tiers),
                typeof(AchievementMedalView));

            View = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    label,
                    tierList,
                },
            };

            Height = 300;
        }
    }
}
