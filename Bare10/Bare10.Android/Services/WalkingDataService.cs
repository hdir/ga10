#if DEBUG
//#define DUMP_DATA
#endif
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Bare10.Droid;
using Bare10.Models.Walking;
using Bare10.Services.Interfaces;
using Bare10.Utils.Time;
using Bare10.Localization;
using Java.Util.Concurrent;
using Microsoft.AppCenter.Analytics;
using MvvmCross;
using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Plugin.Connectivity;
using Acr.UserDialogs;
using Plugin.Connectivity.Abstractions;
using Android.Content;
//using Android.Gms.Common;
using Plugin.LocalNotifications;
using Android.Gms.Fitness.Result;
using Android.Gms.Auth.Api.SignIn;
using System.Xml.Serialization;

namespace Bare10.Services
{
    public sealed class WalkingDataService : WalkingDataServiceBase
    {
        private HistoryClient fitnessHistoryClient;
        private RecordingClient fitnessRecordingClient;
        private MainActivity _activity = null;

        private bool _isConnected = false;
        private bool isWaitingForConnectivity = false;

        private Task checkSubscriptionsTask = null;

        private Task _updateTask;

#if DUMP_DATA
        string dumpFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "gaa10dump.xml");
        XmlSerializer dumpSerializer = new XmlSerializer(typeof(WalkingDataPointModel));
#endif

        #region Interface Overrides

        public override bool HasCaughtUpToNow => true;

        public override void ConnectToOSService(bool backgroundMode = false)
        {
            _activity = MainActivity.Instance;
            if (_activity == null)
            {
                MainActivity.OnActivityCreated += MainActivityCreated;
                return;
            }

            if (!_isConnected)
            {
                CreateGoogleFitClient();
            }
        }

        public override bool GetIsConnectedToOSService()
        {
            return _isConnected;
        }

        public override async Task RequestUpdate()
        {
            if (isUpdateInProgress || !_isConnected)
            {
                return;
            }
            isUpdateInProgress = true;

#if DEBUG
            var timer = Stopwatch.StartNew();
#endif

            if (_updateTask == null || _updateTask.IsCompleted)
            {
                _updateTask = Task.Run(async () =>
                {
                    try
                    {
                        await PollGoogleFitnessAPIAndStoreDataPoints();
                        await UpdateWalkingHistory();
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debug.WriteLine($"Failed to update walking history: \n {e.Message}");
#endif
                        isUpdateInProgress = false;
                        Crashes.TrackError(e);
                    }
                });

                await _updateTask;
            }

            OnUpdateCompleted();
            isUpdateInProgress = false;
#if DEBUG
            Debug.WriteLine("Time spent on walking data update: {0} ms", timer.ElapsedMilliseconds);
#endif
        }
        #endregion //Interface

        private void MainActivityCreated(MainActivity activity)
        {
            MainActivity.OnActivityCreated -= MainActivityCreated;
            _activity = activity;
            ConnectToOSService();
        }

