using System;
using FFImageLoading.Svg.Forms;
using System.Collections.Generic;
using Bare10.Content;
using Xamarin.Forms;

namespace Bare10.Resources
{
    public static class Images
    {
        #region Fra Victoria

        private static readonly ImageSource ic_health_heart = SvgImageSource.FromResource("Bare10.Resources.Images.heart.svg");
        private static readonly ImageSource ic_goal_10 = SvgImageSource.FromResource("Bare10.Resources.Images.goal_10.svg");
        private static readonly ImageSource ic_goal_20 = SvgImageSource.FromResource("Bare10.Resources.Images.goal_20.svg");
        private static readonly ImageSource ic_goal_30 = SvgImageSource.FromResource("Bare10.Resources.Images.goal_30.svg");
        private static readonly ImageSource ic_tab_history_active = SvgImageSource.FromResource("Bare10.Resources.Images.tab_history_active.svg");
        private static readonly ImageSource ic_tab_history_inactive = SvgImageSource.FromResource("Bare10.Resources.Images.tab_history_inactive.svg");
        private static readonly ImageSource ic_tab_achievements_active = SvgImageSource.FromResource("Bare10.Resources.Images.tab_achievements_active.svg");
        private static readonly ImageSource ic_tab_achievements_inactive = SvgImageSource.FromResource("Bare10.Resources.Images.tab_achievements_inactive.svg");
        private static readonly ImageSource ic_tab_goals_active = SvgImageSource.FromResource("Bare10.Resources.Images.tab_goals_active.svg");
        private static readonly ImageSource ic_tab_goals_inactive = SvgImageSource.FromResource("Bare10.Resources.Images.tab_goals_inactive.svg");
        private static readonly ImageSource ic_tab_profile_active = SvgImageSource.FromResource("Bare10.Resources.Images.tab_profile_active.svg");
        private static readonly ImageSource ic_tab_profile_inactive = SvgImageSource.FromResource("Bare10.Resources.Images.tab_profile_inactive.svg");
        private static readonly ImageSource ic_shoe_brisk = SvgImageSource.FromResource("Bare10.Resources.Images.shoe.svg");
        private static readonly ImageSource ic_shoe_brisk_big = SvgImageSource.FromResource("Bare10.Resources.Images.shoe.svg");
        private static readonly ImageSource ic_shoe_normal = SvgImageSource.FromResource("Bare10.Resources.Images.shoe_slow.svg");
        private static readonly ImageSource ic_shoe_normal_big = SvgImageSource.FromResource("Bare10.Resources.Images.shoe_slow.svg");
        private static readonly ImageSource ic_watch = SvgImageSource.FromResource("Bare10.Resources.Images.smartwatch.svg");
        private static readonly ImageSource ic_close = SvgImageSource.FromResource("Bare10.Resources.Images.terms_close.svg");
        private static readonly ImageSource ic_close_small = SvgImageSource.FromResource("Bare10.Resources.Images.terms_close.svg");
        private static readonly ImageSource ic_info = SvgImageSource.FromResource("Bare10.Resources.Images.history_info.svg");
        private static readonly ImageSource ic_arrow = SvgImageSource.FromResource("Bare10.Resources.Images.toolbar_arrow.svg");
        private static readonly ImageSource ic_pen = SvgImageSource.FromResource("Bare10.Resources.Images.pen_edit_name.svg");
        private static readonly ImageSource ic_logo_basic = SvgImageSource.FromResource("Bare10.Resources.Images.toolbar_logo_basic.svg");
        private static readonly ImageSource ic_share = SvgImageSource.FromResource("Bare10.Resources.Images.toolbar_share.svg");
        private static readonly ImageSource ic_helsedir = SvgImageSource.FromResource("Bare10.Resources.Images.helsedirektoratet.svg");
        private static readonly ImageSource ic_checkmark = SvgImageSource.FromResource("Bare10.Resources.Images.checkmark.svg");
        private static readonly ImageSource ic_lock = SvgImageSource.FromResource("Bare10.Resources.Images.lock.svg");

        #endregion

        #region Toolbar
        public static readonly ImageSource ToolbarIcon = ic_logo_basic;
        public static readonly ImageSource ToolbarArrow = ic_arrow;
        public static readonly ImageSource ToolbarShare = ic_share;
        #endregion

