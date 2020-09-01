using Bare10.Models;
using Bare10.Models.Walking;
using Bare10.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Bare10.Content;

#if DEBUG

#endif

namespace Bare10.Services
{
    public class AchievementService : IAchievementService
    {

        #region Progress
        private struct AchievementProgress
        {
            public List<DateTime> Progress;
            public int SubProgress;
        }

        private static void AddProgress(ref AchievementProgress progress, DateTime progressAt)
        {
            if(progress.Progress == null)
            {
                progress.Progress = new List<DateTime>();
            }
            progress.Progress.Add(progressAt);
        }
        #endregion

        //Check if Anders wants a list of achievements, or whether it should be queued like now
        //public event Action<Achievement> OnAchievementUnlocked;

        public event Action<TieredAchievement, Tier> OnTierChanged;
        public event Action<TieredAchievement> OnTierProgress;

        //private readonly List<AchievementModel> achievements;

        //NOTE: Part of update to new achievement system
        private List<TieredAchievementModel> tieredAchievements { get; } = TieredAchievementExtensions.GetAll()
            .Select(tieredAchievement => new TieredAchievementModel(tieredAchievement)).ToList();

        private List<AchievementTierProgressModel> tieredAchievementsProgress = new List<AchievementTierProgressModel>();

        private readonly IStorageService storageService;

        private bool isUpdating = false;
        private Task updateTask = null;

        //private bool hasChangedGoal = false;

        private static AchievementService current = null;
        public static AchievementService Current
        {
            get
            {
                if(current == null)
                {
                    current = new AchievementService();
                }
                return current;
            }
        }

        public AchievementService()
        {
            storageService = StorageService.Current;
            //List<AchievementModel> achievementModels = new List<AchievementModel>();
            //foreach(var a in Enum.GetValues(typeof(Achievement)))
            //{
            //    achievementModels.Add(new AchievementModel { Id = (int)a });
            //}
            //achievements = achievementModels;
            //Task.Run(LoadAndMergeStored).GetAwaiter().GetResult();

#if DEBUG
//#warning Remove this before release!
            //Run code as sync
            //Task.Run(async () => await storageService.DeleteDataBaseTables(Table.GoalsAndAchievements | Table.WalkingDays)).GetAwaiter().GetResult();
            //Task.Run(async () => await storageService.GenerateTieredAchievementsTestData()).GetAwaiter().GetResult();
#endif

            Task.Run(LoadAndMergeStoredTiered).GetAwaiter().GetResult();
        }

        //private async Task LoadAndMergeStored()
        //{
        //    var storedAchievements = await storageService.GetAllCompletedAchievements();

        //    foreach (var sa in storedAchievements)
        //    {
        //        var achievement = achievements.Find(a => (int)a.Id == sa.Id);
        //        achievement.HasBeenAchieved = sa.HasBeenAchieved;
        //        achievement.TimeAchieved = sa.TimeAchieved;
        //    }
        //}

        public async Task LoadAndMergeStoredTiered()
        {
            var storedAchievements = await storageService.GetAllTieredAchievements();

            foreach (var a in tieredAchievements)
            {
                var storedAchievement = storedAchievements.Find(sa => a.Id == sa.Id);
                if (storedAchievement != null)
                {
                    a.CurrentTier = storedAchievement.CurrentTier;
                }
                else
                {
                    a.CurrentTier = Tier.Bronze;
                }
            }

            tieredAchievementsProgress = await storageService.GetAllAchievementProgress();
        }

        public async Task CheckTieredAchievementsProgress(TodaysWalkingModel todaysWalking)
        {
            if(isUpdating || todaysWalking == null)
            {
                return;
            }
            isUpdating = true;

            if (updateTask == null || updateTask.IsCompleted)
            {
                updateTask = Task.Run(async () =>
                {
                    var walkingHistory = await storageService.GetWalkingDays();
                    var goalHistory = await storageService.GetAllGoalResults();

                    foreach (var a in tieredAchievements)
                    {
                        Tier tier = a.CurrentTier;

                        if (tier == Tier.Complete)
                            continue;

                        var storedProgress = GetProgressCurrentTier(a);
                        var relevantWalking = walkingHistory.Where(d => d.Day > storedProgress.TimeProgressAchieved);
                        var relevantGoals = goalHistory.Where(g => g.Day > storedProgress.TimeProgressAchieved);

                        switch ((TieredAchievement)a.Id)
                        {
                            case TieredAchievement.ReachedGoal:
                                {
                                    var goals = relevantGoals.Where(g => g.HasBeenReached);
                                    if (goals.Any())
                                    {
                                        AchievementProgress p = new AchievementProgress
                                        {
                                            Progress = goals.Select(g => g.Day).ToList(),
                                        };

                                        await AddAchievementProgress(a, p);
                                    }
                                }
                                break;
                            case TieredAchievement.ThirtyMinutes:
                                {
                                    var walking = relevantWalking.Where(d => d.MinutesBriskWalking + d.MinutesRegularWalking >= 30);
                                    var subProgress = (int) (todaysWalking.minutesBriskWalkToday +
                                                todaysWalking.minutesRegularWalkToday +
                                                todaysWalking.minutesUnknownWalkToday);

                                    AchievementProgress p = new AchievementProgress
                                    {
                                        Progress = walking.Select(w => w.Day).ToList(),
                                        SubProgress = subProgress,
                                    };

                                    await AddAchievementProgress(a, p);
                                }
                                break;
                            case TieredAchievement.ReachedGoalFiveDaysInARow:
                                {
                                    var progress = GetNumberOfConsectiveGoalDayGroups(relevantGoals, 5);
                                    await AddAchievementProgress(a, progress);
                                }
                                break;
                        }

                        await storageService.UpdateTieredAchievement(a);
                    }
                });
                await updateTask;
            }
            isUpdating = false;
        }