        public void CreateGoogleFitClient()
        {
            if(_activity == null || isWaitingForConnectivity)
            {
                return;
            }

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
            {
                if (!_activity.HasActivityRecognition)
                {
                    _activity.ActivityRecognitionPermissionUpdated += OnActivityRecognitionPermissionUpdated;
                    isWaitingForConnectivity = true;
                    _activity.RequestActivityRecognition();
                    return;
                }
            }

            if (!_activity.HasAccessFineLocation)
            {
                _activity.AccessFineLocationPermissionUpdated += OnAccessFineLocationPermissionUpdated;
                isWaitingForConnectivity = true;
                _activity.RequestAccessFineLocation();
                return;
            }



            if(!_activity.HasGoogleFitnessPermissions)
            {
                _activity.GoogleFitnessPermissionsUpdated += OnFitnessPermissionsUpdated;
                isWaitingForConnectivity = true;
                _activity.RequestFitnessPermissions();
                return;
            }

            GoogleSignInAccount googleAccount = GoogleSignIn.GetLastSignedInAccount(_activity);

            fitnessHistoryClient = FitnessClass.GetHistoryClient(_activity, googleAccount);
            fitnessRecordingClient = FitnessClass.GetRecordingClient(_activity, googleAccount);

            if(checkSubscriptionsTask == null)
            {
                checkSubscriptionsTask = Task.Run(
                    async () =>
                {
                    bool hasSubscribedToSpeedData = await SubscribeToSpeedData();
                    if (hasSubscribedToSpeedData)
                    {
                        _isConnected = true;
                    }
                });
            }


            //create FitnessApi client, needs an activity
            //_client = new GoogleApiClient.Builder(_activity)
            //    .AddApi(FitnessClass.RECORDING_API)
            //    .AddApi(FitnessClass.HISTORY_API)
            //    .AddScope(FitnessClass.ScopeLocationReadWrite)
            //    .AddScope(FitnessClass.ScopeActivityReadWrite)
            //    .AddConnectionCallbacks(
            //        async bundle =>
            //        {

            //            if (Mvx.IoCProvider != null)
            //            {
            //                Mvx.IoCProvider.Resolve<IMissingPermissionService>().ResolvedMissingPermission(MissingPermission.AndroidFitnessAccountLink);
            //            }

            //            var isSubscribed = await SubscribeToSpeedData();
            //            if (!isSubscribed)
            //            {
            //                Analytics.TrackEvent(TrackingEvents.Error, new TrackingEvents.ErrorArgs(TrackingEvents.ErrorData.SpeedDataSubscriptionFailed));
            //            }

            //            _isConnected = true;
            //            CrossLocalNotifications.Current.Cancel(Config.NotificationIdConnectivity);
            //            OnConnected();

            //            await RequestUpdate();
            //        },
            //        suspensionReason =>
            //        {
            //            if (suspensionReason == GoogleApiClient.ConnectionCallbacks.CauseNetworkLost)
            //            {
            //                Crashes.TrackError(new Exception("Connection to Google Fitness lost due to Network Lost"));
            //            }
            //            else if (suspensionReason == GoogleApiClient.ConnectionCallbacks.CauseServiceDisconnected)
            //            {
            //                Crashes.TrackError(new Exception("Connection to Google Fitness lost due to Service Disconnected"));
            //            }
            //            else
            //            {
            //                Crashes.TrackError(new Exception("Connection to Google Fitness lost: " + suspensionReason));
            //            }
            //        })
            //    .AddOnConnectionFailedListener(
            //         async result =>
            //         {
            //             _isConnected = false;

            //             if (UpdateService.Current.GetUpdateMode() == UpdateMode.Foreground && !isWaitingForConnectivity)
            //             {
            //                 lastConnectionResult = result;
            //                 bool canReachGoogle = await CrossConnectivity.Current.IsRemoteReachable("http://www.google.com", 80, 1000);

            //                 if (!canReachGoogle)
            //                 {
            //                     bool hasCellular = CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.Cellular);
            //                     bool hasWifi = CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.WiFi);

            //                     CrossConnectivity.Current.ConnectivityTypeChanged += ConnectivityTypeChanged;
            //                     isWaitingForConnectivity = true;

            //                     await UserDialogs.Instance.AlertAsync(
            //                         "Du må ha en internett-forbindelse for å kunne hente ut dine gangdata fra Google Fit",
            //                         "Trenger nettforbindelse");

            //                     if (!hasWifi)
            //                     {
            //                         var intent = new Intent(Android.Provider.Settings.ActionWifiSettings);
            //                         intent.SetFlags(ActivityFlags.NewTask);
            //                         _activity.StartActivity(intent);
            //                     }
            //                     else if (!hasCellular)
            //                     {
            //                         var intent = new Intent(Android.Provider.Settings.ActionDataUsageSettings);
            //                         intent.SetFlags(ActivityFlags.NewTask);
            //                         _activity.StartActivity(intent);
            //                     }
            //                 }
            //                 else
            //                 {
            //                     _activity.GoogleFitnessAuthenticationUpdated -= OnGoogleFitnessAuthenticationUpdated;
            //                     _activity.GoogleFitnessAuthenticationUpdated += OnGoogleFitnessAuthenticationUpdated;
            //                     _activity.RequestGoogleFitnessAuthentication(lastConnectionResult);
            //                 }
            //             }
            //             else
            //             {
            //                 CrossLocalNotifications.Current.Show(AppText.notification_title, AppText.notification_google_fit_auth_failed, Config.NotificationIdConnectivity);
            //             }
            //         })
            //    .Build();

            //ConnectGoogleFitClient();
        }

