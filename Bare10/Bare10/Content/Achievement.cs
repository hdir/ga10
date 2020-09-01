using System.Collections.Generic;
using Bare10.Localization;
using Bare10.ViewModels.ViewCellsModels;

namespace Bare10.Content
{
    public enum Achievement
    {
        FirstTen,
        SetHarderGoal,
        DailyGoalAchievedOneWeek,
        DailyGoalAchievedTwoWeeks,
        DailyGoalAchievedThreeWeeks,
        DailyGoalAchievedOneMonth,
        TwentyMinutesSixtyDays,
        ThirtyMinutesHundredDays,
    }

    public static class AchievementExtensions
    {
        public static IReadOnlyList<AchievementViewModel> Achievements { get; } = new List<AchievementViewModel>
            {
                new AchievementViewModel(Achievement.FirstTen)
                {
                    Title = AppText.achievement_first_ten_title,
                    Description = AppText.achievement_first_ten_description,
                    UnlockedDescription = AppText.achievement_first_ten_description_achieved,
                    HealthDescription = AppText.achievement_first_ten_health_description,
                },
                new AchievementViewModel(Achievement.SetHarderGoal)
                {
                    Title = AppText.achievement_set_harder_goal_title,
                    Description = AppText.achievement_set_harder_goal_description,
                    UnlockedDescription = AppText.achievement_set_harder_goal_description_achieved,
                    HealthDescription = AppText.achievement_set_harder_goal_health_description,
                },
                new AchievementViewModel(Achievement.DailyGoalAchievedOneWeek)
                {
                    Title = AppText.achievement_daily_goal_achieved_one_week_title,
                    Description = AppText.achievement_daily_goal_achieved_one_week_description,
                    UnlockedDescription = AppText.achievement_daily_goal_achieved_one_week_description_achieved,
                    HealthDescription = AppText.achievement_daily_goal_achieved_one_week_health_description,
                },
                new AchievementViewModel(Achievement.DailyGoalAchievedTwoWeeks)
                {
                    Title = AppText.achievement_daily_goal_achieved_two_weeks_title,
                    Description = AppText.achievement_daily_goal_achieved_two_weeks_description,
                    UnlockedDescription = AppText.achievement_daily_goal_achieved_two_weeks_description_achieved,
                    HealthDescription = AppText.achievement_daily_goal_achieved_two_weeks_health_description,
                },
                new AchievementViewModel(Achievement.DailyGoalAchievedThreeWeeks)
                {
                    Title = AppText.achievement_daily_goal_achieved_three_weeks_title,
                    Description = AppText.achievement_daily_goal_achieved_three_weeks_description,
                    UnlockedDescription = AppText.achievement_daily_goal_achieved_three_weeks_description_achieved,
                    HealthDescription = AppText.achievement_daily_goal_achieved_three_weeks_health_description,
                },
                new AchievementViewModel(Achievement.DailyGoalAchievedOneMonth)
                {
                    Title = AppText.achievement_daily_goal_achieved_one_month_title,
                    Description = AppText.achievement_daily_goal_achieved_one_month_description,
                    UnlockedDescription = AppText.achievement_daily_goal_achieved_one_month_description_achieved,
                    HealthDescription = AppText.achievement_daily_goal_achieved_one_month_health_description,
                },
                new AchievementViewModel(Achievement.TwentyMinutesSixtyDays)
                {
                    Title = AppText.achievement_twenty_mins_sixty_days_title,
                    Description = AppText.achievement_twenty_mins_sixty_days_description,
                    UnlockedDescription = AppText.achievement_twenty_mins_sixty_days_description_achieved,
                    HealthDescription = AppText.achievement_twenty_mins_sixty_days_health_description,
                },
                new AchievementViewModel(Achievement.ThirtyMinutesHundredDays)
                {
                    Title = AppText.achievement_thirty_mins_hundred_days_title,
                    Description = AppText.achievement_thirty_mins_hundred_days_description,
                    UnlockedDescription = AppText.achievement_thirty_mins_hundred_days_description_achieved,
                    HealthDescription = AppText.achievement_thirty_mins_hundred_days_health_description,
                },
            };
    }
}
