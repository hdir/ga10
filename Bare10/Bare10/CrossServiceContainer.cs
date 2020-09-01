using Bare10.Services.Interfaces;

namespace Bare10
{
    public static class CrossServiceContainer
    {
        public static IWalkingDataService WalkingDataService { get; private set; }
        public static IUpdateService UpdateService { get; private set; }
        public static IScreenshotService ScreenshotService { get; private set; }
        public static IShareImageService ShareImageService { get; private set; }

        public static void SetWalkingDataService(IWalkingDataService walkingDataService)
        {
            WalkingDataService = walkingDataService;
        }

        public static void SetUpdateService(IUpdateService updateService)
        {
            UpdateService = updateService;
        }

        public static void SetScreenshotService(IScreenshotService screenshotService)
        {
            ScreenshotService = screenshotService;
        }

        public static void SetShareImageService(IShareImageService shareImageService)
        {
            ShareImageService = shareImageService;
        }
    }
}