        //public async void ConnectivityTypeChanged(object sender, ConnectivityTypeChangedEventArgs e)
        //{
        //    if (await CrossConnectivity.Current.IsRemoteReachable("http://www.google.com", 80, 1000))
        //    {
        //        CrossConnectivity.Current.ConnectivityTypeChanged -= ConnectivityTypeChanged;
        //        isWaitingForConnectivity = false;
        //        _activity.GoogleFitnessAuthenticationUpdated += OnGoogleFitnessAuthenticationUpdated;
        //        _activity.RequestGoogleFitnessAuthentication(lastConnectionResult);
        //    }
        //}

        //public void ConnectGoogleFitClient()
        //{
        //    //TODO: Remove after testing
        //    //Debug.WriteLine("Connecting to Google Fit");
        //    if(_client != null)
        //    {
        //        if(!_client.IsConnected && !_client.IsConnecting)
        //        {
        //            _client.Connect(GoogleApiClient.SignInModeRequired);
        //        }
        //    }
        //}

        /// <summary>
        /// Wrapper to avoid having to use an Action<bool> for missing permissions
        /// </summary>
        //private void ConnectToOSService()
        //{
        //    ConnectToOSService(UpdateService.Current.GetUpdateMode() == UpdateMode.Background);
        //}

        private void OnAccessFineLocationPermissionUpdated(bool hasAccess)
        {
            if (hasAccess)
            {
                _activity.AccessFineLocationPermissionUpdated -= OnAccessFineLocationPermissionUpdated;
                if (Mvx.IoCProvider != null)
                {
                    Mvx.IoCProvider.Resolve<IMissingPermissionService>().ResolvedMissingPermission(MissingPermission.AndroidFineLocation);
                }
                isWaitingForConnectivity = false;
                CreateGoogleFitClient();
            }
            else
            {
                if (Mvx.IoCProvider != null)
                {
                    Mvx.IoCProvider.Resolve<IMissingPermissionService>().ReportMissingPermission(
                        MissingPermission.AndroidFineLocation,
                        _activity.RequestAccessFineLocation);
                }
            }
        }

        private void OnActivityRecognitionPermissionUpdated(bool hasAccess)
        {
            if (hasAccess)
            {
                _activity.ActivityRecognitionPermissionUpdated -= OnActivityRecognitionPermissionUpdated;
                if (Mvx.IoCProvider != null)
                {
                    Mvx.IoCProvider.Resolve<IMissingPermissionService>().ResolvedMissingPermission(MissingPermission.AndroidActivityRecognition);
                }
                isWaitingForConnectivity = false;
                CreateGoogleFitClient();
            }
            else
            {
                if (Mvx.IoCProvider != null)
                {
                    Mvx.IoCProvider.Resolve<IMissingPermissionService>().ReportMissingPermission(
                        MissingPermission.AndroidActivityRecognition,
                        _activity.RequestActivityRecognition);
                }
            }
        }

        private void OnFitnessPermissionsUpdated(bool hasPermissions)
        {
            if (hasPermissions)
            {
                _activity.GoogleFitnessPermissionsUpdated -= OnFitnessPermissionsUpdated;
                if (Mvx.IoCProvider != null)
                {
                    Mvx.IoCProvider.Resolve<IMissingPermissionService>().ResolvedMissingPermission(MissingPermission.AndroidFitnessPermissions);
                }
                isWaitingForConnectivity = false;
                CreateGoogleFitClient();
            }
            else
            {
                if (Mvx.IoCProvider != null)
                {
                    Mvx.IoCProvider.Resolve<IMissingPermissionService>().ReportMissingPermission(
                        MissingPermission.AndroidFitnessPermissions,
                        _activity.RequestFitnessPermissions);
                }
            }
        }

