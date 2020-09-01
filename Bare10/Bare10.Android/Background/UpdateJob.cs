using Android.App;
using Android.App.Job;
using Android.Util;
using Bare10.Droid.Utils;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Plugin.LocalNotifications;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;

namespace Bare10.Droid.Background
{
    [Service(Name = "no.package.name.Background.UpdateJob",
        Permission = "android.permission.BIND_JOB_SERVICE")]
    public class UpdateJob : JobService
    {
        public override bool OnStartJob(JobParameters jobParams)
        {
                //NOTE: Lazy loading of service for platform specific version
#pragma warning disable CS0436 // Type conflicts with imported type
            IWalkingDataService walkingDataService = CrossWalkingDataService.Current;
            IUpdateService updateService = CrossUpdateService.Current;
#pragma warning restore CS0436 // Type conflicts with imported type

            CrossServiceContainer.SetWalkingDataService(walkingDataService);
            CrossServiceContainer.SetUpdateService(updateService);

            //NOTE: As mainactivity might not be run / we need to set this icon
            LocalNotificationsImplementation.NotificationIconId = Resource.Mipmap.notification_icon;
            Task.Run(async () =>
            {
                if (updateService != null)
                {
                    //NOTE: When Service is created - a separate instance of CrossUpdateService may be created where the default mode is foreground, with no lifecycle events
                    //NOTE: Thus we check if the app is running. Note the different packagenames for application and this service
                    if(!AppLifecycleUtil.IsRunning(this, Application.Context.PackageName))
                    {
                        updateService.SetUpdateMode(UpdateMode.Background);
                    }

                    Log.Debug("UpdateJob", "Running update job with updateservice address: " + updateService.ToString());

                    if (updateService.GetUpdateMode() == UpdateMode.Background)
                    {
#if DEBUG
                        Stopwatch timer = Stopwatch.StartNew();
#endif
                        await updateService.UpdateAllDataServices();

                        Analytics.TrackEvent(TrackingEvents.Background,
                            new TrackingEvents.BackgroundArgs(TrackingEvents.BackgroundEvents.BackgroundFetch));
#if DEBUG
                        Console.WriteLine("Completed background update of walking data and achievements in {0} ms", timer.ElapsedMilliseconds);
#endif
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine("Skipped background update since we're not in background");
#endif
                    }
                }

                //NOTE: Return whether or not we want to reschedule the job
                //NOTE: We do not, as JobInfo.Builder.setPeriodic() takes care of resheduling for us!
                JobFinished(jobParams, false);
            });

            //Continue this job until Jobfinished is called
            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            //Return whether or not we want to reschedule the job
            return true;
        }
    }
}