        #region Achievements
        public static ImageSource HealthHeart = ic_health_heart;
        public static readonly Dictionary<Achievement, string> AchievementIcons = new Dictionary<Achievement, string>
        {
            { Achievement.FirstTen, "Bare10.Resources.Images.Achievements.badge_01.svg" },
            { Achievement.SetHarderGoal, "Bare10.Resources.Images.Achievements.badge_02.svg" },
            { Achievement.DailyGoalAchievedOneWeek, "Bare10.Resources.Images.Achievements.badge_03.svg" },
            { Achievement.DailyGoalAchievedTwoWeeks, "Bare10.Resources.Images.Achievements.badge_04.svg" },
            { Achievement.DailyGoalAchievedThreeWeeks, "Bare10.Resources.Images.Achievements.badge_05.svg" },
            { Achievement.DailyGoalAchievedOneMonth, "Bare10.Resources.Images.Achievements.badge_06.svg" },
            { Achievement.TwentyMinutesSixtyDays, "Bare10.Resources.Images.Achievements.badge_07.svg" },
            { Achievement.ThirtyMinutesHundredDays, "Bare10.Resources.Images.Achievements.badge_08.svg" },
        };
        public static readonly Dictionary<Achievement, string> AchievementIncompleteIcons = new Dictionary<Achievement, string>
        {
            { Achievement.FirstTen, "Bare10.Resources.Images.Achievements.badge_01_incomplete.svg"  },
            { Achievement.SetHarderGoal, "Bare10.Resources.Images.Achievements.badge_02_incomplete.svg" },
            { Achievement.DailyGoalAchievedOneWeek, "Bare10.Resources.Images.Achievements.badge_03_incomplete.svg" },
            { Achievement.DailyGoalAchievedTwoWeeks, "Bare10.Resources.Images.Achievements.badge_04_incomplete.svg" },
            { Achievement.DailyGoalAchievedThreeWeeks, "Bare10.Resources.Images.Achievements.badge_05_incomplete.svg" },
            { Achievement.DailyGoalAchievedOneMonth, "Bare10.Resources.Images.Achievements.badge_06_incomplete.svg" },
            { Achievement.TwentyMinutesSixtyDays, "Bare10.Resources.Images.Achievements.badge_07_incomplete.svg" },
            { Achievement.ThirtyMinutesHundredDays, "Bare10.Resources.Images.Achievements.badge_08_incomplete.svg" },
        };

        public static string TieredAchievementIncompleteIcon(TieredAchievement achievement)
        {
            if (TieredAchievementIncompleteIcons.ContainsKey(achievement))
                return TieredAchievementIncompleteIcons[achievement];
#if DEBUG
            throw new ArgumentOutOfRangeException(nameof(achievement), achievement, $"No icon for {achievement}");
#else
            return "";
#endif
        }

        public static string TieredAchievementCompleteIcon(TieredAchievement achievement)
        {
            if (TieredAchievementCompleteIcons.ContainsKey(achievement))
                return TieredAchievementCompleteIcons[achievement];
#if DEBUG
            throw new ArgumentOutOfRangeException(nameof(achievement), achievement, $"No icon for {achievement}");
#else
            return "";
#endif
        }

        private static readonly Dictionary<TieredAchievement, string> TieredAchievementIncompleteIcons = new Dictionary<TieredAchievement, string>
        {
            { TieredAchievement.ReachedGoal, "Bare10.Resources.Images.Achievements.tier_daily_locked.svg"  },
            { TieredAchievement.ThirtyMinutes, "Bare10.Resources.Images.Achievements.tier_minute_locked.svg"  },
            { TieredAchievement.ReachedGoalFiveDaysInARow, "Bare10.Resources.Images.Achievements.tier_5day_locked.svg"  },
        };

        private static readonly Dictionary<TieredAchievement, string> TieredAchievementCompleteIcons = new Dictionary<TieredAchievement, string>
        {
            { TieredAchievement.ReachedGoal, "Bare10.Resources.Images.Achievements.tier_daily.svg"  },
            { TieredAchievement.ThirtyMinutes, "Bare10.Resources.Images.Achievements.tier_minute.svg"  },
            { TieredAchievement.ReachedGoalFiveDaysInARow, "Bare10.Resources.Images.Achievements.tier_5day.svg"  },
        };

        public static readonly ImageSource AchievementProgressCheckmark = ic_checkmark;
        public static readonly ImageSource AchievementProgressLocked = ic_lock;

        #endregion

        #region Goals
        public static ImageSource GoalBorder10 = ic_goal_10;
        public static ImageSource GoalBorder20 = ic_goal_20;
        public static ImageSource GoalBorder30 = ic_goal_30;
        #endregion

        #region Tabs
        public static readonly ImageSource TabHistoryActive = ic_tab_history_active;
        public static readonly ImageSource TabHistoryInactive = ic_tab_history_inactive;
        public static readonly ImageSource TabAchievementsActive = ic_tab_achievements_active;
        public static readonly ImageSource TabAchievementsInactive = ic_tab_achievements_inactive;
        public static readonly ImageSource TabGoalsActive = ic_tab_goals_active;
        public static readonly ImageSource TabGoalsInactive = ic_tab_goals_inactive;
        public static readonly ImageSource TabProfileActive = ic_tab_profile_active;
        public static readonly ImageSource TabProfileInactive = ic_tab_profile_inactive;
        #endregion

        #region Onboarding
        public static readonly ImageSource OnboardingShoe = ic_shoe_brisk;
        public static readonly ImageSource OnboardingWatch = ic_watch;
        #endregion

        #region Popup
        public static readonly ImageSource PopupClose = ic_close;
        public static readonly ImageSource PopupCloseSmall = ic_close_small;
        #endregion

        #region History
        public static readonly ImageSource HistoryInfo = ic_info;
        public static readonly ImageSource HistoryBriskShoe = ic_shoe_brisk_big;
        public static readonly ImageSource HistoryNormalShoe = ic_shoe_normal_big;
        #endregion

        #region Profile
        public static readonly ImageSource ProfileArrow = ic_arrow;
        public static readonly ImageSource ProfilePen = ic_pen;
        public static readonly ImageSource Helsedirektoratet = ic_helsedir;
        #endregion
    }
}
