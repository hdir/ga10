using Bare10.Models;
using Bare10.Models.Walking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bare10.Content;

namespace Bare10.Services.Interfaces
{
    public interface IAchievementService
    {
        /// <summary>
        /// Event fired when achievement has been unlocked
        /// </summary>
        //event Action<Achievement> OnAchievementUnlocked;

        /// <summary>
        /// Event fired when Tier level has progressed
        /// </summary>
        event Action<TieredAchievement, Tier> OnTierChanged;

        /// <summary>
        /// Event fired when Tier progress has been made
        /// </summary>
        event Action<TieredAchievement> OnTierProgress;

        /// <summary>
        /// Check whether achievement has been achieved
        /// </summary>
        /// <param name="achievement">The achievement to check for</param>
        /// <returns>Whether or not the achievement has been achieved</returns>
        //bool HasBeenAchieved(Achievement achievement);

        /// <summary>
        /// Manual entry of goal related achievement. Should be called from the GoalsViewModel
        /// </summary>
        //Task UserChangedGoal();

        /// <summary>
        /// Checks all the achievements for whether they have met the achievment criterias
        /// </summary>
        /// <returns></returns>
        //Task CheckAllAchievementsCriteria(TodaysWalkingModel todaysWalking);

        /// <summary>
        /// Checks the progress of all tiered achievements
        /// </summary>
        /// <param name="todaysWalking">Last version of todays walking data</param>
        /// <returns></returns>
        Task CheckTieredAchievementsProgress(TodaysWalkingModel todaysWalking);

        /// <summary>
        /// Gets the progress for a specific tier for an achievement
        /// </summary>
        /// <param name="achievementId">The specific achievement id</param>
        /// <param name="tier">The specific tier</param>
        /// <returns></returns>
        AchievementTierProgressModel GetProgressForTier(int achievementId, Tier tier);

        /// <summary>
        /// Get all achievements
        /// </summary>
        /// <returns></returns>
        //List<AchievementModel> GetAchievements();

        /// <summary>
        /// Get all tiered achievements
        /// </summary>
        /// <returns></returns>
        List<TieredAchievementModel> GetTieredAchievements();
    }
}
