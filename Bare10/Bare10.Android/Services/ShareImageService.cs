using Android.Content;
using Android.Support.V4.Content;
using Bare10.Services.Interfaces;
using System;
//using System;
using System.IO;
using Uri = Android.Net.Uri;

namespace Bare10.Services
{
    public class ShareImageService : IShareImageService
    {
        public static Context Context { get; set; }

        public void Show(string title, string message, byte[] imageData)
        {
            if(Context == null)
            {
                throw new Exception("You have to set ShareImageService.Context");
            }

            string filesDir = Context.FilesDir.AbsolutePath;

            //NOTE: defined in Resources/xml/filepaths to be available for fileprovider
            string path = Path.Combine(filesDir, "tempscreenshots/screenshot.png");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                stream.Write(imageData, 0, imageData.Length);
            }

            var contentType = "image/png";
            Uri fileUri = FileProvider.GetUriForFile(Context, Context.PackageName + ".fileprovider", new Java.IO.File(path));

            var intent = new Intent(Intent.ActionSend);
            intent.SetType(contentType);
            intent.PutExtra(Intent.ExtraStream, fileUri);
            intent.PutExtra(Intent.ExtraText, string.Empty);
            intent.PutExtra(Intent.ExtraSubject, message ?? string.Empty);

            var chooserIntent = Intent.CreateChooser(intent, title ?? string.Empty);
            chooserIntent.SetFlags(ActivityFlags.ClearTop);
            chooserIntent.SetFlags(ActivityFlags.NewTask);
            chooserIntent.SetFlags(ActivityFlags.GrantReadUriPermission);
            Context.StartActivity(chooserIntent);
        }
    }
}