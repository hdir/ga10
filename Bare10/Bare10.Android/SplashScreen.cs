using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Forms.Platforms.Android.Views;

namespace Bare10.Droid
{
    [Activity(
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait
    )]
    public class SplashScreen : MvxFormsSplashScreenAppCompatActivity<Setup, CoreApp, App>
    {
        public SplashScreen() : base(Resource.Layout.SplashScreen)
        {
        }

        protected override Task RunAppStartAsync(Bundle bundle)
        {
            StartActivity(typeof(MainActivity));
            OverridePendingTransition(0, 0);
            return Task.CompletedTask;
        }
    }
}