using System;
using Bare10.Localization;
using Bare10.Services.Interfaces;

namespace Bare10.Content
{
    public static class Permissions
    {
        public static string GetDescription(MissingPermission permission)
        {
            switch (permission)
            {
                case MissingPermission.iOSLocationAlways: return AppText.permission_ios_location;
                case MissingPermission.iOSMotionAndActivity: return AppText.permission_ios_motion;
                case MissingPermission.AndroidFineLocation: return AppText.permission_fine_location;
                case MissingPermission.AndroidActivityRecognition: return AppText.permission_activity_recognition;
                case MissingPermission.AndroidFitnessAccountLink: return AppText.permission_google_fitness;
                case MissingPermission.AndroidFitnessPermissions: return AppText.permission_google_fitness;
                default:
#if DEBUG
                    throw new ArgumentException("Trying to get text content for a permission that does not exist: {0}", permission.ToString());
#else
                    return string.Empty;
#endif
            }
        }
    }
}