        private AchievementProgress GetNumberOfConsectiveGoalDayGroups(IEnumerable<GoalStorageModel> goals, int consecutiveDaysTarget)
        {
            AchievementProgress result = new AchievementProgress();

            int consecutiveDays = 0;
            foreach (var g in goals)
            {
                if (g.HasBeenReached)
                {
                    consecutiveDays++;
                    if(consecutiveDays == consecutiveDaysTarget)
                    {
                        AddProgress(ref result, g.Day);
                        //NOTE: We want to display a full graph in frontend if we completed today
                        consecutiveDays = (g.Day == DateTime.Today) ? consecutiveDaysTarget : 0;
                    }
                }
                else if(g.Day != DateTime.Today)
                {
                    consecutiveDays = 0;
                }
            }

            result.SubProgress = consecutiveDays;
            return result;
        }

        public AchievementTierProgressModel GetProgressForTier(int achievementId, Tier t)
        {
            try
            {
                return tieredAchievementsProgress.First(p => p.AchievementId == achievementId && p.Tier == t);
            }
            catch (InvalidOperationException)
            {
                var p = new AchievementTierProgressModel(achievementId, t);
                tieredAchievementsProgress.Add(p);
                return p;
            }
        }

        private AchievementTierProgressModel GetProgressCurrentTier(TieredAchievementModel a) =>
            GetProgressForTier(a.Id, a.CurrentTier);

        private async Task AddAchievementProgress(TieredAchievementModel achievement, AchievementProgress progress)
        {
            var storedProgress = GetProgressCurrentTier(achievement);

            if (progress.Progress != null)
            {
                foreach(var p in progress.Progress)
                {
                    storedProgress.Progress++;
                    storedProgress.TimeProgressAchieved = p;

                    if(storedProgress.Progress >= achievement.Type.GetGoal(achievement.CurrentTier))
                    {
                        storedProgress.DateTierAchieved = p;
                        await storageService.UpdateTieredAchievementProgress(storedProgress);
                        OnTierChanged?.Invoke(achievement.Type, achievement.CurrentTier);

                        achievement.CurrentTier++;
                        if(achievement.CurrentTier == Tier.Complete)
                        {
                            break;
                        }
                        storedProgress = GetProgressCurrentTier(achievement);
                        storedProgress.TimeProgressAchieved = p;
                        await storageService.UpdateTieredAchievementProgress(storedProgress);
                    }
                }
            }

            if (storedProgress.SubProgress != progress.SubProgress)
            {
                storedProgress.SubProgress = progress.SubProgress;
                OnTierProgress?.Invoke(achievement.Type);
            }
        }

        //public bool HasBeenAchieved(Achievement achievement)
        //{
        //    return achievements.Find(a => (Achievement)a.Id == achievement).HasBeenAchieved;
        //}

        //public async Task UserChangedGoal()
        //{
        //    hasChangedGoal = true;
        //    if (!achievements.Find(a => (Achievement)a.Id == Achievement.SetHarderGoal).HasBeenAchieved)
        //    {
        //        await UnlockAchievement(Achievement.SetHarderGoal);
        //    }
        //}

        //        public async Task CheckAllAchievementsCriteria(TodaysWalkingModel todaysWalking)
        //        {
        //            if(isUpdating || todaysWalking == null)
        //            {
        //                return;
        //            }
        //            isUpdating = true;

        //            #region Profiling
        //#if DEBUG
        //            Stopwatch timer = Stopwatch.StartNew();
        //#endif
        //            #endregion

        //            if (updateTask == null || updateTask.IsCompleted)
        //            {
        //                updateTask = Task.Run(async () =>
        //                {
        //                    try
        //                    {
        //                        //Get goals
        //                        var goalResults = await storageService.GetAllGoalResults();
        //                        var dailyHistory = await storageService.GetWalkingDays();

