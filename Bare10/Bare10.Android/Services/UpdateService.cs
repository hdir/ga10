using Android.App.Job;
using Android.Content;
using Android.OS;
using Bare10.Droid;
using Bare10.Droid.Background;
using System;

namespace Bare10.Services
{
    public class UpdateService : UpdateServiceBase
    {
        private static readonly Lazy<UpdateServiceBase> current = new Lazy<UpdateServiceBase>(() => new UpdateService());
        public static UpdateServiceBase Current => current.Value;

        private const int BACKGROUND_UPDATE_JOB_ID = 1;
        private MainActivity activity;

        private UpdateService()
        {
            if(MainActivity.Instance == null)
            {
                MainActivity.OnActivityCreated += OnMainActivityCreated;
            }
            else
            {
                SetupBackgroundUpdateJob();
            }
        }

        private void OnMainActivityCreated(MainActivity activity)
        {
            MainActivity.OnActivityCreated -= OnMainActivityCreated;
            this.activity = activity;
            SetupBackgroundUpdateJob();
        }

        private void SetupBackgroundUpdateJob()
        {
            var jobScheduler = (JobScheduler)activity.GetSystemService(Context.JobSchedulerService);

            var javaClass = Java.Lang.Class.FromType(typeof(UpdateJob));
            var componentName = new ComponentName(activity, javaClass);

            var builder = new JobInfo.Builder(BACKGROUND_UPDATE_JOB_ID, componentName);

            JobInfo appBackgroundUpdateJob;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                // 24+ requires minFlexMillis (5 minutes)
                appBackgroundUpdateJob = builder
                    .SetPeriodic(JobInfo.MinPeriodMillis, JobInfo.MinFlexMillis)
                    .SetPersisted(true)
                    .Build();
            }
            else
            {
                appBackgroundUpdateJob = builder
                    .SetPeriodic(Config.BackgroundUpdateIntervalMS)
                    .SetPersisted(true)
                    .Build();
            }

            jobScheduler.Schedule(appBackgroundUpdateJob);
        }
    }
}