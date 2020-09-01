using Bare10.Models.Walking;
using Bare10.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bare10.Resources;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Bare10.Services
{
    public abstract class WalkingDataServiceBase : IWalkingDataService
    {
        protected static bool isUpdateInProgress = false;

        #region Interface Events
        public event Action Connected;
        public event Action UpdateCompleted;
        public event Action<TodaysWalkingModel> TodaysWalkingUpdated;
        public event Action<List<WalkingDayModel>> ThisWeekDaysUpdated;
        public event Action<List<WalkingDayModel>> LastThirtyDaysUpdated;
        #endregion

        protected TodaysWalkingModel todaysHistory = new TodaysWalkingModel();
        protected List<WalkingDayModel> weeklyHistory = new List<WalkingDayModel>();
        protected List<WalkingDayModel> dailyHistory = new List<WalkingDayModel>();

        #region Event wrappers for subclasses 

        protected void OnConnected()
        {
            Connected?.Invoke();
        }

        protected void OnTodaysWalkingUpdated(TodaysWalkingModel todaysWalking)
        {
            TodaysWalkingUpdated?.Invoke(todaysWalking);
        }

        protected void OnThisWeekDaysUpdated(List<WalkingDayModel> walkingDays)
        {
            ThisWeekDaysUpdated?.Invoke(walkingDays);
        }

        protected void OnDailyWalkingHistoryUpdated(List<WalkingDayModel> walkingDays)
        {
            LastThirtyDaysUpdated?.Invoke(walkingDays);
        }

        protected void OnUpdateCompleted()
        {
            UpdateCompleted?.Invoke();
        }
        #endregion

        #region Interface - should be overriden in subclasses

        public virtual bool HasCaughtUpToNow { get; } = false;
        public virtual void ConnectToOSService(bool backgroundMode = false)
        {

        }

        public virtual bool GetIsConnectedToOSService()
        {
            return false;
        }

        public virtual Task ConnectToOSServiceAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task RequestUpdate()
        {
            return Task.CompletedTask;
        }

        public TodaysWalkingModel GetTodaysHistory()
        {
            return todaysHistory;
        }

        public List<WalkingDayModel> GetWeekHistory()
        {
            return weeklyHistory;
        }

        public List<WalkingDayModel> GetThirtyDaysHistory()
        {
            return dailyHistory;
        }
        #endregion

        protected void SplitDataPointsOnHourBoundary(ref List<WalkingDataPointModel> dataPoints)
        {
            for(int i = 0; i < dataPoints.Count; ++i)
            {
                var dp = dataPoints[i];

                var hourDelta = dp.Stop.Hour - dp.Start.Hour;
                var nDpsAdded = 0;
                while(hourDelta != 0)
                {
                    var newStart = dp.Start
                        .AddHours(1)
                        .AddMinutes(-dp.Start.Minute)
                        .AddSeconds(-dp.Start.Second)
                        .AddMilliseconds(-dp.Start.Millisecond);

                    //NOTE: This causes us to drop a millisecond of counted time. Should be reasonable
                    var oldStop = newStart.AddMilliseconds(-1);
                    WalkingDataPointModel newDp = new WalkingDataPointModel(newStart, dp.Stop, dp.AverageSpeedMetersPerSecond);
                    dp.Stop = oldStop;
                    nDpsAdded++;
                    dataPoints.Insert(i + nDpsAdded, newDp);
                    dp = newDp;
                    hourDelta = dp.Stop.Hour - dp.Start.Hour;
                }
                i += nDpsAdded;
            }
        }

        protected void UpdateThisWeekFromThirtyDaysHistory()
        {
            //extract this week from dailyhistory
            weeklyHistory.Clear();
            for(int i = dailyHistory.Count - 1; i >= 0; --i)
            {
                weeklyHistory.Add(dailyHistory[i]);
                if(dailyHistory[i].Day.DayOfWeek == DayOfWeek.Monday)
                {
                    break;
                }
            }
            OnThisWeekDaysUpdated(weeklyHistory);
        }

        /// <summary>
        /// Get safe start time, limited by installation date and maximum 5 days in the past
        /// </summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static async Task<DateTime> GetQueryStartTime(DateTime endTime)
        {
            var last = await StorageService.Current.GetTimeOfLatestWalkingDataPointOrMidnight();

            // Earliest is time of installation
            var start = new DateTime(
                Math.Max(last.Ticks, Settings.StartDate.Ticks)
            );

            // NOTE: Queries will take exceptionally long if longer than 5 days
            // so the start date is capped to maximum 5 days in the past
            if ((endTime - start).TotalDays > 5)
            {
                Analytics.TrackEvent(TrackingEvents.Background, new TrackingEvents.BackgroundArgs(
                    TrackingEvents.BackgroundEvents.StartCapped, (DateTime.Now - start).TotalDays));
                start = endTime.AddDays(-5);
            }

            if (start >= endTime)
            {
                Crashes.TrackError(new Exception($"Last was in the future: start({start}) end({endTime})"));
                start = new DateTime(endTime.Ticks - 1);
            }

            return start;
        }
    }
}