        public async Task PollGoogleFitnessAPIAndStoreDataPoints()
        {
            if(!_isConnected)
            {
                CreateGoogleFitClient();
                return;
            }

            var endTimeMono = DateTime.Now;

            // start from last time walking data was collected
            var startTimeMono = await GetQueryStartTime(endTimeMono);
            //var startTimeMono = DateTime.Today.AddDays(-1);

            var activityDataPoints = await QueryActivityDataBetween(startTimeMono, endTimeMono);

            if (activityDataPoints != null)
            {
                var dataPoints = new List<WalkingDataPointModel>();

                foreach (var adp in activityDataPoints)
                {
                    var diff = adp.Stop.ToUnixTime() - adp.Start.ToUnixTime();

                    if (diff == 0)
                        continue;

                    //long timeAccountedFor = 0;
                    var speedPoints = await QuerySpeedDataBetween(adp.Start, adp.Stop);
                    if (speedPoints != null)
                    {
                        //timeAccountedFor = speedPoints.Sum(dp => (dp.Stop.ToUnixTime() - dp.Start.ToUnixTime()));
                        //var timeUnaccountedFor = diff - timeAccountedFor;

                        DateTime unaccountedStart = adp.Start;

                        speedPoints.Sort((dp1, dp2) => dp1.Start.CompareTo(dp2.Start));
                        List<WalkingDataPointModel> unaccountedDps = new List<WalkingDataPointModel>();
                        foreach(var dp in speedPoints)
                        {
                            var ticks = dp.Start - unaccountedStart;
                            if(ticks.Minutes > 0)
                            {
                                unaccountedDps.Add(new WalkingDataPointModel(unaccountedStart, dp.Start, true));
                            }
                            unaccountedStart = dp.Stop;
                        }

                        speedPoints.AddRange(unaccountedDps);
                        speedPoints.Sort((dp1, dp2) => dp1.Start.CompareTo(dp2.Start));
                        dataPoints.AddRange(speedPoints);
                    }

                }

                await StorageService.Current.StoreWalkingDataPoints(dataPoints);
            }
        }

        private async Task<List<WalkingDataPointModel>> QueryActivityDataBetween(DateTime bucketStart, DateTime bucketEnd)
        {
            var dailyActivityRequest = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeActivitySegment, DataType.AggregateActivitySummary)
                .BucketByTime(1, TimeUnit.Minutes)
                .SetTimeRange(bucketStart.ToUnixTime(), bucketEnd.ToUnixTime(), TimeUnit.Milliseconds)
                .Build();

            DataReadResponse activityResponse = null;
            var readTask = fitnessHistoryClient.ReadDataAsync(dailyActivityRequest);
            if(await Task.WhenAny(readTask, Task.Delay(30000)) == readTask)
            {
                // Task completed within timeout.
                // Consider that the task may have faulted or been canceled.
                // We re-await the task so that any exceptions/cancellation is rethrown.
                activityResponse = await readTask;
            }
            else
            {
                return new List<WalkingDataPointModel>();
            }

            if (activityResponse.Status.IsSuccess)
            {
                var activityDataPoints = activityResponse.Buckets
                    .SelectMany(b => b.DataSets)
                    .SelectMany(ds => ds.DataPoints)
                    .Select(adp =>
                    {
                        var start = adp.GetStartTime(TimeUnit.Milliseconds).DateTimeFromUnixTime();
                        var end = adp.GetEndTime(TimeUnit.Milliseconds).DateTimeFromUnixTime();


                        var activity = adp.GetValue(Field.FieldActivity).AsActivity();

                        // ON_FOOT // The user is on foot, walking or running.
                        // RUNNING // The user is running.
                        // RUNNING_JOGGING // The user is jogging.
                        // RUNNING_SAND // The user is running on sand.
                        // RUNNING_TREADMILL // The user is running on a treadmill.
                        // WALKING // The user is walking.
                        // WALKING_FITNESS // The user is walking at a moderate to high pace, for fitness.
                        // WALKING_NORDIC // The user is performing Nordic walking(with poles).
                        // WALKING_STROLLER // The user is walking while pushing a stroller.
                        // WALKING_TREADMILL // The user is walking on a treadmill

                        if (Regex.IsMatch(activity, "^(ON_FOOT|RUNNING|WALKING)", RegexOptions.IgnoreCase))
                        {
                            return new WalkingDataPointModel(start, end, 0);
                        }

                        return null;
                    })
                    .Where(adp => adp != null)
                    .ToList();
                return activityDataPoints;

            }
#if DEBUG
            Debug.WriteLine($"Failed to poll speed data: {activityResponse.Status.StatusMessage}");
#else
            Crashes.TrackError(new Exception($"Failed to poll speed data: {activityResponse.Status.StatusMessage}"), new Dictionary<string, string> { { "StatusMessage", activityResponse.Status.StatusMessage } });
#endif
            return null;
        }

        private async Task<List<WalkingDataPointModel>> QuerySpeedDataBetween(DateTime bucketStart, DateTime bucketStop)
        {
            var speedRequest = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeSpeed, DataType.AggregateSpeedSummary)
                .BucketByTime(1, TimeUnit.Minutes)
                .SetTimeRange(bucketStart.ToUnixTime(), bucketStop.ToUnixTime(), TimeUnit.Milliseconds)
                .Build();

            var speedResponse = await fitnessHistoryClient.ReadDataAsync(speedRequest);

            if (speedResponse.Status.IsSuccess)
            {
                var dataSets = speedResponse.Buckets.SelectMany(b => b.DataSets).ToList();

                var dataPoints = dataSets.SelectMany(ds => ds.DataPoints).ToList();

                return speedResponse.Buckets
                    .SelectMany(b => b.DataSets)
                    .SelectMany(ds => ds.DataPoints)
                    .Select(dp =>
                    {
                        //convert the times
                        var start = dp.GetStartTime(TimeUnit.Milliseconds).DateTimeFromUnixTime();
                        var end = dp.GetEndTime(TimeUnit.Milliseconds).DateTimeFromUnixTime();

                        if (start == end)
                            end = end.AddMinutes(1);

                        var average = dp.GetValue(Field.FieldAverage).AsFloat();

                        if (average > 0.1f && average < 12)
                        {
#if DUMP_DATA
                            WalkingDataPointModel res = new WalkingDataPointModel(start, end, average);
                            using (System.IO.FileStream fs = new System.IO.FileStream(dumpFilePath, System.IO.FileMode.Append))
                            {
                                dumpSerializer.Serialize(fs, res);
                            }
                            return res;

#else
                            return new WalkingDataPointModel(start, end, average);
#endif
                        }

                        return null;
                    })
                    .Where(dp => dp != null)
                    .ToList();
            }
