using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views.Accessibility;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.Utils;
using CarouselView.FormsPlugin.Android;
using DLToolkit.Forms.Controls;
using FFImageLoading.Forms.Platform;
using MvvmCross;
using MvvmCross.Forms.Platforms.Android.Core;
using MvvmCross.Forms.Platforms.Android.Views;
using MvvmCross.Forms.Presenters;
using Plugin.LocalNotifications;
using System;
using System.Threading.Tasks;
using Bare10.Resources;
using Microsoft.AppCenter.Analytics;
using Plugin.StoreReview;
using static Android.Views.Accessibility.AccessibilityManager;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Fitness;

namespace Bare10.Droid
{
    [Activity(
        Theme = "@style/MainTheme",
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait
    )]
    public class MainActivity : MvxFormsAppCompatActivity<Setup, CoreApp, App>, ITouchExplorationStateChangeListener
    {

        private const int REQUEST_GOOGLE_FITNESS_AUTH = 1;
        private const int REQUEST_ACCESS_FINE_LOCATION = 2;
        private const int REQUEST_ACTIVITY_RECOGNITION = 3;
        private const int REQUEST_FITNESS_PERMISSIONS = 4;
        private const int REQUEST_GOOGLE_SIGN_IN = 5;

        GoogleSignInAccount account = null;
        private bool authInProgress = false;
        private bool googleLoginInProgress = false;

        #region Make activity related data available for callbacks and results from and to services
        internal static MainActivity Instance { get; private set; }
        internal bool HasAccessFineLocation { get; private set; } = false;
        internal bool HasActivityRecognition { get; private set; } = false;
        internal bool HasGoogleFitnessAuthentication { get; private set; } = false;
        internal bool HasGoogleFitnessPermissions { get; private set; } = false;

        internal static event Action<MainActivity> OnActivityCreated;
        internal event Action<bool> AccessFineLocationPermissionUpdated;
        internal event Action<bool> ActivityRecognitionPermissionUpdated;
        internal event Action<bool> GoogleFitnessAuthenticationUpdated;
        internal event Action<bool> GoogleFitnessPermissionsUpdated;

        #endregion

        public MainActivity()
        {
            ToolbarResource = Resource.Layout.Toolbar;
            TabLayoutResource = Resource.Layout.Tabbar;
        }

        protected override void OnCreate(Bundle bundle)
        {
            AppDomain.CurrentDomain.UnhandledException += PlatformError.CurrentDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += PlatformError.TaskSchedulerUnobservedTaskException;

            App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);
            App.VersionName = Application.ApplicationContext.PackageManager
                                .GetPackageInfo(Application.ApplicationContext.PackageName,0)
                                .VersionName;

            AccessibilityManager accessibilityManager = (AccessibilityManager)GetSystemService(AccessibilityService);
            //NOTE: ref: https://stackoverflow.com/a/12362545 we only use touch exploration events for checking if talkback is enabled
            accessibilityManager.AddTouchExplorationStateChangeListener(this);
            Accessibility.AccessibilityEnabled = accessibilityManager.IsTouchExplorationEnabled;

            Xamarin.Forms.Forms.Init(this, bundle);
            UserDialogs.Init(this);
            CarouselViewRenderer.Init();
            CachedImageRenderer.Init(true);
            FlowListView.Init();

            Rg.Plugins.Popup.Popup.Init(this, bundle);

            LocalNotificationsImplementation.NotificationIconId = Resource.Mipmap.notification_icon;
                //NOTE: We know. Lazy loading of service for platform specific version
#pragma warning disable CS0436 // Type conflicts with imported type
            IWalkingDataService walkingDataService = CrossWalkingDataService.Current;
            IUpdateService updateService = CrossUpdateService.Current;
            IScreenshotService screenshotService = CrossScreenshotService.Current;
            IShareImageService shareImageService = CrossImageShareService.Current;
#pragma warning restore CS0436 // Type conflicts with imported type
            ScreenshotService.Activity = this;
            ShareImageService.Context = this;

            CrossServiceContainer.SetWalkingDataService(walkingDataService);
            CrossServiceContainer.SetUpdateService(updateService);
            CrossServiceContainer.SetScreenshotService(screenshotService);
            CrossServiceContainer.SetShareImageService(shareImageService);

            Log.Debug("MainActivity", "Mainactivity updateservice: " + updateService.ToString());

            base.OnCreate(bundle);

            Instance = this;
            OnActivityCreated?.Invoke(this);

            if (Settings.CanRequestReview())
            {
                RequestReview();
            }
        }

        private async void RequestReview()
        {
            // wait a bit before requesting
            await Task.Delay(2000);
            var requestReview = await UserDialogs.Instance.ConfirmAsync(
                message: "Fornøyd med appen? Vi vil gjerne ha din vurdering så vi kan bli bedre",
                okText: "Gi vurdering",
                cancelText: "Hopp over");

            if (requestReview)
            {
                CrossStoreReview.Current.OpenStoreReviewPage(Application.PackageName);
            }

            Analytics.TrackEvent(TrackingEvents.RequestedReview,
                new TrackingEvents.RequestedReviewArgs(requestReview));
            Settings.HasRequestedReview = true;
        }

