using Acr.UserDialogs;
using Bare10.Localization;
using Bare10.Resources;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.Utils;
using Bare10.ViewModels;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bare10
{
    public class CoreApp : MvxApplication
    {
        public override void Initialize()
        {
            Mvx.IoCProvider.RegisterSingleton<IMissingPermissionService>(new MissingPermissionService());

            RegisterCustomAppStart<ConditionalAppStart>();
        }

        public override Task Startup()
        {
            Mvx.IoCProvider.Resolve<IMissingPermissionService>().MissingPermissionsAdded += RequestResolveMissingPermission;
            return base.Startup();
        }

        private static bool _isRequesting;
        private static void RequestResolveMissingPermission(MissingPermission permission, Action callback)
        {
            if (_isRequesting)
                return;

            _isRequesting = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await UserDialogs.Instance.ConfirmAsync(
                    permission.ToDescription(),
                    AppText.permission_title,
                    "Tillat",
                    "Ikke Tillat");

                _isRequesting = false;

                if (result)
                    callback?.Invoke();
                else
                    RequestResolveMissingPermission(permission, callback);
            });
        }
    }

    public class ConditionalAppStart : MvxAppStart
    {
        private bool isAccessibilityCallbackRegistered = false;
        public ConditionalAppStart(IMvxApplication application, IMvxNavigationService navigationService) : base(application, navigationService)
        {
        }

        protected override Task NavigateToFirstViewModel(object hint = null)
        {
            Task result = null;
            if (!Settings.IntroCompleted)
            {
                result = Accessibility.AccessibilityEnabled ?
                    NavigationService.Navigate<ViewModels.Accessibility.OnboardingViewModel>()
                    : NavigationService.Navigate<OnboardingViewModel>();
            }
            else
            {
                result = Accessibility.AccessibilityEnabled ?
                    NavigationService.Navigate<ViewModels.Accessibility.HomeViewModel>()
                    : NavigationService.Navigate<HomeViewModel>();
            }


            if (!isAccessibilityCallbackRegistered)
            {
                Accessibility.AccessibilityEnabledChanged += async enabled => { await NavigateToFirstViewModel(); };
                isAccessibilityCallbackRegistered = true;
            }
            return result;
        }
    }
}
