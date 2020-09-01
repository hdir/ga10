using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Bare10.Localization;
using Bare10.Resources;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.ViewModels.Base;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;

namespace Bare10.ViewModels
{
    public class ProfileViewModel : ToolbarViewModelBase
    {
        private readonly IStorageService storageService;

        #region Commands
        public IMvxCommand<SettingViewModel.SettingType> SelectedSettingCommand { get; }
        public IMvxAsyncCommand DeleteAllData { get; }
        #endregion //Commands

        #region Properties
        public string Name
        {
            get => Settings.UserName;
            set
            {
                if(value != Settings.UserName)
                {
                    Analytics.TrackEvent(TrackingEvents.NameChanged);
                }
                Settings.UserName = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Height
        {
            get => Settings.UserHeight.ToString();
            set
            {
                if (int.TryParse(value, out var height))
                {
                    if (height != Settings.UserHeight)
                    {
                        Analytics.TrackEvent(TrackingEvents.HeightChanged);
                    }
                    Settings.UserHeight = height;
                }
                RaisePropertyChanged(() => Height);
                RaisePropertyChanged(() => HeightVisible);
            }
        }

        public bool HeightVisible => Settings.UserHeight > 0;


        public bool WillSendNotifications
        {
            get => Settings.WillSendNotifications;
            set
            {
                Settings.WillSendNotifications = value;
                RaisePropertyChanged(() => WillSendNotifications);
            }
        }

        public bool WillRegisterStatistics
        {
            get => Settings.AllowTracking;
            set
            {
                Settings.AllowTracking = value;
                SetAppCenterEnabledAsync(value);
            }
        }

        private async void SetAppCenterEnabledAsync(bool enabled)
        {
            await AppCenter.SetEnabledAsync(enabled);
            await RaisePropertyChanged(() => WillRegisterStatistics);
        }

        public override ToolbarState ToolbarState { get; } = new ToolbarState()
        {
            Title = AppText.navigation_profile_title,
        };

        public List<SettingViewModel> DetailViewModels { get; } = new List<SettingViewModel>()
        {
            new AboutViewModel(),
            new TermsViewModel(),
            new FeedbackViewModel(),
            //new TestingViewModel(),
        };

        private SettingViewModel _selectedSetting;
        public SettingViewModel SelectedSetting
        {
            get => _selectedSetting;
            set => SetProperty(ref _selectedSetting, value);
        }

        private bool _showDetailView;
        public bool ShowDetailView
        {
            get => _showDetailView;
            set => SetProperty(ref _showDetailView, value);
        }
        #endregion // Properties

        public ProfileViewModel()
        {
            storageService = StorageService.Current;

            DeleteAllData = new MvxAsyncCommand(async () =>
            {
                var result = await UserDialogs.Instance.ConfirmAsync(AppText.profile_setting_delete_confirm,
                    AppText.profile_setting_delete_title, "Ja", "Avbryt");

                if (result)
                {
                    await storageService.DeleteDataBaseTables(Table.All);
                    Analytics.TrackEvent(TrackingEvents.WipedData);
                    Settings.StartDate = DateTime.Now;
                    await CrossServiceContainer.WalkingDataService.RequestUpdate();
                }
            });
            SelectedSettingCommand = new MvxCommand<SettingViewModel.SettingType>(OpenSettingDetails);
        }

        private void OpenSettingDetails(SettingViewModel.SettingType selected)
        {
            SelectedSetting = DetailViewModels.FirstOrDefault(vm => vm.Type == selected);
            if (SelectedSetting != null)
            {
                Analytics.TrackEvent(TrackingEvents.ItemTapped, new TrackingEvents.ItemTappedArgs(selected.ToString()));
                Toolbar.Instance.Push(SelectedSetting.ToolbarState, () =>
                {
                    // close setting
                    ShowDetailView = false;
                });
                ShowDetailView = true;
            }
        }
    }
}