        public void RequestGoogleFitnessAuthentication(ConnectionResult result)
        {
            if (!authInProgress)
            {
                authInProgress = true;
                result.StartResolutionForResult(this, REQUEST_GOOGLE_FITNESS_AUTH);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == REQUEST_GOOGLE_FITNESS_AUTH)
            {
                authInProgress = false;
                HasGoogleFitnessAuthentication = resultCode == Result.Ok;
                System.Diagnostics.Debug.WriteLine($"Result after auth is: {HasGoogleFitnessAuthentication}");
                GoogleFitnessAuthenticationUpdated?.Invoke(HasGoogleFitnessAuthentication);
            }

            if(requestCode == REQUEST_GOOGLE_SIGN_IN)
            {
                googleLoginInProgress = false;
                var signInTask = GoogleSignIn.GetSignedInAccountFromIntent(data);
                if(signInTask.IsSuccessful)
                {
                    account = (GoogleSignInAccount)signInTask.Result;
                    RequestFitnessPermissions();
                }
            }

            if(requestCode == REQUEST_FITNESS_PERMISSIONS)
            {
                HasGoogleFitnessPermissions = resultCode == Result.Ok;
                GoogleFitnessPermissionsUpdated?.Invoke(HasGoogleFitnessPermissions);
            }

        }

        public void RequestAccessFineLocation()
        {
            const string accessFineLocation = Manifest.Permission.AccessFineLocation;
            HasAccessFineLocation = ContextCompat.CheckSelfPermission(this, accessFineLocation) == Permission.Granted;
            if (!HasAccessFineLocation)
            {
                ActivityCompat.RequestPermissions(this, new string[] { accessFineLocation }, REQUEST_ACCESS_FINE_LOCATION);
            }
            else
            {
                AccessFineLocationPermissionUpdated?.Invoke(HasAccessFineLocation);
            }
        }

        public void RequestActivityRecognition()
        {
            const string activityRecognition = Manifest.Permission.ActivityRecognition;
            HasActivityRecognition = ContextCompat.CheckSelfPermission(this, activityRecognition) == Permission.Granted;

            if(!HasActivityRecognition)
            {
                ActivityCompat.RequestPermissions(this, new string[] { activityRecognition }, REQUEST_ACTIVITY_RECOGNITION);
            }
            else
            {
                ActivityRecognitionPermissionUpdated?.Invoke(HasActivityRecognition);
            }
        }

        public void RequestFitnessPermissions()
        {
            //we have to do all this hullaballoo because Xamarin.PlayServices.Fitness does not contain a constructor for FitnessOptions
            //IntPtr classRef = JNIEnv.FindClass("com/google/android/gms/fitness/FitnessOptions$Builder");
            //IntPtr constructorId = JNIEnv.GetMethodID(classRef, "<init>", "I(V)");
            //IntPtr referenceInstance = JNIEnv.NewObject(classRef, constructorId);

            //var fitnessApiOptions = new FitnessOptions.Builder()
            var fitnessApiOptions = FitnessOptions.InvokeBuilder()
                .AddDataType(Android.Gms.Fitness.Data.DataType.AggregateActivitySummary, FitnessOptions.AccessWrite)
                .AddDataType(Android.Gms.Fitness.Data.DataType.AggregateSpeedSummary, FitnessOptions.AccessWrite)
                .Build();

            account = GoogleSignIn.GetLastSignedInAccount(this);

            if(account == null && !googleLoginInProgress)
            {
                GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestEmail()
                    .Build();

                var signInClient = GoogleSignIn.GetClient(this, gso);
                googleLoginInProgress = true;
                StartActivityForResult(signInClient.SignInIntent, REQUEST_GOOGLE_SIGN_IN);
                return;
            }

            if(!GoogleSignIn.HasPermissions(account, fitnessApiOptions))
            {
                GoogleSignIn.RequestPermissions(
                    this,
                    REQUEST_FITNESS_PERMISSIONS,
                    account,
                    fitnessApiOptions);

            }
            else
            {
                HasGoogleFitnessPermissions = true;
                GoogleFitnessPermissionsUpdated?.Invoke(HasGoogleFitnessPermissions);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == REQUEST_ACCESS_FINE_LOCATION)
            {
                HasAccessFineLocation = (grantResults.Length == 1) && (grantResults[0] == Permission.Granted);
                AccessFineLocationPermissionUpdated?.Invoke(HasAccessFineLocation);
            }
            else if(requestCode == REQUEST_ACTIVITY_RECOGNITION)
            {
                HasActivityRecognition = (grantResults.Length == 1) && (grantResults[0] == Permission.Granted);
                ActivityRecognitionPermissionUpdated?.Invoke(HasActivityRecognition);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            //Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
        }

        public void OnTouchExplorationStateChanged(bool enabled)
        {
            Accessibility.AccessibilityEnabled = enabled;
        }
    }

    public class Setup : MvxFormsAndroidSetup<CoreApp, App>
    {
        protected override IMvxFormsPagePresenter CreateFormsPagePresenter(IMvxFormsViewPresenter viewPresenter)
        {
            var formsPresenter = base.CreateFormsPagePresenter(viewPresenter);
            Mvx.IoCProvider.RegisterSingleton(formsPresenter);
            return formsPresenter;
        }
    }
}
