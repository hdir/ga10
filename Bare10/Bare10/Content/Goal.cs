using System;
using System.Linq;
using Bare10.Localization;

namespace Bare10.Content
{
    public enum Goal
    {
        TenMinutes,
        TwentyMinutes,
        ThirtyMinutes,
    }

    public struct GoalDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public uint RequiredMinutes { get; set; }
    }

    public static class GoalExtensions
    {
        public static Goal[] GetAll()
        {
            return Enum.GetValues(typeof(Goal)).Cast<Goal>().ToArray();
        }

        public static uint RequiredMinutes(this Goal goal) => goal.Details().RequiredMinutes;

        public static GoalDetails Details(this Goal goal)
        {
            switch (goal)
            {
                case Goal.TenMinutes:
                    return new GoalDetails()
                    {
                        Title = AppText.goal_1x10_title,
                        Description = AppText.goal_1x10_description,
                        RequiredMinutes = 10,
                    };
                case Goal.TwentyMinutes:
                    return new GoalDetails()
                    {
                        Title = AppText.goal_2x10_title,
                        Description = AppText.goal_2x10_description,
                        RequiredMinutes = 20,
                    };
                case Goal.ThirtyMinutes:
                    return new GoalDetails()
                    {
                        Title = AppText.goal_3x10_title,
                        Description = AppText.goal_3x10_description,
                        RequiredMinutes = 30,
                    };
                default:
#if DEBUG
                    throw new ArgumentOutOfRangeException(nameof(goal), goal, null);
#else
                    return new GoalDetails()
                    {
                        Title = "Unknown",
                        Description = "Unknown",
                        RequiredMinutes = uint.MaxValue,
                    };
#endif
            }
        }
    }
}
