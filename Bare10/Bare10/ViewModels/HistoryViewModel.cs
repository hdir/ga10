using Bare10.Localization;
using Bare10.Models.Walking;
using Bare10.Resources;
using Bare10.ViewModels.Base;
using Bare10.ViewModels.Items;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Bare10.Pages.Custom;
using Bare10.Pages.Popups;
using Bare10.Services;
using Bare10.Utils;

namespace Bare10.ViewModels
{
    public class HistoryViewModel : ToolbarViewModelBase
    {
        private uint _loadingMessage;
        private uint _uknownWalkingDataMessage;

        #region Commands

        public IMvxAsyncCommand<string> OpenInfo { get; }

        #endregion

        #region Properties

        private int _todaysBriskWalkingMinutes;

        public int TodaysBriskWalkingMinutes
        {
            get => _todaysBriskWalkingMinutes;
            set
            {
                SetProperty(ref _todaysBriskWalkingMinutes, value);
                Progress.UpdateWithBriskWalkMinutes(TodaysBriskWalkingMinutes);
            }
        }

        private int _todaysRegularWalkingMinutes;

        public int TodaysRegularWalkingMinutes
        {
            get => _todaysRegularWalkingMinutes;
            set => SetProperty(ref _todaysRegularWalkingMinutes, value);
        }

        public ExplicitObservableCollection<WalkingDataPointsViewModel> HourlyData { get; } =
            new ExplicitObservableCollection<WalkingDataPointsViewModel>();

        public ExplicitObservableCollection<WalkingDayModel> WeekData { get; } =
            new ExplicitObservableCollection<WalkingDayModel>();

        public ExplicitObservableCollection<WalkingDayModel> MonthData { get; } =
            new ExplicitObservableCollection<WalkingDayModel>();

        public ProgressViewModel Progress { get; } = new ProgressViewModel();

        public override ToolbarState ToolbarState { get; } = new ToolbarState()
        {
            ShowIcon = true,
            ShowShareButton = true,
        };

        private int goalsMetTotal;
        public int GoalsMetTotal
        {
            get => goalsMetTotal;
            private set
            {
                SetProperty(ref goalsMetTotal, value);
                RaisePropertyChanged(() => GoalsMetPostfix);
            }
        }

        public string GoalsMetPostfix => GoalsMetTotal == 1 ? " gang." : " ganger.";

        private bool shouldShowGoalsMet;
        public bool ShouldShowGoalsMet
        {
            get => shouldShowGoalsMet;
            private set => SetProperty(ref shouldShowGoalsMet, value);
        }


        #endregion // Properties

        public HistoryViewModel()
        {
            _loadingMessage = NotifyCenter.Instance.AddMessage(new NotifyCenter.MessageDetails()
            {
                Text = AppText.history_loading_message,
                Type = NotifyCenter.InfoType.Loading,
            });

            var walkingDataService = CrossServiceContainer.WalkingDataService;
            walkingDataService.TodaysWalkingUpdated += UpdateDay;
            walkingDataService.ThisWeekDaysUpdated += UpdateWeek;
            walkingDataService.LastThirtyDaysUpdated += UpdateMonth;
            walkingDataService.UpdateCompleted += () =>
            {
                if (walkingDataService.HasCaughtUpToNow && _loadingMessage > 0)
                {
                    NotifyCenter.Instance.Clear(_loadingMessage);
                    _loadingMessage = 0;
                }
            };

            CrossServiceContainer.UpdateService.UpdateCompleted += async () =>
            {
                var goalResults = await StorageService.Current.GetAllGoalResults();
                GoalsMetTotal = goalResults.Where(g => g.HasBeenReached).Count();
                ShouldShowGoalsMet = GoalsMetTotal > 0;
            };

            walkingDataService.ConnectToOSService();

            //if (Device.RuntimePlatform == Device.iOS)
            //{
            //    //NOTE: Manually init viewModels on iOS since it might've updated before views were created, and only updates on new data in
            //    UpdateDay(walkingDataService.GetTodaysHistory());
            //    UpdateWeek(walkingDataService.GetWeekHistory());
            //    UpdateMonth(walkingDataService.GetThirtyDaysHistory());
            //}

            OpenInfo = new MvxAsyncCommand<string>(async (type) =>
            {
                switch (type)
                {
                    case "brisk":
                        await PopupNavigation.Instance.PushAsync(new WalkingInfoPopup(
                            Images.HistoryBriskShoe,
                            AppText.description_brisk_title,
                            AppText.description_brisk_text,
                            Colors.TextBriskWalk
                        ));
                        Analytics.TrackEvent(TrackingEvents.ItemTapped,
                            new TrackingEvents.ItemTappedArgs(TrackingEvents.ItemsToTap.BriskWalkExplanation));
                        break;
                    case "normal":
                        await PopupNavigation.Instance.PushAsync(new WalkingInfoPopup(
                            Images.HistoryNormalShoe,
                            AppText.description_normal_title,
                            AppText.description_normal_text,
                            Colors.TextNormalWalk
                        ));
                        Analytics.TrackEvent(TrackingEvents.ItemTapped,
                            new TrackingEvents.ItemTappedArgs(TrackingEvents.ItemsToTap.RegularWalkExplanation));
                        break;
                }
            });
            
            App.Resumed += () =>
            {
                // Reset day data if it's a new day
                if (Settings.PreviousDay != DateTime.Now.Day)
                {
                    ResetDay();
                    Settings.PreviousDay = DateTime.Now.Day;
                }
            };

            Toolbar.Instance.OnShareButtonClicked += () =>
            {
#if SHARE_AS_TEXT
                if(CrossShare.IsSupported)
                {
                    string shareText = string.Format(AppText.sharing_history_content, TodaysBriskWalkingMinutes, TodaysRegularWalkingMinutes);

                    CrossShare.Current.Share(new ShareMessage
                    {
                        Title = AppText.sharing_history_title,
                        Text = shareText,
                        Url = AppText.sharing_url,
                    }, new ShareOptions
                    {
                        ChooserTitle = AppText.sharing_dialog_title,
                    });
                }
#else
                byte[] screenshot = CrossServiceContainer.ScreenshotService.Capture();
                CrossServiceContainer.ShareImageService.Show(AppText.sharing_history_title, string.Empty, screenshot);
#endif
            };
        }

