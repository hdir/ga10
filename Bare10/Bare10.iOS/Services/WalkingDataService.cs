#if DEBUG
//#define DUMP_DATA
//#define DELETE_ON_STARTUP
#endif // DEBUG
using Bare10.iOS;
using Bare10.iOS.Utils;
using Bare10.Models.Walking;
using Bare10.Services.Interfaces;
using CoreMotion;
using Foundation;
using Microsoft.AppCenter.Crashes;
using MvvmCross;
using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
using System.IO;
#endif
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AppCenter.Analytics;
using UIKit;
using Xamarin.Forms;

namespace Bare10.Services
{
    public class WalkingDataService : WalkingDataServiceBase
    {
        //These stages happen from top to bottom
        private enum UpdateStage
        {
            LoadToday,
            LoadBackLog,
            Regular,
        }

        private const float BINARY_SEARCH_SECONDS_THRESHOLD = 5 * 60;

        private CMMotionActivityManager activityManager = new CMMotionActivityManager();
        private CMPedometer pedometer = new CMPedometer();
        private bool isConnected = false;

        private UpdateStage updateStage = UpdateStage.LoadToday;

        Task conversionTask = null;

#if DELETE_ON_STARTUP
        private bool hasDeleted = false;
#endif

#if DUMP_DATA
        string dumpFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "gaa10dump.xml");
        XmlSerializer dumpSerializer = new XmlSerializer(typeof(WalkingDataPointModel));
#endif

        #region Interface Overrides

        public override bool HasCaughtUpToNow => updateStage == UpdateStage.Regular;

        /// <summary>
        /// Sets up CMPedometer event callbacks and handlers
        /// </summary>
        public override void ConnectToOSService(bool backgroundMode = false)
        {
            if (isConnected)
            {
                return;
            }

            if(!backgroundMode)
            {
                AppDelegate.GetNotificationPermissions();
                RequestMotionPermissions();
            }

            isConnected = true;

        }

        public override bool GetIsConnectedToOSService()
        {
            return isConnected;
        }

