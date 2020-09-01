using System;
using System.Collections.Generic;
using Bare10.Localization;

namespace Bare10.Content
{
    public enum TieredAchievement
    {
        ReachedGoal,
        ThirtyMinutes,
        ReachedGoalFiveDaysInARow,
    }

    public struct TieredAchievementDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CompletedDescription { get; set; }
    }

    public static class TieredAchievementExtensions
    {
        public static TieredAchievement[] GetAll()
        {
            //return Enum.GetValues(typeof(TieredAchievement)).Cast<TieredAchievement>().ToArray();
            // NOTE: Wanted in a specific order
            return new[]
            {
                TieredAchievement.ThirtyMinutes, 
                TieredAchievement.ReachedGoal,
                TieredAchievement.ReachedGoalFiveDaysInARow,
            };
        }

        public static TieredAchievementDetails Details(this TieredAchievement achievement)
        {
            if (TieredAchievements.ContainsKey(achievement))
                return TieredAchievements[achievement];
#if DEBUG
            throw new ArgumentOutOfRangeException(nameof(achievement), achievement, null);
#else
            return new TieredAchievementDetails()
            {
                Title = "Uknown",
                Description = "Uknown",
                CompletedDescription = "Uknown",
            };
#endif
        }

        public static int GetProgressiveGoal(this TieredAchievement achievement, Tier tier)
        {
            var goal = 0;
            foreach (var t in TierExtensions.GetAll())
            {
                if (t > tier)
                    break;

                goal += achievement.GetGoal(t);
            }
            return goal;
        }

        public static int GetGoal(this TieredAchievement achievement, Tier tier)
        {
            switch (achievement)
            {
                case TieredAchievement.ReachedGoal:
                    switch (tier)
                    {
                        case Tier.Bronze:
                            return 5;
                        case Tier.Silver:
                            return 15;
                        case Tier.Gold:
                            return 30;
                        case Tier.Complete:
                            return int.MaxValue;
                        default:
#if DEBUG
                            throw new ArgumentOutOfRangeException(nameof(tier), tier, null);
#else
                            return int.MaxValue;
#endif
                    }
                case TieredAchievement.ThirtyMinutes:
                    switch (tier)
                    {
                        case Tier.Bronze:
                            return 5;
                        case Tier.Silver:
                            return 15;
                        case Tier.Gold:
                            return 30;
                        case Tier.Complete:
                            return int.MaxValue;
                        default:
#if DEBUG
                            throw new ArgumentOutOfRangeException(nameof(tier), tier, null);
#else
                            return int.MaxValue;
#endif
                    }
                case TieredAchievement.ReachedGoalFiveDaysInARow:
                    switch (tier)
                    {
                        case Tier.Bronze:
                            return 1;
                        case Tier.Silver:
                            return 3;
                        case Tier.Gold:
                            return 6;
                        case Tier.Complete:
                            return int.MaxValue;
                        default:
#if DEBUG
                            throw new ArgumentOutOfRangeException(nameof(tier), tier, null);
#else
                            return int.MaxValue;
#endif
                    }
                default:
#if DEBUG
                    throw new ArgumentOutOfRangeException(nameof(achievement), achievement, null);
#else
                    return int.MaxValue;
#endif
            }
        }

        public static int GetSubGoal(this TieredAchievement achievement)
        {
            switch (achievement)
            {
                case TieredAchievement.ReachedGoal:
                    return 0;
                case TieredAchievement.ThirtyMinutes:
                    return 30;
                case TieredAchievement.ReachedGoalFiveDaysInARow:
                    return 5;
                default:
#if DEBUG
                    throw new ArgumentOutOfRangeException(nameof(achievement), achievement, null);
#else
                    return int.MaxValue;
#endif
            }
        }

        private static readonly Dictionary<TieredAchievement, Dictionary<Tier, string>> CompletedDictionary = 
            new Dictionary<TieredAchievement, Dictionary<Tier, string>>()
        {
            {
                TieredAchievement.ThirtyMinutes, new Dictionary<Tier, string>
                {
                    { Tier.Bronze, AppText.achievement_thirtyminutes_unlocked_bronze },
                    { Tier.Silver, AppText.achievement_thirtyminutes_unlocked_silver },
                    { Tier.Gold, AppText.achievement_thirtyminutes_unlocked_gold },
                }
            },
            {
                TieredAchievement.ReachedGoal, new Dictionary<Tier, string>
                {
                    { Tier.Bronze, AppText.achievement_reachedgoal_unlocked_bronze },
                    { Tier.Silver, AppText.achievement_reachedgoal_unlocked_silver },
                    { Tier.Gold, AppText.achievement_reachedgoal_unlocked_gold },
                }
            },
            {
                TieredAchievement.ReachedGoalFiveDaysInARow, new Dictionary<Tier, string>
                {
                    { Tier.Bronze, AppText.achievement_5days_unlocked_bronze },
                    { Tier.Silver, AppText.achievement_5days_unlocked_silver },
                    { Tier.Gold, AppText.achievement_5days_unlocked_gold },
                }
            },
        };

        public static string GetUnlockedText(this TieredAchievement achievement, Tier tier)
        {
            if (CompletedDictionary.ContainsKey(achievement))
            {
                if (CompletedDictionary[achievement].ContainsKey(tier))
                {
                    return CompletedDictionary[achievement][tier];
                }
#if DEBUG
                throw new ArgumentOutOfRangeException(nameof(tier), tier, null);
#else
                    return "";
#endif
            }
#if DEBUG
            throw new ArgumentOutOfRangeException(nameof(achievement), achievement, null);
#else
                    return "";
#endif
        }

        private static IReadOnlyDictionary<TieredAchievement, TieredAchievementDetails> TieredAchievements { get; } 
            = new Dictionary<TieredAchievement, TieredAchievementDetails>()
        {
            {
                TieredAchievement.ThirtyMinutes,
                new TieredAchievementDetails()
                {
                    Title = AppText.achievement_title_thirtyminutes,
                    Description = AppText.achievement_title_thirtyminutes_description,
                    CompletedDescription = AppText.achievement_title_thirtyminutes_description_completed,
                }
            },
            {
                TieredAchievement.ReachedGoal,
                new TieredAchievementDetails()
                {
                    Title = AppText.achievement_title_reachedgoal,
                    Description = AppText.achievement_title_reachedgoal_description,
                    CompletedDescription = AppText.achievement_title_reachedgoal_description_completed,
                }
            },
            {
                TieredAchievement.ReachedGoalFiveDaysInARow,
                new TieredAchievementDetails()
                {
                    Title = AppText.achievement_title_5days,
                    Description = AppText.achievement_title_5days_description,
                    CompletedDescription = AppText.achievement_title_5days_description_completed,
                }
            },
        };
    }
}