        public void ResetDay()
        {
            TodaysRegularWalkingMinutes = 0;
            TodaysBriskWalkingMinutes = 0;
            HourlyData.Clear();
            HourlyData.RaisePropertyChanged();
        }

        private async void UpdateDay(TodaysWalkingModel today)
        {
            var hourlyDataChanged = false;

            for (var hour = 0u; hour <= DateTime.Now.Hour; hour++)
            {
                var walkingPoints = today.todaysWalking.Where(t => t.Start.Hour == hour && t.Stop.Hour == hour);   //TODO: missing cases where they cross over hours

                if (!walkingPoints.Any())
                    continue;   // skip this hour because it's empty

                // get the viewmodel or create a new
                var vm = HourlyData.FirstOrDefault(m => m.Hour == hour);
                if (vm == null)
                {
                    vm = new WalkingDataPointsViewModel(hour);
                    HourlyData.Add(vm);
                    hourlyDataChanged = true;
                }

                // update if list has new values (redraws graph)
                if (!vm.Equals(walkingPoints))
                    vm.UpdateFrom(walkingPoints);
            }

            if (hourlyDataChanged)
            {
                await RaisePropertyChanged(() => HourlyData);
                HourlyData.RaisePropertyChanged();
            }

            TodaysRegularWalkingMinutes = (int)today.minutesRegularWalkToday + (int)today.minutesUnknownWalkToday;
            TodaysBriskWalkingMinutes = (int)today.minutesBriskWalkToday;

            if (today.minutesUnknownWalkToday >= 10 && !(_uknownWalkingDataMessage > 0))
            {
                _uknownWalkingDataMessage = NotifyCenter.Instance.AddMessage(new NotifyCenter.MessageDetails()
                {
                    Text = "Registrert gange med ukjent hastighet",
                    Type = NotifyCenter.InfoType.Warning,
                    Action = async () =>
                    {
                        await PopupNavigation.Instance.PushAsync(new UnknownWalkingDataPopupPage());
                        NotifyCenter.Instance.Clear(_uknownWalkingDataMessage);
                    }
                });
            }
        }

        private void UpdateWeek(List<WalkingDayModel> walking)
        {
            var week = new WalkingDayModel[7];
            foreach (var walk in walking)
                week[((int)walk.Day.DayOfWeek + 6) % 7] = walk;

            if (!CompareWalkingDataModelLists(week, WeekData))
            {
                WeekData.Replace(week);
                WeekData.RaisePropertyChanged();
            }
        }

        private void UpdateMonth(List<WalkingDayModel> walking)
        {
            if(walking.Count == 0)
            {
                MonthData.Clear();
                MonthData.RaisePropertyChanged();
                return;
            }

            var month = walking.GetRange(0, 30);

            if (!CompareWalkingDataModelLists(month, MonthData))
            {
                MonthData.Replace(month);
                MonthData.RaisePropertyChanged();
            }
        }

        private static bool CompareWalkingDataModelLists(IList<WalkingDayModel> updated, IList<WalkingDayModel> current)
        {
            if (current.Count != updated.Count)
                return false;

            for (var i = 0; i < updated.Count; i++)
            {
                if (updated[i]?.MinutesBriskWalking != current[i]?.MinutesBriskWalking)
                    return false;
            }
            return true;
        }
       
    }


    public class WalkingDataPointsViewModel : MvxNotifyPropertyChanged
    {
        public uint Hour { get; }

        public string HourText => $"{Hour:00}:00";

        public WalkingDataPointsViewModel(uint hour)
        {
            Hour = hour;
        }

        public void UpdateFrom(IEnumerable<WalkingDataPointModel> list)
        {
            WalkingDataPoints = new MvxObservableCollection<WalkingDataPointModel>(list);
            RaisePropertyChanged(() => WalkingDataPoints);
        }

        public override bool Equals(object obj)
        {
            if (obj is WalkingDataPointsViewModel vm)
                return Equals(vm.WalkingDataPoints);
            return false;
        }

        public override int GetHashCode()
        {
            return Hour.GetHashCode();
        }

        public bool Equals(IEnumerable<WalkingDataPointModel> list)
        {
            return Math.Abs(
                           list.Sum(t => t.AverageSpeedMetersPerSecond) 
                           - WalkingDataPoints.Sum(t => t.AverageSpeedMetersPerSecond)
                       ) < 0.1f;
        }

        private MvxObservableCollection<WalkingDataPointModel> _walkingDataPoints =
            new MvxObservableCollection<WalkingDataPointModel>();
        public MvxObservableCollection<WalkingDataPointModel> WalkingDataPoints
        {
            get => _walkingDataPoints;
            set => SetProperty(ref _walkingDataPoints, value);
        }
    }
}
