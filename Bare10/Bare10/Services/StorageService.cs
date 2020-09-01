using Bare10.Models;
using Bare10.Models.Walking;
using Bare10.Services.Interfaces;
using Bare10.Utils.Comparers;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bare10.Content;
using Microsoft.AppCenter.Crashes;
using Bare10.Resources;

namespace Bare10.Services
{
    public class StorageService : IStorageService
    {
        private readonly SQLiteAsyncConnection connection;

        private static StorageService current = null;
        public static StorageService Current
        {
            get
            {
                if(current == null)
                {
                    current = new StorageService();
                }
                return current;
            }
        }

        public StorageService()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Config.DatabaseName);
            connection = new SQLiteAsyncConnection(path);

            //For current day
            connection.CreateTableAsync<WalkingDataPointModel>().Wait();
            //For walking history
            connection.CreateTableAsync<WalkingDayModel>().Wait();
            connection.CreateTableAsync<AchievementModel>().Wait();
            connection.CreateTableAsync<TieredAchievementModel>().Wait();
            connection.CreateTableAsync<AchievementTierProgressModel>().Wait();
            connection.CreateTableAsync<GoalStorageModel>().Wait();
            connection.CreateTableAsync<LastWalkingQuery>().Wait();
        }

        #region WalkingDataPoints - Recent walking
        public async Task<List<WalkingDataPointModel>> GetAllWalkingDataPoints()
        {
            return await connection.Table<WalkingDataPointModel>().ToListAsync();
        }

        public async Task<WalkingDataPointModel> GetLatestWalkingDataPoint()
        {
            return await connection.Table<WalkingDataPointModel>()
                .OrderByDescending(dp => dp.Stop)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateWalkingDataPoints(IEnumerable<WalkingDataPointModel> dataPoints)
        {
            await connection.UpdateAllAsync(dataPoints);
        }

        public async Task RemoveOldWalkingDataPoints()
        {
            await connection.Table<WalkingDataPointModel>()
                .Where(dp => dp.Start < DateTime.Today)
                .DeleteAsync();
        }

        public async Task<DateTime> GetTimeOfLatestWalkingDataPointOrMidnight()
        {
            var latestDataPoint = await connection.Table<LastWalkingQuery>()
             .FirstOrDefaultAsync();

            return latestDataPoint?.Timestamp ?? Settings.StartDate;
        }

        public async Task StoreTimeOfLatestWalkingDataPointOrMidnight(DateTime timestamp)
        {
            var latest = await GetTimeOfLatestWalkingDataPointOrMidnight();

            if (timestamp.Ticks > latest.Ticks)
            {
                await connection.InsertOrReplaceAsync(new LastWalkingQuery { Timestamp = timestamp });
            }
        }

        public async Task StoreWalkingDataPoints(List<WalkingDataPointModel> dataPoints)
        {
            await connection.InsertAllAsync(dataPoints);
        }

        public async Task StoreWalkingDataPoint(WalkingDataPointModel dataPoint)
        {
            await connection.InsertAsync(dataPoint);
        }
        #endregion

        #region WalkingDay - walking history

        public async Task RemoveOldWalkingDays()
        {
            DateTime threshold = DateTime.Today.AddDays(-Config.DaysToRetainData);
            await connection.Table<WalkingDayModel>()
                .Where(g => g.Day < threshold)
                .DeleteAsync();
        }

        public async Task AddOrUpdateWalkingDay(WalkingDayModel day)
        {
            await connection.InsertOrReplaceAsync(day);
        }

        public async Task<WalkingDayModel> GetWalkingDay(DateTime day)
        {
            try
            {
                return await connection.FindAsync<WalkingDayModel>(day);
            } catch (Exception e)
            {
                //TOOD possibly remove tracking on this?
                Crashes.TrackError(e);
                return null;
            }
        }

        public async Task<List<WalkingDayModel>> GetWalkingDays()
        {
            return await connection.Table<WalkingDayModel>().ToListAsync();
        }

        public async Task<List<WalkingDayModel>> GetWalkingDaysSinceInclusive(DateTime startDate)
        {
            return await connection.Table<WalkingDayModel>()
                .Where(dp => dp.Day >= startDate)
                .OrderBy(dp => dp.Day)
                .ToListAsync();
        }

        #endregion

        #region Achievements
        public async Task<List<AchievementModel>> GetAllCompletedAchievements()
        {
            return await connection.Table<AchievementModel>().ToListAsync();
        }

        public async Task StoreAchievement(AchievementModel achievement)
        {
            if(await connection.FindAsync<AchievementModel>((int)achievement.Id) != null)
            {
                Console.WriteLine("Attemped to store an existing achievement with id: {0}", achievement.Id);
            }
            else
            {
                await connection.InsertAsync(achievement);
            }
        }
        #endregion

        #region TieredAchievements

        public async Task<List<TieredAchievementModel>> GetAllTieredAchievements()
        {
            return await connection.Table<TieredAchievementModel>().ToListAsync();
        }

        public async Task<List<AchievementTierProgressModel>> GetAllAchievementProgress()
        {
            return await connection.Table<AchievementTierProgressModel>().ToListAsync();
        }
        
        public async Task UpdateTieredAchievement(TieredAchievementModel achievement)
        {
            await connection.InsertOrReplaceAsync(achievement, typeof(TieredAchievementModel));
        }
        
        public async Task UpdateTieredAchievementProgress(AchievementTierProgressModel progress)
        {
            await connection.InsertOrReplaceAsync(progress, typeof(AchievementTierProgressModel));
        }

        #endregion

        #region Goals
        private async Task RemoveOldGoalData()
        {
            DateTime threshold = DateTime.Today.AddDays(-Config.DaysToRetainData);
            await connection.Table<GoalStorageModel>()
                .Where(g => g.Day < threshold)
                .DeleteAsync();
        }

        public async Task StoreDailyGoalResult(GoalStorageModel dailyGoal)
        {
            if (await connection.FindAsync<GoalStorageModel>(dailyGoal.Day) != null)
            {
                Console.WriteLine("Updating goal with latest values");
                await connection.UpdateAsync(dailyGoal);
            }
            else
            {
                Console.WriteLine("Storing goal: {0}\n", dailyGoal);
                await connection.InsertAsync(dailyGoal);
            }
        }

        public async Task<GoalStorageModel> GetTodaysGoal()
        {
            return await connection.FindAsync<GoalStorageModel>(DateTime.Today);
        }

        public async Task<List<GoalStorageModel>> GetAllGoalResults()
        {
            await RemoveOldGoalData();
            return await connection.Table<GoalStorageModel>().ToListAsync();
        }
        #endregion

        #region Utility
        public async Task DeleteDataBaseTables(Table tables)
        {
            if(tables.HasFlag(Table.WalkingDataPoints))
            {
                await connection.DeleteAllAsync<WalkingDataPointModel>();
            }
            
            if(tables.HasFlag(Table.WalkingDays))
            {
                await connection.DeleteAllAsync<WalkingDayModel>();
            }

            if(tables.HasFlag(Table.Achievements))
            {
                await connection.DeleteAllAsync<AchievementModel>();
                await connection.DeleteAllAsync<TieredAchievementModel>();
                await connection.DeleteAllAsync<AchievementTierProgressModel>();
                await AchievementService.Current.LoadAndMergeStoredTiered(); // update achievementService
            }

            if(tables.HasFlag(Table.Goals))
            {
                await connection.DeleteAllAsync<GoalStorageModel>();
            }
        }
        #endregion

        #region Testing and debugging
        public async Task GenerateTestData(Table tables)
        {
            if(tables.HasFlag(Table.WalkingDataPoints))
            {
                await GenerateTestWalkingDataPoints();
            }
            
            if(tables.HasFlag(Table.WalkingDays))
            {
                await GenerateTestDailyWalkingData();
            }

            if(tables.HasFlag(Table.Achievements))
            {
                await GenerateTieredAchievementsTestData();
                //await GenerateAchievementsTestData();
            }

            if(tables.HasFlag(Table.Goals))
            {
                await GenerateGoalsTestData();
            }
        }
        
        public async Task GenerateTestWalkingDataPoints()
        {
            var testDataPoints = new List<WalkingDataPointModel>();

            DateTime now = DateTime.Now;

            //generate datapoints for today
            const int N_TESTDATAPOINTS = 100;
            for(int i = 0; i < N_TESTDATAPOINTS / 2; i++)
            {
                testDataPoints.Add(new WalkingDataPointModel(
                    now.AddMinutes(-(N_TESTDATAPOINTS * 2) + i),
                    now.AddMinutes(-(N_TESTDATAPOINTS * 2) + i + 1),
                    1.0f + ((i % 10) * 0.1f)
                 ));
            }

            //cull points that already exist in storage so we don't overwrite
            DateTime endRange = now.AddMinutes(-N_TESTDATAPOINTS);
            DateTime startRange = now.AddMinutes(-N_TESTDATAPOINTS * 2);

            var existingDataPoints = await connection.Table<WalkingDataPointModel>()
                .Where(dp => dp.Stop < endRange && dp.Stop > startRange)
                .ToListAsync();

            testDataPoints = testDataPoints.Except(existingDataPoints, new WalkingDataOverlapComparer()).ToList();

            //generate points for previous day
            const double DAY_IN_MINS = 60.0 * 24.0;
            for(int i = N_TESTDATAPOINTS / 2; i < N_TESTDATAPOINTS; i++)
            {
                testDataPoints.Add(new WalkingDataPointModel(
                    now.AddMinutes(-(N_TESTDATAPOINTS * 2) + i - DAY_IN_MINS),
                    now.AddMinutes(-(N_TESTDATAPOINTS * 2) + i + 1 - DAY_IN_MINS),
                    1.0f + ((i % 10) * 0.1f)
                 ));
            }

            //cull points that already exist in storage so we don't overwrite
            endRange = now.AddMinutes(-N_TESTDATAPOINTS - DAY_IN_MINS);
            startRange = now.AddMinutes(-N_TESTDATAPOINTS * 2 - DAY_IN_MINS);

            existingDataPoints = await connection.Table<WalkingDataPointModel>()
                .Where(dp => dp.Stop < endRange && dp.Stop > startRange)
                .ToListAsync();

            testDataPoints = testDataPoints.Except(existingDataPoints, new WalkingDataOverlapComparer()).ToList();

            await StoreWalkingDataPoints(testDataPoints);
        }

        public async Task GenerateTestDailyWalkingData()
        {
            const int DAYS_TO_GENERATE = 30;
            //Generate data for last 30 days
            var testDaily = new List<WalkingDayModel>(DAYS_TO_GENERATE);
            for(int i = 0; i < Config.DaysToRetainData; ++i)
            {
                testDaily.Add(new WalkingDayModel()
                {
                    Day = DateTime.Today.AddDays(-DAYS_TO_GENERATE + i),
                    MinutesBriskWalking = (uint)i % 40,
                    MinutesRegularWalking = 40 - ((uint)i % 40)
                });
            }

            var existingDaily = await GetWalkingDaysSinceInclusive(DateTime.Today.AddDays(-DAYS_TO_GENERATE));

            testDaily = testDaily.Except(existingDaily, new WalkingDayComparer()).ToList();

            await connection.InsertAllAsync(testDaily);
        }

        public async Task GenerateAchievementsTestData()
        {
            AchievementModel firstTen = new AchievementModel { Id = (int)Achievement.FirstTen, TimeAchieved = DateTime.Now, HasBeenAchieved = true };
            await StoreAchievement(firstTen);
            AchievementModel changedGoal = new AchievementModel { Id = (int)Achievement.SetHarderGoal, TimeAchieved = DateTime.Now.AddDays(-1), HasBeenAchieved = true };
            await StoreAchievement(changedGoal);
        }

        public async Task GenerateGoalsTestData()
        {
            const int DAYS_TO_GENERATE = 30;
            for (int i = 0; i < DAYS_TO_GENERATE; ++i)
            {
                GoalStorageModel goalResult = new GoalStorageModel
                {
                    Day = DateTime.Today.AddDays(-DAYS_TO_GENERATE + i),
                    HasBeenReached = (i % 14) >= 7,
                };

                await StoreDailyGoalResult(goalResult);
            }
        }

        //NOTE: Make sure this does not make it into production anywhere!
        public async Task GenerateTieredAchievementsTestData()
        {
            //NOTE: does not delete todays data
            await DeleteDataBaseTables(Table.GoalsAndAchievements | Table.WalkingDays);

#if true
            const int DAYS_TO_GENERATE = 31;
#else
            const int DAYS_TO_GENERATE = 21;
#endif

            List<WalkingDayModel> days = new List<WalkingDayModel>(DAYS_TO_GENERATE);

            for(int i = 0; i < DAYS_TO_GENERATE; ++i)
            {
                //generate goal models with this pattern
                // XXXXXXX 0 XXX 0
                //where X is goal reached and 0 is not reached
#if true
                bool reachedGoal = false;
                int period = i % 12;
                if (period <= 6 || (period >= 8 && period <= 10))
                    reachedGoal = true;
#else
                bool reachedGoal = true;
                //int period = i % 9;
                //if (period < 5)
                //    reachedGoal = true;
#endif


                DateTime date = DateTime.Today.AddDays(-DAYS_TO_GENERATE + i + 1);

                GoalStorageModel goal = new GoalStorageModel
                {
                    Day = date,
                    MinutesToReach = 10,
                    HasBeenReached = reachedGoal,
                };
                await StoreDailyGoalResult(goal);

                days.Add(new WalkingDayModel
                {
                    Day = date,
                    MinutesBriskWalking = (uint)(reachedGoal ? 30 : 1),
                    MinutesRegularWalking = (uint)(reachedGoal ? 1 : 30),
                });
            }
            await connection.InsertAllAsync(days);
        }

#endregion
    }
}
