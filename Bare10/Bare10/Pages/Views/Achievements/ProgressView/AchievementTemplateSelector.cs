using System;
using Bare10.ViewModels.Items;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Achievements.ProgressView
{
    public class AchievementTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is AchievementTierProgressViewModel vm)
            {
                switch (vm.Achievement)
                {
                    case Content.TieredAchievement.ReachedGoal:
                        return new DataTemplate(typeof(ReachedGoalView));
                    case Content.TieredAchievement.ThirtyMinutes:
                        return new DataTemplate(typeof(ThirtyMinutesView));
                    case Content.TieredAchievement.ReachedGoalFiveDaysInARow:
                        return new DataTemplate(typeof(ReachedGoalFiveDaysInARowView));
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new DataTemplate(Fallback);
        }

        private static View Fallback()
        {
            return new Grid();
        }
    }
}