#if DEBUG
            Debug.WriteLine($"Failed to poll speed data: {speedResponse.Status.StatusMessage}");
#else
            Crashes.TrackError(new Exception($"Failed to poll speed data: {speedResponse.Status.StatusMessage}"), new Dictionary<string, string> { { "StatusMessage", speedResponse.Status.StatusMessage } });
#endif
            return null;
        }

        private async Task UpdateWalkingHistory()
        {
            IStorageService storageService = StorageService.Current;

            //convert old data to daily data points
            var storedDataPoints = await storageService.GetAllWalkingDataPoints();
            var convertedDaysTasks = storedDataPoints
                .Where(dp => !dp.HasBeenCounted)
                .GroupBy(dp => dp.Start.Date)
                .Select(async g =>
                {

                    
                    var minutesBriskWalking = (uint) Math.Ceiling(g.Where(dp => dp.WasBrisk && !dp.WasUnknown).Sum(t => (t.Stop - t.Start).TotalMinutes));
                    var minutesRegularWalking = (uint) Math.Ceiling(g.Where(dp => !dp.WasBrisk && !dp.WasUnknown).Sum(t => (t.Stop - t.Start).TotalMinutes));
                    var minutesUnknownWalking = (uint)Math.Ceiling(g.Where(dp => dp.WasUnknown).Sum(t => (t.Stop - t.Start).TotalMinutes));

                    var existingDay = await storageService.GetWalkingDay(g.Key);
                    if (existingDay == null)
                    {
                        existingDay = new WalkingDayModel
                        {
                            Day = g.Key,
                            MinutesBriskWalking = minutesBriskWalking,
                            MinutesRegularWalking = minutesRegularWalking,
                            MinutesUnknownWalking = minutesUnknownWalking,
                        };
                    }
                    else
                    {
                        existingDay.MinutesBriskWalking += minutesBriskWalking;
                        existingDay.MinutesRegularWalking += minutesRegularWalking;
                        existingDay.MinutesUnknownWalking += minutesUnknownWalking;
                    }
                    await storageService.AddOrUpdateWalkingDay(existingDay);

                    return existingDay;
                });
            var convertedDays = await Task.WhenAll(convertedDaysTasks);

            //NOTE: We delete all here since they have all been accounted for above. Mixing merged dp with "raw" dps from Google Fitness is a recipe for disaster.
            await storageService.DeleteDataBaseTables(Table.WalkingDataPoints);

            //Flag todays as counted
            var todaysDataPoints = storedDataPoints
                .Where(dp => dp.Start >= DateTime.Today)
                .Select(dp =>
                {
                    dp.HasBeenCounted = true;
                    return dp;
                }).ToList();
            //await storageService.UpdateWalkingDataPoints(todaysDataPoints);
            await storageService.StoreWalkingDataPoints(todaysDataPoints);

            if (storedDataPoints.Count > 0)
            {
                await storageService.StoreTimeOfLatestWalkingDataPointOrMidnight(storedDataPoints
                    .OrderByDescending(dp => dp.Stop).First().Stop);
            }

            var todaysData = convertedDays.FirstOrDefault(day => day.Day == DateTime.Today);
            //Make sure we add days without walking
            if (todaysData == null)
            {
                todaysData = new WalkingDayModel
                {
                    Day = DateTime.Today,
                    MinutesBriskWalking = (uint)Math.Ceiling(todaysDataPoints.Where(dp => dp.WasBrisk && !dp.WasUnknown).Sum(t => (t.Stop - t.Start).TotalMinutes)),
                    MinutesRegularWalking = (uint)Math.Ceiling(todaysDataPoints.Where(dp => !dp.WasBrisk && !dp.WasUnknown).Sum(t => (t.Stop - t.Start).TotalMinutes)),
                    MinutesUnknownWalking = (uint)Math.Ceiling(todaysDataPoints.Where(dp => dp.WasUnknown).Sum(t => (t.Stop - t.Start).TotalMinutes)),
                };
                await storageService.AddOrUpdateWalkingDay(todaysData);
            }

            todaysHistory.minutesRegularWalkToday = todaysData.MinutesRegularWalking;
            todaysHistory.minutesBriskWalkToday = todaysData.MinutesBriskWalking;
            todaysHistory.minutesUnknownWalkToday = todaysData.MinutesUnknownWalking;
            todaysHistory.todaysWalking = MergeTimeAdjacentDataPoints(todaysDataPoints);
            OnTodaysWalkingUpdated(todaysHistory);

            await storageService.RemoveOldWalkingDays();

            const int THIRTY_DAYS = 30;
            var startDate = DateTime.Today.AddDays(-THIRTY_DAYS);
            dailyHistory = await storageService.GetWalkingDaysSinceInclusive(startDate);

            for (var i = 0; i < THIRTY_DAYS - 1; ++i)
            {
                var dayOffset = (i + 1);
                var day = DateTime.Today.AddDays(-dayOffset);
                //Check if data for day already exists
                if (!dailyHistory.Any(d => d.Day.Date == day.Date))
                {
                    dailyHistory.Add(new WalkingDayModel
                    {
                        Day = day.Date,
                        MinutesBriskWalking = 0,
                        MinutesRegularWalking = 0,
                        MinutesUnknownWalking = 0,
                    });
                }
            }

            dailyHistory.Sort((d1, d2) => d1.Day.CompareTo(d2.Day));
            OnDailyWalkingHistoryUpdated(dailyHistory);

            UpdateThisWeekFromThirtyDaysHistory();
        }

        private List<WalkingDataPointModel> MergeTimeAdjacentDataPoints(List<WalkingDataPointModel> dataPoints)
        {
            //early out
            if (dataPoints.Count == 0)
                return dataPoints;

            //Merge the adjacent datapoints
            var mergedWalking = new List<WalkingDataPointModel>(dataPoints);

            mergedWalking.Sort((dp1, dp2) => dp1.Start.CompareTo(dp2.Start));

            //NOTE: Starting second to last
            for(var i = mergedWalking.Count - 2; i >= 0; --i)
            {
                if (mergedWalking[i].WasBrisk == mergedWalking[i + 1].WasBrisk && mergedWalking[i].WasUnknown == mergedWalking[i + 1].WasUnknown)
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


       
        private async Task<bool> SubscribeToSpeedData()
        {
            var existingSubs = await fitnessRecordingClient.ListSubscriptionsAsync();

            //check if we're already subscribed to the recording of speed data
            if (!existingSubs.Any(s => s.DataType.Name == DataType.TypeSpeed.Name))
            {
                await fitnessRecordingClient.SubscribeAsync(DataType.TypeSpeed);

                existingSubs = await fitnessRecordingClient.ListSubscriptionsAsync();

                if(!existingSubs.Any(s => s.DataType.Name == DataType.TypeSpeed.Name))
                {
#if DEBUG
                    Debug.WriteLine($"Failed to subscribe to speed data");
#endif
                    Crashes.TrackError(new Exception($"Failed to subscribe to speed data"), new Dictionary<string, string> { { "StatusMessage", "unknown reason" } });
                    return false;
                }
                return true;
            }

            return true;
        }
    }
}
