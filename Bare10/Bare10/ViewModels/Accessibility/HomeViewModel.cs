using Bare10.Localization;
using Bare10.Models.Walking;
using Bare10.Resources;
using Bare10.Services.Interfaces;
using Bare10.ViewModels.Base;
using MvvmCross.Commands;
using System;
using System.Threading.Tasks;
using Bare10.Content;
using Xamarin.Forms;

namespace Bare10.ViewModels.Accessibility
{
    public class HomeViewModel : ViewModelBase
    {
        public enum WalkingDataLoadingStatus
        {
            Loading,
            Complete,
            Error,
        }

        private readonly IWalkingDataService walkingDataService;
        private readonly IUpdateService updateService;
        private bool isUpdateLoopRunning = false;

        #region Commands
        public IMvxAsyncCommand NavigateToHistory { get; private set; }
        public IMvxAsyncCommand NavigateToAchievements { get; private set; }
        public IMvxAsyncCommand NavigateToGoals { get; private set; }
        public IMvxAsyncCommand NavigateToProfile { get; private set; }
        #endregion

        #region Properties

        private WalkingDataLoadingStatus loadingStatus;
        public WalkingDataLoadingStatus LoadingStatus
        {
            get => loadingStatus;
            set
            {
                if(loadingStatus != value)
                {
                    loadingStatus = value;
                    RaisePropertyChanged(nameof(LoadingStatusText));
                }
            }

        }

        public string LoadingStatusText
        {
            get
            {
                switch(loadingStatus)
                {
                    case WalkingDataLoadingStatus.Loading: return AppText.history_loading_message;
                    case WalkingDataLoadingStatus.Complete: return AppText.history_loading_message_complete;
                    case WalkingDataLoadingStatus.Error: return AppText.history_loading_message_error;
                    default: return string.Empty;
                }
            }
        }

        public string HelloMessage
        {
            get => string.Format(AppText.accessibility_welcome_text, Settings.UserName);
        }

        private string todaysBriskWalkingMinutes;
        public string TodaysBriskWalkingMinutes
        {
            get => todaysBriskWalkingMinutes;
            set => SetProperty(ref todaysBriskWalkingMinutes, value);
        }

        private string todaysRegularWalkingMinutes;
        public string TodaysRegularWalkingMinutes
        {
            get => todaysRegularWalkingMinutes;
            set => SetProperty(ref todaysRegularWalkingMinutes, value);
        }

        private string todaysGoalProgress;
        public string TodaysGoalProgress
        {
            get => todaysGoalProgress;
            private set => SetProperty(ref todaysGoalProgress, value);
        }

        #endregion

        public HomeViewModel()
        {
            LoadingStatus = WalkingDataLoadingStatus.Loading;

            walkingDataService = CrossServiceContainer.WalkingDataService;
            updateService = CrossServiceContainer.UpdateService;

            walkingDataService.TodaysWalkingUpdated += UpdateDay;
            walkingDataService.UpdateCompleted += () => LoadingStatus = WalkingDataLoadingStatus.Complete;

            walkingDataService.ConnectToOSService();
            Settings.GoalChanged += OnGoalChanged;

            NavigateToHistory = new MvxAsyncCommand(async () => await NavigationService.Navigate<HistoryViewModel>());
            NavigateToAchievements = new MvxAsyncCommand(async () => await NavigationService.Navigate<TieredAchievementsViewModel>());
            NavigateToGoals = new MvxAsyncCommand(async () => await NavigationService.Navigate<GoalsViewModel>());
            NavigateToProfile = new MvxAsyncCommand(async () => await NavigationService.Navigate<ProfileViewModel>());

            App.Resumed += () =>
            {
                if (Settings.PreviousDay != DateTime.Now.Day)
                {
                    ResetDay();
                    Settings.PreviousDay = DateTime.Now.Day;
                }
            };

            updateService.UpdateModeChanged += mode =>
            {
                if (mode == UpdateMode.Foreground)
                {
                    StartUpdateLoop();
                }
            };

            StartUpdateLoop();
        }

        private void StartUpdateLoop()
        {
            if (!isUpdateLoopRunning)
            {
                RequestDataUpdates();
                if (Device.RuntimePlatform == Device.Android)
                {
                    Device.StartTimer(new TimeSpan(0, 0, 0, 0, Config.AndroidForegroundUpdateIntervalMS), RequestDataUpdates);
                } else if(Device.RuntimePlatform == Device.iOS)
                {
                    Device.StartTimer(new TimeSpan(0, 0, 0, 0, Config.iOSForegroundUpdateIntervalMS), RequestDataUpdates);
                }
                isUpdateLoopRunning = true;
            }
        }

        private bool RequestDataUpdates()
        {
            Task.Run(updateService.UpdateAllDataServices);
            isUpdateLoopRunning = updateService.GetUpdateMode() == UpdateMode.Foreground;
            return isUpdateLoopRunning;
        }

        private void OnGoalChanged(Goal goal)
        {
            if(LoadingStatus == WalkingDataLoadingStatus.Complete)
            {
                UpdateDay(walkingDataService.GetTodaysHistory());
            }
        }

        private void ResetDay()
        {
            TodaysRegularWalkingMinutes = "0 minutter";
            TodaysBriskWalkingMinutes = "0 minutter";
            TodaysGoalProgress = string.Format(AppText.accessibility_history_daily_goal_progress, 
                todaysBriskWalkingMinutes, Settings.CurrentGoal.RequiredMinutes());
        }

        private void UpdateDay(TodaysWalkingModel today)
        {
            TodaysRegularWalkingMinutes = today.minutesRegularWalkToday.ToString() + (today.minutesRegularWalkToday == 1 ? " minutt" : " minutter");
            TodaysBriskWalkingMinutes = today.minutesBriskWalkToday.ToString() + (today.minutesBriskWalkToday == 1 ? " minutt" : " minutter");

            if (today.minutesBriskWalkToday < Settings.CurrentGoal.RequiredMinutes())
            {
                TodaysGoalProgress = string.Format(AppText.accessibility_history_daily_goal_progress, todaysBriskWalkingMinutes, Settings.CurrentGoal.RequiredMinutes());
            }
            else
            {
                TodaysGoalProgress = string.Format(AppText.accessibility_history_daily_goal_reached, Settings.CurrentGoal.RequiredMinutes());
            }
        }
    }
}
