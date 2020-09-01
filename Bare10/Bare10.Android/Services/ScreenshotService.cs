using Android.App;
using Android.Graphics;
using Bare10.Services.Interfaces;
using System;
using System.IO;

namespace Bare10.Services
{
    public class ScreenshotService : IScreenshotService
    {
        public static Activity Activity { get; set; }

        public byte[] Capture()
        {
            if (Activity == null)
            {
                throw new Exception("You have to set ScreenshotService.Activity");
            }

            var view = Activity.Window.DecorView.RootView;

            using (var screenshot = Bitmap.CreateBitmap(
                view.Width,
                view.Height,
                Bitmap.Config.Argb8888))
            {
                Canvas canvas = new Canvas(screenshot);
                view.Draw(canvas);

                using (var stream = new MemoryStream())
                {
                    screenshot.Compress(Bitmap.CompressFormat.Png, 90, stream);
                    return stream.ToArray();
                }
            }
        }

    }
}