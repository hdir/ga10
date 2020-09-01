using Bare10.Resources;
using Bare10.Services;
using Bare10.Services.Interfaces;
using CarouselView.FormsPlugin.iOS;
using DLToolkit.Forms.Controls;
using FFImageLoading.Forms.Platform;
using Foundation;
using Lottie.Forms.iOS.Renderers;
using MvvmCross;
using MvvmCross.Forms.Platforms.Ios.Core;
using MvvmCross.Forms.Presenters;
using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using StoreKit;
using UIKit;
using UserNotifications;
using XFShapeView.iOS;
using Bare10.Utils;

namespace Bare10.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : MvxFormsApplicationDelegate<Setup, CoreApp, App>
    {
        public static event Action WillTerminateEvent;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            OnFinishedLaunching(options);
            app.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            return base.FinishedLaunching(app, options);
        }

        public override void FinishedLaunching(UIApplication application)
        {
            OnFinishedLaunching();
            base.FinishedLaunching(application);
        }

        public void OnFinishedLaunching(NSDictionary options = null)
        {

            AppDomain.CurrentDomain.UnhandledException += PlatformError.CurrentDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += PlatformError.TaskSchedulerUnobservedTaskException;

            //NOTE: this is in seconds
            //UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(Config.BackgroundUpdateIntervalMS / 1000);
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
            App.VersionName = NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")].ToString();

            Accessibility.AccessibilityEnabled = UIAccessibility.IsVoiceOverRunning;
            NSNotificationCenter.DefaultCenter.AddObserver(UIView.VoiceOverStatusDidChangeNotification, notification => Accessibility.AccessibilityEnabled = UIAccessibility.IsVoiceOverRunning);

            Xamarin.Forms.Forms.Init();

            AnimationViewRenderer.Init();
            CarouselViewRenderer.Init();
            CachedImageRenderer.Init();
            Rg.Plugins.Popup.Popup.Init();
            
            CarouselViewRenderer.Init();
            CachedImageRenderer.Init();
            ShapeRenderer.Init();
            
            FlowListView.Init();

            //Setup accessibility callbacks

            Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            GetNotificationPermissions();

                //NOTE: We know. Lazy loading of service for platform specific version
#pragma warning disable CS0436 // Type conflicts with imported type
            IWalkingDataService walkingDataService = CrossWalkingDataService.Current;
            IUpdateService updateService = CrossUpdateService.Current;
            IScreenshotService screenshotService = CrossScreenshotService.Current;
            IShareImageService shareImageService = CrossImageShareService.Current;
#pragma warning restore CS0436 // Type conflicts with imported type

            CrossServiceContainer.SetWalkingDataService(walkingDataService);
            CrossServiceContainer.SetUpdateService(updateService);
            CrossServiceContainer.SetScreenshotService(screenshotService);
            CrossServiceContainer.SetShareImageService(shareImageService);

            Console.WriteLine("Appdelegate unning updateService: " + updateService.ToString());
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // reset our badge when app is resumed
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            base.WillEnterForeground(application);

            if (Settings.CanRequestReview())
            {
                RequestReviewArgs();
            }
        }

        private async void RequestReviewArgs()
        {
            // wait a bit before requesting
            await Task.Delay(2000);

            var requestReview = false;

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 3))
            {
                // In-App request
                SKStoreReviewController.RequestReview();
            }
            else
            {
                //NOTE: untested behaviour is emitted

                //requestReview = await UserDialogs.Instance.ConfirmAsync(
                //    message: "Fornøyd med appen? Vi vil gjerne ha din vurdering så vi kan bli bedre",
                //    okText: "Gi vurdering",
                //    cancelText: "Hopp over");

                //if (requestReview)
                //{
                //    if (UIDevice.CurrentDevice.CheckSystemVersion(10, 3))
                //    {
                //        SKStoreReviewController.RequestReview();
                //    }
                //    CrossStoreReview.Current.OpenStoreReviewPage("1447263129");
                //}
            }

            Analytics.TrackEvent(TrackingEvents.RequestedReview,
                new TrackingEvents.RequestedReviewArgs(requestReview));
            Settings.HasRequestedReview = true;
        }

        public override void WillTerminate(UIApplication uiApplication)
        {
            WillTerminateEvent?.Invoke();
            Crashes.TrackError(new BackgroundException("App was terminated!"));
            base.WillTerminate(uiApplication);
        }

        public override async void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            Analytics.TrackEvent(TrackingEvents.BackgroundEvents.BackgroundFetch);

            //NOTE: We know. Lazy loading of service for platform specific version
#pragma warning disable CS0436 // Type conflicts with imported type
            IWalkingDataService walkingDataService = CrossWalkingDataService.Current;
            IUpdateService updateService = CrossUpdateService.Current;
#pragma warning restore CS0436 // Type conflicts with imported type

            CrossServiceContainer.SetWalkingDataService(walkingDataService);
            CrossServiceContainer.SetUpdateService(updateService);

            Console.WriteLine("Background Fetch running updateService: " + updateService.ToString());

            var result = UIBackgroundFetchResult.NoData;
            var minutesBefore = walkingDataService.GetTodaysHistory().minutesBriskWalkToday +
                               walkingDataService.GetTodaysHistory().minutesRegularWalkToday;
            try {
                await updateService.UpdateAllDataServices();
                var minutesAfter = walkingDataService.GetTodaysHistory().minutesBriskWalkToday +
                                   walkingDataService.GetTodaysHistory().minutesRegularWalkToday; ;
                if (minutesBefore != minutesAfter)
                {
                    result = UIBackgroundFetchResult.NewData;
                }
            }
            catch
            {
                Crashes.TrackError(new BackgroundException(UIBackgroundFetchResult.Failed.ToString()));
                result = UIBackgroundFetchResult.Failed;
            }

            var output = "Ran background fetch with result: " + Enum.GetName(typeof(UIBackgroundFetchResult), result);
#if DEBUG
            LiveLogService.Current.LogLine(output);
#endif
            completionHandler(result);
            Analytics.TrackEvent(TrackingEvents.Background, new TrackingEvents.BackgroundArgs(result.ToString()));
        }

        public static void GetNotificationPermissions()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                        UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                        (approved, error) => {
                        });

                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                        new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
        }
    }

    public class Setup : MvxFormsIosSetup<CoreApp, App>
    {
        protected override IMvxFormsPagePresenter CreateFormsPagePresenter(IMvxFormsViewPresenter viewPresenter)
        {
            var formsPresenter = base.CreateFormsPagePresenter(viewPresenter);
            Mvx.IoCProvider.RegisterSingleton(formsPresenter);

            return formsPresenter;
        }
    }

    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }

    [Serializable]
    public class BackgroundException : Exception
    {
        public BackgroundException(string message) : base(message)
        {
        }

        protected BackgroundException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
