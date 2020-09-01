using Android.App;
using Android.Content;

namespace Bare10.Droid.Utils
{
    public static class AppLifecycleUtil
    {
        public static bool IsRunning(Context context, string packageName)
        {
            ActivityManager activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            foreach (var p in activityManager.RunningAppProcesses)
            {
                if(p.ProcessName.Equals(packageName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}