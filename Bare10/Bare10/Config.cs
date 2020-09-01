namespace Bare10
{
    public static class Config
    {
        /// <summary>
        /// How fast brisk pace is considered to be in m/s
        /// </summary>
        //public const float BriskPaceMetersPerSecond = 5000f / 3600f;  //5 km/h
        public const float BriskPaceMetersPerSecond = 4700 / 3600f;  //4.7 km/h

        /// <summary>
        /// How often to poll for data / check for goal completion / achievements
        /// </summary>
        public const long BackgroundUpdateIntervalMS = 1000 * 60 * 30;     //Once per 30 mins

        /// <summary>
        /// Days until app will ask user for a review
        /// </summary>
        public const int DaysBeforeRequestingReview = 10;

        /// <summary>
        /// Url to Questback
        /// </summary>
        public const string FeedbackUrl =
            "https://response.questback.com/helsedirektoratet/mmdnwuarjx";

        #region iOS
        public const double BackgroundAchievementAndGoalIntervalSeconds = 15;
        public const double SecondsForPollingPeriod = 15;
        public const int iOSForegroundUpdateIntervalMS = 1000 * 60;
        public const int iOSInitialForegroundUpdateIntervalMS = 200;
        #endregion

        #region Android
        /// <summary>
        /// How often to refresh data for a particular view when that view is shown
        /// </summary>
        public const int AndroidForegroundUpdateIntervalMS = 1000 * 5;
        public const int MaxTimeForWalkingDataPollMS = 20000;
        #endregion

        #region Database and storage

        /// <summary>
        /// SQLite database file name
        /// </summary>
        public const string DatabaseName = "gaa10.db";
        /// <summary>
        /// How many days to we retain data history
        /// </summary>
        public const int DaysToRetainData = 365;
        /// <summary>
        /// Set in case we need to update the db schema, allowing for table migrations
        /// </summary>
        public const int StorageSchemaVersion = 1;

        #endregion

        #region Notification ids

        //NOTE: These are spaced out so that individual classes can have up to 100 specific notification per topic
        public const int NotificationIdAchievement = 100;
        public const int NotificationIdGoal = 200;
        public const int NotificationIdConnectivity = 300;
        #endregion
    }
}
