using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Bare10.Pages.Views.Onboarding;
using Bare10.Pages.Views.Onboarding.Base;
using Bare10.Resources;
using Bare10.ViewModels.Base;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Bare10.ViewModels
{
    public class OnboardingViewModel : ViewModelBase, OnboardingDelegate
    {
        public ICommand NextClickedCommmand => new MvxCommand(OnNextClicked);

        public MvxObservableCollection<OnboardingItemViewModel> Pages { get; }

        public OnboardingViewModel()
        {
            Pages = new MvxObservableCollection<OnboardingItemViewModel>()
            {
                new OnboardingWelcomeViewModel(this),
                new OnboardingHeightViewModel(this),
                new OnboardingTermsViewModel(this),
                new OnboardingOutroViewModel(this),
            };

            AppCenter.SetEnabledAsync(false);
        }

        public void ValidChanged(bool isValid)
        {
            RaisePropertyChanged(() => ButtonIsValid);
        }

        public async void OnNextClicked()
        {
            if (!IsBusy && ButtonIsValid)
            {
                await ShowPage(CarouselPosition + 1);
            }

            // NOTE: Special case for last page, animated out automatically
            if (Pages.Count > 1 && CarouselPosition == Pages.Count-1)
            {
                // hide button
                IsBusy = true;
                // wait for user to read
                await Task.Delay(2000);
                // automatically move to next
                await ShowPage(Pages.Count);
            }
        }

        public async void ViewLoaded()
        {
            await ShowPage(0);

            if (!Settings.RegisteredApp)
            {
                Analytics.TrackEvent(TrackingEvents.OnBoarding,
                    new TrackingEvents.OnBoardingArgs(TrackingEvents.OnBoardingEvents.Started));
                Settings.RegisteredApp = true;
            }
        }

        private async void OnComplete()
        {
            Settings.UserName = Pages.OfType<OnboardingWelcomeViewModel>().FirstOrDefault()?.UserName;
            Settings.StartDate = DateTime.Now;

            Analytics.TrackEvent(TrackingEvents.OnBoarding,
                new TrackingEvents.OnBoardingArgs(TrackingEvents.OnBoardingEvents.Completed));

            if (Settings.UserHeight > 0)
                Analytics.TrackEvent(TrackingEvents.OnBoarding, new Dictionary<string, string>(){{"height", Settings.UserHeight.ToString()}});

            await NavigationService.Navigate<HomeViewModel>();
        }

        private async Task ShowPage(int index)
        {
            IsBusy = true;

            if (index > 0)
                await Pages[CarouselPosition].Close();
            else
                await Task.Delay(500);
            CarouselPosition = index;
            await Pages[CarouselPosition].Open();

            IsBusy = false;

            Analytics.TrackEvent(TrackingEvents.OnBoarding, new TrackingEvents.OnBoardingArgs(index));
        }

        private int _carouselPosition;
        public int CarouselPosition
        {
            get => _carouselPosition;
            set
            {
                if (value >= Pages.Count)
                {
                    OnComplete();
                }
                else
                {
                    SetProperty(ref _carouselPosition, value);
                    RaisePropertyChanged(nameof(ButtonIsValid));
                }
            }
        }

        private bool _isBusy = true;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool ButtonIsValid => Pages[CarouselPosition].IsValid;
    }
}