        public override async Task RequestUpdate()
        {
            if (isUpdateInProgress || !isConnected || CMPedometer.AuthorizationStatus != CMAuthorizationStatus.Authorized)
            {
                return;
            }
            isUpdateInProgress = true;

#if DELETE_ON_STARTUP
            if(!hasDeleted)
            {
                await StorageService.Current.DeleteDataBaseTables(Table.All);
                hasDeleted = true;
            }
#endif

#if DEBUG
            DateTime profileStart = DateTime.Now;
#endif

            //NOTE: Run as iOS background task in case application is force closed so that no data is lost.
            nint taskId = UIApplication.SharedApplication.BeginBackgroundTask("UpdateWalkingHistory", () => isUpdateInProgress = false);


            List<WalkingDataPointModel> pedometerDataPoints = await GetPedometerData();
            try
            {
                // store walking data points
                await StorageService.Current.StoreWalkingDataPoints(pedometerDataPoints);

                // convert and delete walking points
                
                await ConvertAndStoreDataPoints();

                // store point from where to pick up updates
                switch(updateStage)
                {
                    case UpdateStage.LoadToday:
                        updateStage = UpdateStage.LoadBackLog;
                        break;
                    case UpdateStage.LoadBackLog:
                        var last = await StorageService.Current.GetLatestWalkingDataPoint();
                        if (last != null)
                        {
                            await StorageService.Current.StoreTimeOfLatestWalkingDataPointOrMidnight(last.Stop);
                        }
                        updateStage = UpdateStage.Regular;
                        break;
                    case UpdateStage.Regular:
                        if (pedometerDataPoints.Count > 0)
                        {
                            await StorageService.Current.StoreTimeOfLatestWalkingDataPointOrMidnight(pedometerDataPoints
                                .OrderByDescending(dp => dp.Stop).First().Stop);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }

            OnUpdateCompleted();
            isUpdateInProgress = false;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);

#if DEBUG
            var timeSpent = DateTime.Now - profileStart;
            Debug.WriteLine("Time spent on walking data update: {0} ms", timeSpent.TotalMilliseconds);
#endif
        }
#endregion //Interface Overrides

        private async Task<List<WalkingDataPointModel>> GetPedometerData()
        {
            var pedometerData = new List<WalkingDataPointModel>();
            var debugStart = DateTime.Now;


            DateTime from, to;


            switch(updateStage)
            {
                case UpdateStage.LoadToday:
#if DELETE_ON_STARTUP
                    DateTime queryStartTime = DateTime.Today.AddDays(-7);
#else
                    DateTime queryStartTime = await GetQueryStartTime(DateTime.Now);
#endif

                    //if the QueryStartTime is midnight today or later, just run regular updates
#if true
                    if (queryStartTime.CompareTo(DateTime.Today) >= 0)
                    {
                        updateStage = UpdateStage.Regular;
                        from = queryStartTime;
                    }
                    else
                    {
                        from = DateTime.Today;
                    }
#else
                    from = DateTime.Today;
#endif
                    to = DateTime.Now;
                    break;
                case UpdateStage.LoadBackLog:
                    to = DateTime.Today;
#if DELETE_ON_STARTUP
                    from = DateTime.Now.AddDays(-7);
#else
                    from = await GetQueryStartTime(to);
#endif
                    break;
                default:
                case UpdateStage.Regular:
                    to = DateTime.Now;
                    //start from last time walking data was collected
                    from = await GetQueryStartTime(to);
                    break;
            }

#if DEBUG
            Debug.WriteLine($"Running walkingdataservice update at stage: {updateStage}");
            Debug.WriteLine("Running query from {0} to {1}", from.ToLongTimeString(), to.ToLongTimeString());
#endif

            pedometerData.AddRange(await RecursiveBinarySearch(from, to));

            var completionTime = (DateTime.Now - debugStart).TotalMilliseconds;
#if DEBUG
            Debug.WriteLine($"Finished query in {completionTime} ms with a search range of {BINARY_SEARCH_SECONDS_THRESHOLD / 60f} minutes");
#endif

            if(UpdateService.Current.GetUpdateMode() == UpdateMode.Background)
            {
                Analytics.TrackEvent(TrackingEvents.Background, new TrackingEvents.BackgroundArgs(completionTime));
            }

            return pedometerData;
        }


        private async Task<List<WalkingDataPointModel>> RecursiveBinarySearch(DateTime from, DateTime to)
        {
            var pedometerData = new List<WalkingDataPointModel>();

            var interval = to - from;

            var periodData = await QueryPedometerPeriodAsync(from, to);
            if (periodData != null)
            {
                var periodInterval = periodData.Stop - periodData.Start;
                // Below threshold so we divide and gather the data
                if (periodInterval.TotalSeconds < BINARY_SEARCH_SECONDS_THRESHOLD)
                {

                    var start = periodData.Start;
                    var period = Config.SecondsForPollingPeriod;

                    //var wasWalkingOrRunning = await QueryActivityPeriodAsync(from, to);
                    do
                    {
                        var end = start.AddSeconds(period);
                        if (end > periodData.Stop)
                            end = periodData.Stop;

                        var data = await QueryPedometerPeriodAsync(start, end);

                        if (data != null && data.AverageSpeedMetersPerSecond < 12 && data.AverageSpeedMetersPerSecond > 0.2)
                        {
                            var wasWalkingOrRunning = await QueryActivityPeriodAsync(start, end);

                            if (wasWalkingOrRunning.HasValue && wasWalkingOrRunning.Value)
                                pedometerData.Add(data);
                        }

                        start = end;
                    } while (start < periodData.Stop);


                    return pedometerData;
                }
                // Divide again and recursively search each half
                else
                {

                    var diff = interval.TotalMilliseconds - periodInterval.TotalMilliseconds;
                    if (diff > 0)
                    {
                        from = periodData.Start;
                        to = periodData.Stop;
                        interval = periodInterval;
                    }

                    var mid = from.Add(TimeSpan.FromTicks(interval.Ticks / 2));

                    var left = await RecursiveBinarySearch(from, mid);
                    if (left != null)
                        pedometerData.AddRange(left);
                    var right = await RecursiveBinarySearch(mid, to);
                    if (right != null)
                        pedometerData.AddRange(right);
                }
            }

            // no periodData in this interval
            return pedometerData;
        }

        private async Task ConvertAndStoreDataPoints()
        {
            if(conversionTask == null || conversionTask.IsCompleted)
            {
                conversionTask = Task.Run(async () =>
                {
#if DEBUG
                    Stopwatch timer = Stopwatch.StartNew();
#endif
                    IStorageService storageService = StorageService.Current;
                    var dataPoints = await storageService.GetAllWalkingDataPoints();
                    //MergeWalkingDataPoints(ref dataPoints);

                    var daysConverted = dataPoints
                    .Where(dp => !dp.HasBeenCounted)
                    .GroupBy(dp => dp.Start.Date)
                    .Select(g =>
                    {
                        float minutesBrisk = g
                         .Where(dp => dp.WasBrisk)
                         .Aggregate(0f, (acc, dp) => acc + (float)(dp.Stop - dp.Start).TotalSeconds) / 60f;

                        
                        float minutesRegular = g
                         .Where(dp => !dp.WasBrisk)
                         .Aggregate(0f, (acc, dp) => acc + (float)(dp.Stop - dp.Start).TotalSeconds) / 60f;


                        return new WalkingDayModel
                        {
                            Day = g.Key,
                            MinutesBriskWalking = (uint)Math.Ceiling(minutesBrisk),
                            MinutesRegularWalking = (uint)Math.Ceiling(minutesRegular),
                        };
                    }).ToList();

                    foreach (var day in daysConverted)
                    {
                        //Make sure that we update existing days
                        var existingDay = await storageService.GetWalkingDay(day.Day);
                        if (existingDay != null)
                        {
                            day.MinutesBriskWalking += existingDay.MinutesBriskWalking;
                            day.MinutesRegularWalking += existingDay.MinutesRegularWalking;
                        }
                        //Store updated day
                        await storageService.AddOrUpdateWalkingDay(day);
                    }

                    //NOTE: Remove all datapoints as they now have been accounted for and todays merged datapoints will be resaved
                    await storageService.DeleteDataBaseTables(Table.WalkingDataPoints);

                    var todaysDataPoints = dataPoints
                        .Where(dp => dp.Start >= DateTime.Today)
                        .ToList();

                    //make sure we set them to counted
                    foreach (var dp in todaysDataPoints)
                    {
                        dp.HasBeenCounted = true;
                    }
                    await storageService.StoreWalkingDataPoints(todaysDataPoints);

                    var today = await storageService.GetWalkingDay(DateTime.Today);
                    if (today == null)
                    {
                        today = new WalkingDayModel
                        {
                            Day = DateTime.Today,
                            MinutesBriskWalking = 0,
                            MinutesRegularWalking = 0
                        };
                        daysConverted.Add(today);
                    }
                    todaysHistory.minutesBriskWalkToday = today.MinutesBriskWalking;
                    todaysHistory.minutesRegularWalkToday = today.MinutesRegularWalking;
                    todaysHistory.todaysWalking = MergeTimeAdjacentDataPoints(todaysDataPoints);
                    OnTodaysWalkingUpdated(todaysHistory);

                    await storageService.RemoveOldWalkingDays();
                    const int THIRTY_DAYS = 30;
                    var daysInStorage = await storageService.GetWalkingDaysSinceInclusive(DateTime.Today.AddDays(-THIRTY_DAYS));

                    daysConverted = daysConverted.Union(daysInStorage).ToList();

                    //fill in missing days from last 30
                    for (int i = 0; i < THIRTY_DAYS - 1; ++i)
                    {
                        var dayOffset = (i + 1);
                        DateTime day = DateTime.Today.AddDays(-dayOffset);
                        //Check if data for day already exists
                        if (!daysConverted.Any(d => d.Day.Date == day.Date))
                        {
                            daysConverted.Add(new WalkingDayModel
                            {
                                Day = day.Date,
                                MinutesBriskWalking = 0,
                                MinutesRegularWalking = 0
                            });
                        }
                    }

                    dailyHistory = daysConverted;
                    dailyHistory.Sort((dA, dB) => dA.Day.CompareTo(dB.Day));
                    OnDailyWalkingHistoryUpdated(dailyHistory);

                    //Make absolutely sure they are in order
                    UpdateThisWeekFromThirtyDaysHistory();
#if DEBUG
                    Debug.WriteLine("Running datapoint conversion took {0} ms", timer.ElapsedMilliseconds);
#endif

                });
                await conversionTask;
            }
        }

        private List<WalkingDataPointModel> MergeTimeAdjacentDataPoints(List<WalkingDataPointModel> dataPoints)
        {
            //early out
            if (dataPoints.Count == 0)
                return dataPoints;

            //Merge the adjacent datapoints
            List<WalkingDataPointModel> mergedWalking = new List<WalkingDataPointModel>(dataPoints);

            //NOTE: Starting second to last
            for(int i = mergedWalking.Count - 2; i >= 0; --i)
            {
                if (mergedWalking[i].WasBrisk == mergedWalking[i + 1].WasBrisk)
                {
                    var minuteDelta = mergedWalking[i + 1].Start.Minute - mergedWalking[i].Stop.Minute;
                    var hourDelta = mergedWalking[i + 1].Start.Hour - mergedWalking[i].Stop.Hour;
                    if(minuteDelta <= 1 && hourDelta == 0)
                    {
                        mergedWalking[i].Stop = mergedWalking[i + 1].Stop;
                        //NOTE: discarding average of data points as these are only used for frontend graphs
                        mergedWalking.RemoveAt(i + 1);
                    }
                }
            }

            SplitDataPointsOnHourBoundary(ref mergedWalking);

            return mergedWalking;
        }

        //private void MergeWalkingDataPoints(ref List<WalkingDataPointModel> dataPoints)
        //{
        //    dataPoints.Sort((dp1, dp2) => dp1.Start.CompareTo(dp2.Start));
        //    for(int i = dataPoints.Count - 2; i >= 0; --i)
        //    {
        //        var current = dataPoints[i];
        //        var next = dataPoints[i + 1];

        //        if (next.Start >= DateTime.Today)
        //        {
        //            const double MERGE_THRESHOLD_SECONDS = 1;

        //            if (next.WasBrisk == current.WasBrisk)
        //            {
        //                if (Math.Abs((next.Start - current.Stop).TotalSeconds) <= MERGE_THRESHOLD_SECONDS)
        //                {
        //                    current.Stop = next.Stop;
        //                    dataPoints.Remove(next);
        //                }
        //            }
        //        }
        //    }
        //}

        private async Task<bool?> QueryActivityPeriodAsync(DateTime from, DateTime to)
        {
            try
            {
                bool wasWalkingOrRunning = false;
                var rawDataPoints = await activityManager.QueryActivityAsync(from.ToNSDate(), to.ToNSDate(), NSOperationQueue.MainQueue);
                foreach(var dp in rawDataPoints)
                {
                    if (dp.Walking || dp.Running)
                        wasWalkingOrRunning = true;
                }
                return wasWalkingOrRunning;

            }catch(Exception e)
            {
#if DEBUG
                Debug.WriteLine("Failed to query activity datapoint {0}-{1}: {2}", from.ToLongTimeString(), to.ToLongTimeString(), e.Message);
#endif
            }

            return null;
        }

        private async Task<WalkingDataPointModel> QueryPedometerPeriodAsync(DateTime from, DateTime to)
        {
            try
            {
                var rawDataPoint = await pedometer.QueryPedometerDataAsync(from.ToNSDate(), to.ToNSDate());
                if(rawDataPoint.AverageActivePace != null)
                {
                    var start = rawDataPoint.StartDate.ToDateTime();
                    var end = rawDataPoint.EndDate.ToDateTime();

                    var averagePace = rawDataPoint.AverageActivePace.FloatValue;
                    var distance = rawDataPoint.Distance.FloatValue;
                    if (averagePace > 0)
                    {
                        //NOTE: This is for some reason measured in SECONDS / METER, so we flip it.
                        averagePace = 1 / averagePace;
                        return new WalkingDataPointModel(start, end, averagePace);
                    } else if (distance > 0)
                    {
                        averagePace = distance / (float)(end - start).TotalSeconds;
                        return new WalkingDataPointModel(start, end, averagePace);
                    }
                }
            } catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("Failed to query pedometer datapoint {0}-{1}: {2}", from.ToLongTimeString(), to.ToLongTimeString(), e.Message);
#endif
            }
            return null;
        }

        private void RequestMotionPermissions()
        {
            Task.Run(async () =>
            {
                IMissingPermissionService permissionService = null;
                if (Mvx.IoCProvider != null)
                {
                    permissionService = Mvx.IoCProvider.Resolve<IMissingPermissionService>();
                    if(permissionService != null)
                    {
                        bool allowed = false;
                        try
                        {
                            var result = await new CMMotionActivityManager().QueryActivityAsync(NSDate.DistantPast, NSDate.DistantFuture, NSOperationQueue.MainQueue);
                            if (result != null)
                            {
                                allowed = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Crashes.TrackError(e);
                        }

                        if (allowed)
                        {
                            permissionService.ResolvedMissingPermission(MissingPermission.iOSMotionAndActivity);
                        }
                        else
                        {
                            Action openSettings = new Action(
                                () =>
                                {
                                    var settingsUri = new NSUrl("app-settings:");
                                    if (UIApplication.SharedApplication.CanOpenUrl(settingsUri))
                                    {
                                        UIApplication.SharedApplication.OpenUrl(settingsUri);
                                    }
                                });
                            permissionService.ReportMissingPermission(MissingPermission.iOSMotionAndActivity, openSettings);
                        }

                    }
                }

            });
        }
        
    }
}
