using Bare10.Models;
using Bare10.Models.Walking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bare10.Services.Interfaces
{

    [Flags]
    public enum Table
    {
        None                    = 0,
        WalkingDataPoints       = 1 << 0,
        WalkingDays             = 1 << 1,
        Achievements            = 1 << 2,
        Goals                   = 1 << 3,

        /// <summary>
        /// Compound flag for WalkingDataPoints | WalkingDays
        /// </summary>
        AllWalking              = WalkingDataPoints | WalkingDays,
        /// <summary>
        /// Compound flag for Achievements | Goals
        /// </summary>
        GoalsAndAchievements     = Achievements | Goals,

        /// <summary>
        /// Compound flag for all tables
        /// </summary>
        All                     = AllWalking | GoalsAndAchievements,
    }

    public interface IStorageService
    {
        /// <summary>
        /// Retrieve all walking points
        /// </summary>
        /// <returns>List of all data points</returns>
        Task<List<WalkingDataPointModel>> GetAllWalkingDataPoints();

        /// <summary>
        /// Retrieve the datapoint with the most recent Stop field
        /// </summary>
        /// <returns>The walkingdatapoint in question</returns>
        Task<WalkingDataPointModel> GetLatestWalkingDataPoint();

        /// <summary>
        /// Update a set of datapoints
        /// </summary>
        /// <param name="dataPoints">The datapoints to update</param>
        /// <returns></returns>
        Task UpdateWalkingDataPoints(IEnumerable<WalkingDataPointModel> dataPoints);

        /// <summary>
        /// Retrieve the date and time of the latest stored walking data point
        /// </summary>
        /// <returns>The time of the latest data point OR midnight today if no points are stored</returns>
        Task<DateTime> GetTimeOfLatestWalkingDataPointOrMidnight();

        /// <summary>
        /// Set the date and time of the latest stored walking data point
        /// </summary>
        Task StoreTimeOfLatestWalkingDataPointOrMidnight(DateTime timestamp);

        /// <summary>
        /// Remove walking data points before midnight today
        /// </summary>
        /// <returns></returns>
        Task RemoveOldWalkingDataPoints();

        /// <summary>
        /// Store a list of walking data points
        /// </summary>
        /// <param name="dataPoints"></param>
        Task StoreWalkingDataPoints(List<WalkingDataPointModel> dataPoints);

        /// <summary>
        /// Store a single walking data point
        /// </summary>
        /// <param name="dataPoint">datapoint to store</param>
        /// <returns></returns>
        Task StoreWalkingDataPoint(WalkingDataPointModel dataPoint);


        /// <summary>
        /// Returns the complete list of walking day data
        /// NOTE: Should NOT contain current day
        /// NOTE: Side-effect: Removes all data older than Config.DaysToRetainData
        /// </summary>
        /// <returns></returns>
        Task<List<WalkingDayModel>> GetWalkingDays();

        /// <summary>
        /// Insert a list of walking days
        /// </summary>
        /// <param name="days">The days to insert</param>
        /// <returns></returns>
        Task AddOrUpdateWalkingDay(WalkingDayModel day);

        /// <summary>
        /// Removes all days stored older than Config.DaysToRetain
        /// </summary>
        /// <returns></returns>
        Task RemoveOldWalkingDays();

        /// <summary>
        /// Gets the walking days from the startdate until and including today
        /// </summary>
        /// <param name="startDate">The date from which to start the list</param>
        /// <returns></returns>
        Task<List<WalkingDayModel>> GetWalkingDaysSinceInclusive(DateTime startDate);

        /// <summary>
        /// Get a walking day
        /// </summary>
        /// <param name="day">The time of midnight of the day to get</param>
        /// <returns>The day requested</returns>
        Task<WalkingDayModel> GetWalkingDay(DateTime day);

        /// <summary>
        /// Gets all achievements that have been stored as achieved
        /// Note that the returned list does NOT hold the content data,
        /// so they have to be merged.
        /// </summary>
        /// <returns>A List of achieved Achievements</returns>
        Task<List<AchievementModel>> GetAllCompletedAchievements();

        /// <summary>
        /// Store an achievement
        /// </summary>
        /// <param name="achievement">The achievement to store</param>
        /// <returns></returns>
        Task StoreAchievement(AchievementModel achievement);

        /// <summary>
        /// Gets all stored achievements.
        /// Note that the returned list does not hold the content data,
        /// so they have to be merged
        /// </summary>
        /// <returns>A list of achievement progress</returns>
        Task<List<TieredAchievementModel>> GetAllTieredAchievements();

        /// <summary>
        /// Gets the progress for all achievements tiers
        /// </summary>
        /// <returns></returns>
        Task<List<AchievementTierProgressModel>> GetAllAchievementProgress();

        /// <summary>
        /// Stores a tiered achievement
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns></returns>
        Task UpdateTieredAchievement(TieredAchievementModel achievement);

        /// <summary>
        /// Stores the progression for an achievement tier
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task UpdateTieredAchievementProgress(AchievementTierProgressModel progress);

        //Task UpdateTieredAchievementProgress(TieredAchievementModel achievement);
        /// <summary>
        /// Store a daily goal
        /// </summary>
        /// <param name="dailyGoal"></param>
        /// <returns></returns>
        Task StoreDailyGoalResult(GoalStorageModel dailyGoal);

        /// <summary>
        /// Get the goal stored for today
        /// </summary>
        /// <returns>Todays goal</returns>
        Task<GoalStorageModel> GetTodaysGoal();

        /// <summary>
        /// Get all goal results - should yield maximum Config.DaysToRetainData entries 
        /// NOTE: Side-effect: Removes all data older than Config.DaysToRetainData
        /// </summary>
        /// <returns></returns>
        Task<List<GoalStorageModel>> GetAllGoalResults();

        /// <summary>
        /// Deletes the specified tables completely
        /// 
        /// WARNING: This deletes ALL data and this should only be used for testing
        /// </summary>
        /// <param name="tables">Flags for the tables to delete</param>
        /// <returns></returns>
        Task DeleteDataBaseTables(Table tables);

        #region Testing and Debug

        /// <summary>
        /// Runs generation of ALL test data available
        ///
        /// WARNING: Should ONLY be used for testing
        /// </summary>
        /// <returns></returns>
        Task GenerateTestData(Table tables);

        /// <summary>
        /// Generate unique datapoints for today, both brisk and regular
        ///
        /// WARNING: Only for use in testing!
        /// </summary>
        /// <returns></returns>
        Task GenerateTestWalkingDataPoints();

        /// <summary>
        /// Generates daily walking data for a month
        /// 
        /// WARNING: Only for use in testing
        /// </summary>
        /// <returns></returns>
        Task GenerateTestDailyWalkingData();


        /// <summary>
        /// Generates walkingdaymodel and goalstoragemodel data for testing tiered achievements
        ///
        /// WARNING: Only for use in testing
        /// </summary>
        /// <param name="clearPreviousData"></param>
        /// <returns></returns>
        Task GenerateTieredAchievementsTestData();
        #endregion
    }
}