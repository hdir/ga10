using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using Bare10.Content;
using Bare10.Services.Interfaces;
using Newtonsoft.Json;

namespace Bare10.Resources
{
    public static class Settings
    {
        private static ISettings XSettings => CrossSettings.Current;

        public static event Action<Goal> GoalChanged;

        /// <summary>
        /// Return whether or not onboarding has been completed
        /// </summary>
        public static bool IntroCompleted => !string.IsNullOrEmpty(UserName);

        /// <summary>
        /// The username shown in UI
        /// </summary>
        public static string UserName
        {
            get => XSettings.GetValueOrDefault(nameof(UserName), string.Empty);
            set => XSettings.AddOrUpdateValue(nameof(UserName), value);
        }

        /// <summary>
        /// The users height
        /// </summary>
        public static int UserHeight
        {
            get => XSettings.GetValueOrDefault(nameof(UserHeight), 0);
            set => XSettings.AddOrUpdateValue(nameof(UserHeight), value);
        }

        /// <summary>
        /// If app has already requested height
        /// </summary>
        public static bool HasRequestedHeight
        {
            get => XSettings.GetValueOrDefault(nameof(HasRequestedHeight), false);
            set => XSettings.AddOrUpdateValue(nameof(HasRequestedHeight), value);
        }

        /// <summary>
        /// Whether or not the application will show notifications
        /// </summary>
        public static bool WillSendNotifications
        {
            get => XSettings.GetValueOrDefault(nameof(WillSendNotifications), true);
            set => XSettings.AddOrUpdateValue(nameof(WillSendNotifications), value);
        }

        /// <summary>
        /// Current goal as set by the user
        /// </summary>
        public static Goal CurrentGoal
        {
            get => (Goal)XSettings.GetValueOrDefault(nameof(CurrentGoal), (int)Goal.TenMinutes);
            set
            {
                XSettings.AddOrUpdateValue(nameof(CurrentGoal), (int)value);
                GoalChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// The date at midnight of the time when the user completed onboarding
        /// OR deleted all existing data
        /// NOTE: Returns DateTime.Now if this has not been set
        /// </summary>
        public static DateTime StartDate
        {
            get => XSettings.GetValueOrDefault(nameof(StartDate), DateTime.Now).ToLocalTime();
            set => XSettings.AddOrUpdateValue(nameof(StartDate), value);
        }

        /// <summary>
        /// If app has been registered in app center (for tracking onboarding)
        /// </summary>
        public static bool RegisteredApp
        {
            get => XSettings.GetValueOrDefault(nameof(RegisteredApp), false);
            set => XSettings.AddOrUpdateValue(nameof(RegisteredApp), value);
        }

        /// <summary>
        /// If app allows tracking
        /// </summary>
        public static bool AllowTracking
        {
            get => XSettings.GetValueOrDefault(nameof(AllowTracking), false);
            set => XSettings.AddOrUpdateValue(nameof(AllowTracking), value);
        }

        /// <summary>
        /// If app has requested review from user
        /// </summary>
        public static bool HasRequestedReview
        {
            get => XSettings.GetValueOrDefault(nameof(HasRequestedReview), false);
            set => XSettings.AddOrUpdateValue(nameof(HasRequestedReview), value);
        }
        
        public static DateTime LastWalkingDataEventDate
        {
            get => XSettings.GetValueOrDefault(nameof(LastWalkingDataEventDate), DateTime.MinValue).ToLocalTime();
            set => XSettings.AddOrUpdateValue(nameof(LastWalkingDataEventDate), value);
        }

        public static List<Bulletin> PendingBulletins
        {
            get
            {
                var json = JsonConvert.SerializeObject(new List<Bulletin>());
                return JsonConvert.DeserializeObject<List<Bulletin>>(
                    XSettings.GetValueOrDefault(nameof(PendingBulletins), json));
            }
            set
            {
                var json = JsonConvert.SerializeObject(value);
                XSettings.AddOrUpdateValue(nameof(PendingBulletins), json);
            }
        }

        public static int StorageSchemaVersion
        {
            get => XSettings.GetValueOrDefault(nameof(StorageSchemaVersion), Config.StorageSchemaVersion);
            set => XSettings.AddOrUpdateValue(nameof(StorageSchemaVersion), value);
        }

        public static int PreviousDay
        {
            get => XSettings.GetValueOrDefault(nameof(PreviousDay), -1);
            set => XSettings.AddOrUpdateValue(nameof(PreviousDay), value);
        }

        public static bool CanRequestReview()
        {
            return !HasRequestedReview 
                   && (DateTime.Now - StartDate).TotalDays >= Config.DaysBeforeRequestingReview 
                   && PendingBulletins.Count == 0;
        }
    }
}
