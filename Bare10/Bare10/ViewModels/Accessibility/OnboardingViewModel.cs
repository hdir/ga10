using Bare10.Resources;
using Bare10.ViewModels.Base;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using System;

namespace Bare10.ViewModels.Accessibility
{
    public class OnboardingViewModel : ViewModelBase
    {
        #region Commands
        public IMvxAsyncCommand OnComplete { get; }
        #endregion

        #region Properties
        private string userName;
        public string UserName
        {
            get => userName;
            set
            {
                SetProperty(ref userName, value);
                HasName = UserName != string.Empty;
            }
        }

        private bool hasName = false;
        public bool HasName
        {
            get => hasName;
            set
            {
                SetProperty(ref hasName, value);
                RaisePropertyChanged(nameof(ReadyToProceed));
            }
        }

        private bool hasAcceptedTerms = false;
        public bool HasAcceptedTerms
        {
            get =>  hasAcceptedTerms;
            set
            {
                SetProperty(ref hasAcceptedTerms, value);
                RaisePropertyChanged(nameof(ReadyToProceed));
            }
        }

        public bool ReadyToProceed
        {
            get => HasAcceptedTerms && HasName;
        }
        #endregion

        public OnboardingViewModel()
        {
            OnComplete = new MvxAsyncCommand(async () =>
            {
                Settings.UserName = UserName;
                Settings.StartDate = DateTime.Now;

                Analytics.TrackEvent(TrackingEvents.OnBoarding,
                    new TrackingEvents.OnBoardingArgs(TrackingEvents.OnBoardingEvents.Completed));
                await NavigationService.Navigate<HomeViewModel>();
            });
        }
    }

}
