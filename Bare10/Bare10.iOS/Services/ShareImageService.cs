using Bare10.Services.Interfaces;
using Foundation;
using System;
using System.IO;
using UIKit;

namespace Bare10.Services
{
    public class ShareImageService : IShareImageService
    {
        //NOTE: Must be called on UI Thread
        public void Show(string title, string message, byte[] imageData)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "screenshot.png");
            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                stream.Write(imageData, 0, imageData.Length);
            }
            var items = new NSObject[] { NSObject.FromObject(title), NSUrl.FromFilename(path) };
            var activityController = new UIActivityViewController(items, null);
            var vc = GetVisibleViewController();

            NSString[] excludedActivityTypes = null;

            if(excludedActivityTypes != null && excludedActivityTypes.Length > 0)
            {
                activityController.ExcludedActivityTypes = excludedActivityTypes;
            }

            vc.PresentViewController(activityController, true, () => { });
        }

        UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if(rootController.PresentedViewController == null)
            {
                return rootController;
            }

            if(rootController.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)rootController.PresentedViewController).TopViewController;
            }

            if(rootController.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;
            }

            return rootController.PresentedViewController;
        }
    }
}