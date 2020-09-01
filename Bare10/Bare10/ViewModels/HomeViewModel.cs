using Bare10.Services.Interfaces;
using Bare10.ViewModels.Base;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Bare10.Localization;
using Bare10.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Bare10.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        #region Enums
        public enum View
        {
            Home,
            Achievements,
            Goals,
            Profile,
        }
        #endregion //Enums

        private readonly IUpdateService _updateService;

        private bool _isUpdateLoopRunning;

        #region Properties
        private Dictionary<View, ToolbarViewModelBase> Views { get; }

        public HistoryViewModel HistoryViewModel => (HistoryViewModel)Views[View.Home];
        //public AchievementsViewModel AchievementsViewModel => (AchievementsViewModel)Views[View.Achievements];
        public TieredAchievementsViewModel AchievementsViewModel => (TieredAchievementsViewModel)Views[View.Achievements];
        public GoalsViewModel GoalsViewModel => (GoalsViewModel)Views[View.Goals];
        public ProfileViewModel ProfileViewModel => (ProfileViewModel)Views[View.Profile];
    
        private View _currentView;
        public View CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }
        #endregion

        #region Commands
        public IMvxCommand<View> ChangeView { get; }

        #endregion

        public HomeViewModel()
        {
            _updateService = CrossServiceContainer.UpdateService;

            ChangeView = new MvxCommand<View>(ShowView);

            Views = new Dictionary<View, ToolbarViewModelBase>()
            {
                { View.Home, new HistoryViewModel() },
                { View.Achievements, new TieredAchievementsViewModel() },
                { View.Goals, new GoalsViewModel()},
                { View.Profile, new ProfileViewModel() },
            };

            Toolbar.Instance.SetState(Views[CurrentView].ToolbarState);

            _updateService.UpdateModeChanged += mode =>
            {
                if (mode == UpdateMode.Foreground)
                {
                    StartUpdateLoop();
                }
            };

            StartUpdateLoop();

            ShowView(View.Home);

            if (!Settings.HasRequestedHeight && Settings.UserHeight == 0)
            {
                RequestUserHeightAsync().ConfigureAwait(false);
                Settings.HasRequestedHeight = true;
            }
        }

        private async Task RequestUserHeightAsync()
        {
            await Task.Delay(1000);
            // wait a second;
            var result = await UserDialogs.Instance.ConfirmAsync(
                AppText.profile_setting_height_request,
                AppText.profile_setting_height_request_title,
                "Profil", "Ikke nå");
            if (result)
            {
                ShowView(View.Profile);
            }
        }

        private void StartUpdateLoop()
        {
            if (!_isUpdateLoopRunning)
            {
                RequestDataUpdates(); // run first data request
                if (Device.RuntimePlatform == Device.Android)
                {
                    Device.StartTimer(new TimeSpan(0, 0, 0, 0, Config.AndroidForegroundUpdateIntervalMS), RequestDataUpdates);
                } else if(Device.RuntimePlatform == Device.iOS)
                {
                    Device.StartTimer(new TimeSpan(0, 0, 0, 0, Config.iOSInitialForegroundUpdateIntervalMS), RequestInitialDataUpdates);
                }
                _isUpdateLoopRunning = true;
            }
        }

        /// <summary>
        /// Runs an update for all services on a timer - runs a quick load of today, then the backlog<br/><b>iOS only!</b>
        /// </summary>
        /// <returns>Whether or not to terminate the preload update loop</returns>
        private bool RequestInitialDataUpdates()
        {
            Task.Run(_updateService.UpdateAllDataServices);
            _isUpdateLoopRunning = _updateService.GetUpdateMode() == UpdateMode.Foreground;
            bool runRegularLoop = CrossServiceContainer.WalkingDataService.HasCaughtUpToNow;
            if(runRegularLoop)
            {
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, Config.iOSForegroundUpdateIntervalMS), RequestDataUpdates);
            }
            return _isUpdateLoopRunning && !runRegularLoop;
        }

        /// <summary>
        /// Runs an update for all services on a timer
        /// </summary>
        /// <returns>Whether or not to terminate the update loop</returns>
        private bool RequestDataUpdates()
        {
            Task.Run(_updateService.UpdateAllDataServices);
            _isUpdateLoopRunning = _updateService.GetUpdateMode() == UpdateMode.Foreground;
            return _isUpdateLoopRunning;
        }

        /// <summary>
        /// Pops all views in toolbar stack and sets the toolbar to the new view's state and then opens view.
        /// </summary>
        private void ShowView(View view)
        {
            if (_currentView != view)
            {
                Toolbar.Instance.PopAll();
                Toolbar.Instance.SetState(Views[view].ToolbarState);

                CurrentView = view;

                Views[view].ViewAppearing();

                Analytics.TrackEvent(TrackingEvents.NavigatedTo, new TrackingEvents.NavigatedToArgs(Enum.GetName(typeof(View), _currentView)));
            }
        }
    }
}
