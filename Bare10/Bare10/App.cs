using System;
using System.Globalization;
using Bare10.Resources;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Bare10
{
    public class App : Application
    {
        public static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("nb-NO");

        public static event Action Resumed;
        public static event Action Asleep;

        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }

        public static string VersionName { get; set; }

        public static bool IsForeground { get; private set; }

        public App()
        {
            IsForeground = true;
            AddResources();
        }

        protected override void OnStart()
        {
            AppCenter.Start("ios=NOT IN THE SOURCE CODE!;" +
                            "android=NOT IN THE SOURCE CODE!;",
                typeof(Analytics), typeof(Crashes));

            Analytics.SetEnabledAsync(!Settings.RegisteredApp || Settings.AllowTracking);
#if DEBUG
            Analytics.SetEnabledAsync(false);
#endif

            Analytics.TrackEvent(TrackingEvents.Started);

            BulletinService.Instance.ShowBulletins();
        }

        protected override void OnResume()
        {
            IsForeground = true;
            CrossServiceContainer.UpdateService?.SetUpdateMode(UpdateMode.Foreground);
            Resumed?.Invoke();
            base.OnResume();

            BulletinService.Instance.ShowBulletins();
        }

        protected override void OnSleep()
        {
            IsForeground = false;
            CrossServiceContainer.UpdateService?.SetUpdateMode(UpdateMode.Background);
            Asleep?.Invoke();
            base.OnSleep();
        }

        private void AddResources()
        {
            #region Label
            Resources.Add(new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter()
                    {
                        Property = Label.TextColorProperty,
                        Value = Colors.Text,
                    },
                    new Setter()
                    {
                        Property = Label.FontSizeProperty,
                        Value = Sizes.TextMedium,
                    },
                    new Setter()
                    {
                        Property = Label.FontFamilyProperty,
                        Value = Fonts.Normal,
                    },
                },
                Triggers =
                {
                    new Trigger(typeof(Label))
                    {
                        Property = Label.FontAttributesProperty,
                        Value = FontAttributes.Bold,
                        Setters = {
                            new Setter()
                            {
                                Property = Label.FontFamilyProperty,
                                Value = Fonts.Bold,
                            },
                        }
                    }
                }
            });
            #endregion

            #region Entry
            Resources.Add(new Style(typeof(Entry))
            {
                Setters =
                {
                    new Setter
                    {
                        Property = Entry.TextColorProperty,
                        Value = Colors.Text
                    },
                },
            });
            #endregion

            #region TextCell
            Resources.Add(new Style(typeof(TextCell))
            {
                Setters = 
                {
                    new Setter
                    {
                        Property = TextCell.TextColorProperty,
                        Value = Colors.Text,
                    },
                },
            });
            #endregion

            #region NavigationPage
            Resources.Add(new Style(typeof(NavigationPage))
            {
                Setters =
                {
                    new Setter()
                    {
                        Property = NavigationPage.BarBackgroundColorProperty,
                        Value = Colors.Background,
                    },
                    new Setter()
                    {
                        Property = NavigationPage.BarTextColorProperty,
                        Value = Colors.Text,
                    },
                }
            });
            #endregion
        }
    }
}
