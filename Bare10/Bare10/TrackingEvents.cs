using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Bare10.Models.Walking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bare10
{
    public static class TrackingEvents
    {
        public const string Started = "Started";
        public const string WalkingRaw = "Walking (Raw)";
        public const string BriskWalking = "Walking (Brisk)";
        public const string RegularWalking = "Walking (Regular)";
        public const string DailyHistory = "History";
        public const string WipedData = "Wiped Data";
        public const string NameChanged = "Name Changed";
        public const string HeightChanged = "Height Changed";
        public const string NavigatedTo = "Navigated To";
        public const string ItemTapped = "Item Tapped";
        public const string GoalChanged = "Goal Changed";
        public const string GoalAccomplished = "Goal Accomplished";
        public const string AchievementUnlocked = "Achievement Unlocked";
        public const string Error = "Error";
        public const string OnBoarding = "OnBoarding";
        public const string Background = "Background";
        public const string RequestedReview = "RequestedReview";

        public static class OnBoardingEvents
        {
            public const string Started = "Started";
            public const string Completed = "Completed";
            public const string Carousel = "Page";
        }

        public static class ItemsToTap
        {
            public const string Terms = "Terms";
            public const string Share = "Share";
            public const string BriskWalkExplanation = "Brisk Walk Explanation";
            public const string RegularWalkExplanation = "Regular Walk Explanation";
            public const string AchievementDetails = "Achievement Details";
        }

        public static class ErrorData
        {
            public const string SpeedDataRequestFailed = "Speed Data Request Failed";
            public const string SpeedDataSubscriptionFailed = "Speed Data Subscription Failed";
        }

        public static class BackgroundEvents
        {
            public const string BackgroundFetch = "Background Fetch";
            public const string NotificationSent = "Background Notification";
            public const string QueryTime = "Background Query Time";
            public const string FetchResult = "Background Fetch Result";
            public const string StartCapped = "Background Start was capped";
        }

        public class GenericEventArgs : Dictionary<string, string>
        {
            public GenericEventArgs(string key, string value)
            {
                Add(key, value);
            }
        }
        
        public class OnBoardingArgs : Dictionary<string, string>
        {
            public OnBoardingArgs(string onBoardingEvent)
            {
                Add("Event", onBoardingEvent);
            }

            public OnBoardingArgs(int pageIndex)
            {
                Add(OnBoardingEvents.Carousel, pageIndex.ToString());
            }
        }
        
        public class BackgroundArgs : Dictionary<string, string>
        {
            public BackgroundArgs(double milliseconds)
            {
                Add(BackgroundEvents.QueryTime, TimeToGroup(milliseconds));
            }

            public BackgroundArgs(string result)
            {
                Add(BackgroundEvents.FetchResult, result);
            }

            public BackgroundArgs(string eventName, double value)
            {
                Add(eventName, value.ToString(CultureInfo.InvariantCulture));
            }

            private string TimeToGroup(double milliseconds)
            {
                if (milliseconds > 60 * 1000) // show full time
                    return milliseconds.ToString();
                if (milliseconds > 30 * 1000)
                    return "<30, 60>";
                if (milliseconds > 10 * 1000)
                    return "<10, 30>";
                if (milliseconds > 5 * 1000)
                    return "<5, 10>";
                if (milliseconds > 2 * 1000)
                    return "<2, 5>";
                if (milliseconds > 1 * 1000)
                    return "<1, 2>";
                return "<0, 1>";
            }
        }

        public class WalkingRawDataArgs : Dictionary<string, string>
        {
            public WalkingRawDataArgs(DateTime date, uint brisk, uint regular, uint unknown, int height)
            {
                Add("all", JsonConvert.SerializeObject(new DataEvent()
                {
                    Date = date, 
                    Brisk = brisk,
                    Regular = regular,
                    Unknown = unknown,
                    Height = height,
                }));
            }

            private class DataEvent
            {
                public DateTime Date;
                public uint Brisk;
                public uint Regular;
                public uint Unknown;
                public int Height;
            }
        }

        public class WalkingDataArgs : Dictionary<string, string>
        {
            private const byte MAX_LENGTH = 30; // 30 days

            public WalkingDataArgs(DayOfWeek day, uint minutes)
            {
                Add(day.ToString(), TimeToGroup(minutes));
            }

            public WalkingDataArgs(List<WalkingDayModel> history)
            {
                if (!history.Any())
                    return;

                //var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
                //jsonResolver.IgnoreProperty(typeof(WalkingDayModel), nameof(WalkingDayModel.Day));
                //jsonResolver.RenameProperty(typeof(WalkingDayModel), nameof(WalkingDayModel.MinutesBriskWalking), "b");
                //jsonResolver.RenameProperty(typeof(WalkingDayModel), nameof(WalkingDayModel.MinutesRegularWalking), "r");
                //jsonResolver.RenameProperty(typeof(WalkingDayModel), nameof(WalkingDayModel.MinutesUnknownWalking), "u");


                // From doc https://docs.microsoft.com/en-us/appcenter/sdk/analytics/xamarin
                //You can send up to 200 distinct event names. 
                //Also, there is a maximum limit of 256 characters per event name and 125 characters per event property name and event property value.

                //var serializerSettings = new JsonSerializerSettings {ContractResolver = jsonResolver};

                //var json = JsonConvert.SerializeObject(history, serializerSettings);
                //Add("json", json);

                var brisk = history.Select(dp => dp.MinutesBriskWalking).ToArray();
                var regular = history.Select(dp => dp.MinutesRegularWalking).ToArray();
                var unknown = history.Select(dp => dp.MinutesUnknownWalking).ToArray();

                SubArrays(brisk, MAX_LENGTH, out var briskFirst, out var briskLast);
                Add("brisk_first", JsonConvert.SerializeObject(new DateTimeWalkingData(history.First().Day, briskFirst)));
                Add("brisk_last", JsonConvert.SerializeObject(new DateTimeWalkingData(history.Last().Day, briskLast)));

                SubArrays(regular, MAX_LENGTH, out var regularFirst, out var regularLast);
                Add("regular_first", JsonConvert.SerializeObject(new DateTimeWalkingData(history.First().Day, regularFirst)));
                Add("regular_last", JsonConvert.SerializeObject(new DateTimeWalkingData(history.Last().Day, regularLast)));

                SubArrays(unknown, MAX_LENGTH, out var unknownFirst, out var unknownLast);
                Add("unknown_first", JsonConvert.SerializeObject(new DateTimeWalkingData(history.First().Day, unknownFirst)));
                Add("unknown_last", JsonConvert.SerializeObject(new DateTimeWalkingData(history.Last().Day, unknownLast)));
            }

            private struct DateTimeWalkingData
            {
                public string Day { get; set; }
                public uint[] D { get; set; }

                public DateTimeWalkingData(DateTime date, uint[] data)
                {
                    Day = $"{date.DayOfYear}-{date.Year-2000}";
                    D = data;
                }
            }

            private void SubArrays(uint[] arr, uint maxElements, out uint[] first, out uint[] last)
            {
                first = arr.Length < MAX_LENGTH ? arr : arr.Take(MAX_LENGTH).ToArray();
                last = arr.Length < MAX_LENGTH ? arr : arr.Skip(arr.Length - MAX_LENGTH).Take(MAX_LENGTH).ToArray();
            }

            private string TimeToGroup(double minutes)
            {
                if (minutes > 60) // show full time
                    return minutes.ToString();
                if (minutes > 60)
                    return "60+";
                if (minutes > 30)
                    return "<30, 60>";
                if (minutes > 20)
                    return "<20, 30>";
                if (minutes > 10)
                    return "<10, 20>";
                if (minutes > 5)
                    return "<5, 10>";
                if (minutes > 0)
                    return "<0, 5>";
                return "0";
            }
        }

        public class NavigatedToArgs : Dictionary<string, string>
        {
            public NavigatedToArgs(string whichTab)
            {
                Add("Tab", whichTab);
            }
        }

        public class ErrorArgs : Dictionary<string, string>
        {
            public ErrorArgs(string type)
            {
                Add("Type", type);
            }
        }

        public class ItemTappedArgs : Dictionary<string, string>
        {
            public ItemTappedArgs(string whichItem)
            {
                Add("Item", whichItem);
            }
        }

        public class GoalChangedArgs : Dictionary<string, string>
        {
            public GoalChangedArgs(string whichGoal)
            {
                Add("Goal", whichGoal);
            }
        }

        public class GoalAccomplishedArgs : Dictionary<string, string>
        {
            public GoalAccomplishedArgs(string whichGoal)
            {
                Add("Goal", whichGoal);
            }
        }

        public class AchievementUnlockedArgs : Dictionary<string, string>
        {
            public AchievementUnlockedArgs(string whichAchievement)
            {
                Add("Achievement", whichAchievement);
            }
        }

        public class RequestedReviewArgs : Dictionary<string, string>
        {
            public RequestedReviewArgs(bool accepted)
            {
                Add("OpenedStore", accepted.ToString());
            }
        }
    }
    public class PropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores;
        private readonly Dictionary<Type, Dictionary<string, string>> _renames;

        public PropertyRenameAndIgnoreSerializerContractResolver()
        {
            _ignores = new Dictionary<Type, HashSet<string>>();
            _renames = new Dictionary<Type, Dictionary<string, string>>();
        }

        public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
                _ignores[type] = new HashSet<string>();

            foreach (var prop in jsonPropertyNames)
                _ignores[type].Add(prop);
        }

        public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            if (!_renames.ContainsKey(type))
                _renames[type] = new Dictionary<string, string>();

            _renames[type][propertyName] = newJsonPropertyName;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }

            if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
                property.PropertyName = newJsonPropertyName;

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!_ignores.ContainsKey(type))
                return false;

            return _ignores[type].Contains(jsonPropertyName);
        }

        private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
        {
            Dictionary<string, string> renames;

            if (!_renames.TryGetValue(type, out renames) || !renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
            {
                newJsonPropertyName = null;
                return false;
            }

            return true;
        }
    }
}