        //                        //check criteria on goals
        //                        foreach (Achievement a in Enum.GetValues(typeof(Achievement)))
        //                        {
        //                            if (HasBeenAchievedFirstTime(a, goalResults, todaysWalking.minutesBriskWalkToday, dailyHistory))
        //                            {
        //                                await UnlockAchievement(a);
        //                            }
        //                        }
        //                    } catch(Exception e)
        //                    {
        //                        #region Profiling
        //#if DEBUG
        //                        Debug.WriteLine("Failed to update achievements: \n{0}", e.Message);
        //#endif
        //                        #endregion
        //                        isUpdating = false;
        //                        Crashes.TrackError(e);
        //                    }
        //                });
        //                await updateTask;
        //            }

        //            isUpdating = false;
        //            #region Profiling
        //#if DEBUG
        //            Debug.WriteLine("Time spent on achievements update: {0} ms", timer.ElapsedMilliseconds);
        //#endif
        //            #endregion
        //        }

        //private async Task UnlockAchievement(Achievement a)
        //{
        //    AchievementModel achievement;
        //    achievement = achievements.Find(searchA => ((Achievement)searchA.Id == a));
        //    achievement.HasBeenAchieved = true;
        //    achievement.TimeAchieved = DateTime.Now;

        //    //store achieved
        //    await storageService.StoreAchievement(achievement);
        //    //callback on achievement update

        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        OnAchievementUnlocked?.Invoke(a);
        //    });

        //    Analytics.TrackEvent(AchievementUnlocked, new AchievementUnlockedArgs($"{a.ToString()} ({GetAchievementViewModel(a)?.Title})"));
        //}

        //public List<AchievementModel> GetAchievements()
        //{
        //    return achievements;
        //}

        public List<TieredAchievementModel> GetTieredAchievements() => tieredAchievements;

        //private bool HasBeenAchievedFirstTime(Achievement achievementToComplete, List<GoalStorageModel> goalResults, uint minutesBriskWalkToday, List<WalkingDayModel> dailyHistory)
        //{
        //    //early out
        //    if (achievements.Find(a => (Achievement)a.Id == achievementToComplete).HasBeenAchieved)
        //    {
        //        return false;
        //    }

        //    switch(achievementToComplete)
        //    {
        //        case Achievement.FirstTen: return minutesBriskWalkToday >= 10;
        //        case Achievement.SetHarderGoal: return hasChangedGoal;
        //        case Achievement.DailyGoalAchievedOneWeek: return HasGoalBeenReachedForConsecutiveDays(goalResults, 7);
        //        case Achievement.DailyGoalAchievedTwoWeeks: return HasGoalBeenReachedForConsecutiveDays(goalResults, 7 * 2);
        //        case Achievement.DailyGoalAchievedThreeWeeks: return HasGoalBeenReachedForConsecutiveDays(goalResults, 7 * 3);
        //        case Achievement.DailyGoalAchievedOneMonth: return HasGoalBeenReachedForConsecutiveDays(goalResults, 30);
        //        case Achievement.TwentyMinutesSixtyDays: return HasTargetMinutesBeenReachedForConsecutiveDays(20, 60, dailyHistory);
        //        case Achievement.ThirtyMinutesHundredDays: return HasTargetMinutesBeenReachedForConsecutiveDays(30, 100, dailyHistory);

        //       //NOTE: NO default to make sure all achievements are accounted for
        //    }

        //    return false;
        //}

        //private bool HasGoalBeenReachedForConsecutiveDays(List<GoalStorageModel> goalResults, uint daysInclusive)
        //{
        //    if(goalResults.Count == 0)
        //    {
        //        return false;
        //    }

        //    int consecutiveDays = 0;
        //    DateTime previousDate = goalResults[0].Day;
        //    for(int i = 1; i < goalResults.Count; i++)
        //    {
        //       if(goalResults[i].HasBeenReached &&
        //            (goalResults[i].Day - previousDate).TotalDays <= 1.0)
        //        {
        //            consecutiveDays++;
        //        }
        //        else
        //        {
        //            consecutiveDays = 0;
        //        }
        //        previousDate = goalResults[i].Day;
        //    }

        //    return consecutiveDays >= daysInclusive;
        //}

        //private bool HasTargetMinutesBeenReachedForConsecutiveDays(uint briskMinutesTarget, uint daysInclusive, List<WalkingDayModel> dailyHistory)
        //{
        //    if(dailyHistory.Count < daysInclusive)
        //    {
        //        return false;
        //    }

        //    int consecutiveDays = 0;
        //    DateTime previousDate = dailyHistory[0].Day;
        //    for(int i = 1; i < dailyHistory.Count; i++)
        //    {
        //       if(dailyHistory[i].MinutesBriskWalking >= briskMinutesTarget &&
        //            (dailyHistory[i].Day - previousDate).TotalDays <= 1.0)
        //        {
        //            consecutiveDays++;
        //        }
        //        else
        //        {
        //            consecutiveDays = 0;
        //        }
        //        previousDate = dailyHistory[i].Day;
        //    }

        //    return consecutiveDays >= daysInclusive;
        //}
    }
}
