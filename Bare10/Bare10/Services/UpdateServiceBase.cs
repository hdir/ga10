using Bare10.Localization;
using Bare10.Models.Walking;
using Bare10.Resources;
using Bare10.Services.Interfaces;
using Microsoft.AppCenter.Analytics;
using Plugin.LocalNotifications;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bare10.Content;

#if DEBUG
using Xamarin.Forms;
#else
using Microsoft.AppCenter.Crashes;
#endif

namespace Bare10.Services
{
    public abstract class UpdateServiceBase : IUpdateService
    {
        protected UpdateMode currentUpdateMode = UpdateMode.Background;

        public event Action<UpdateMode> UpdateModeChanged;
        public event Action UpdateCompleted;

        protected UpdateServiceBase()
        {
            IAchievementService achievementService = AchievementService.Current;

            achievementService.OnTierChanged += AchievementUnlocked;
        }

        private void AchievementUnlocked(TieredAchievement achievement, Tier tier)
        {
            BulletinService.Instance.AddBulletin(
                AppText.bulletin_achievement_title,
                $"{achievement.Details().CompletedDescription} {(achievement.GetGoal(tier) > 1 ? $"{achievement.GetGoal(tier)} ganger" : "")}\n\n" +
                $"{achievement.GetUnlockedText(tier)}", 
                Images.TieredAchievementCompleteIcon(achievement), 
                tier: (int)tier);

            if (!App.IsForeground && Settings.WillSendNotifications)
            {
                var id = (Config.NotificationIdAchievement + ((int)achievement * 10) + (int)tier);
                CrossLocalNotifications.Current.Show(AppText.notification_title, AppText.notification_achievement_unlocked, id);
            }
        }

        public async Task UpdateAllDataServices()
        {
            Debug.WriteLine($"Updating all data services in mode: {currentUpdateMode}");

            try
            {
                IWalkingDataService walkingDataService = CrossServiceContainer.WalkingDataService;

                if(!walkingDataService.GetIsConnectedToOSService())
                {
                    walkingDataService.ConnectToOSService();
                    return;
                }

                TodaysWalkingModel todaysWalking = null;

                if (walkingDataService != null)
                {
                    await walkingDataService.RequestUpdate();
                    todaysWalking = walkingDataService.GetTodaysHistory();
                }

                IGoalService goalService = GoalService.Current;

                if (goalService != null && todaysWalking != null)
                {
                    if (await goalService.CheckIfGoalCompleted(todaysWalking))
                    {
                        if (!App.IsForeground && Settings.WillSendNotifications)
                        {
                            var id = (Config.NotificationIdGoal + (int)Settings.CurrentGoal);
                            CrossLocalNotifications.Current.Show(AppText.notification_title, AppText.notification_goal_reached, id);
                            Analytics.TrackEvent(TrackingEvents.BackgroundEvents.NotificationSent);
                        }
                    }
                }

                IAchievementService achievementService = AchievementService.Current;

                if (achievementService != null && todaysWalking != null)
                {
                    //await achievementService.CheckAllAchievementsCriteria(todaysWalking);
                    await achievementService.CheckTieredAchievementsProgress(todaysWalking);
                }

                var lastWalkingDateEventDate = Settings.LastWalkingDataEventDate;
                if (lastWalkingDateEventDate.AddDays(1) < DateTime.Today)
                {
                    IStorageService storageService = StorageService.Current;
                    if (storageService != null)
                    {
                        var days = await storageService.GetWalkingDaysSinceInclusive(lastWalkingDateEventDate.AddDays(1));
                        var today = DateTime.Today;
                        var daysToTrack = days.Where(d => Settings.LastWalkingDataEventDate < d.Day && d.Day < today);
                        foreach (var d in daysToTrack)
                        {
                            Analytics.TrackEvent(TrackingEvents.BriskWalking, new TrackingEvents.WalkingDataArgs(d.Day.DayOfWeek, d.MinutesBriskWalking));
                            Analytics.TrackEvent(TrackingEvents.RegularWalking, new TrackingEvents.WalkingDataArgs(d.Day.DayOfWeek, d.MinutesRegularWalking));
                            
                            Analytics.TrackEvent(TrackingEvents.WalkingRaw, new TrackingEvents.WalkingRawDataArgs(d.Day.Date, d.MinutesBriskWalking, d.MinutesRegularWalking, d.MinutesUnknownWalking, Settings.UserHeight));
                        }
                        //Last date stored is always today - 1
                        Settings.LastWalkingDataEventDate = today.AddDays(-1);

                        // log history
                        var all = await storageService.GetWalkingDaysSinceInclusive(DateTime.MinValue);
                        Analytics.TrackEvent(TrackingEvents.DailyHistory, new TrackingEvents.WalkingDataArgs(all));
                    }
                }
                UpdateCompleted?.Invoke();

            }
            catch (Exception e)
            {
#if DEBUG
                Device.BeginInvokeOnMainThread(() => throw e);
#else
                Crashes.TrackError(e);
#endif
            }
        }

        public void SetUpdateMode(UpdateMode mode)
        {
            Console.WriteLine("Setting update mode to {0}", Enum.GetName(typeof(UpdateMode), mode));
            currentUpdateMode = mode;
            UpdateModeChanged?.Invoke(currentUpdateMode);
        }

        public UpdateMode GetUpdateMode()
        {
            currentUpdateMode = App.IsForeground ? UpdateMode.Foreground : UpdateMode.Background;
            return currentUpdateMode;
        }
    }
}
