using Bare10.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Bare10.Pages.Popups;
using Bare10.Resources;
using Microsoft.AppCenter.Crashes;
using MvvmCross.ViewModels;
using Rg.Plugins.Popup.Services;
using Device = Xamarin.Forms.Device;
using Acr.UserDialogs;
using Bare10.Utils;

namespace Bare10.Services
{
    public class BulletinService : MvxViewModel
    {
        #region Singleton
        private static readonly Lazy<BulletinService> lazy =
            new Lazy<BulletinService>(() => new BulletinService());

        public static BulletinService Instance => lazy.Value;

        #endregion

        #region Properties

        public ObservableCollection<Bulletin> Bulletins { get; } = new MvxObservableCollection<Bulletin>(Settings.PendingBulletins);

        #endregion

        private BulletinPopupPage _page;

        private BulletinService()
        {
            Bulletins.CollectionChanged += (sender, args) => { Settings.PendingBulletins = Bulletins.ToList(); };
        }

        public void AddBulletin(string title, string description, string iconSource = default, string animation = default, int tier = 0)
        {
            var bulletin = new Bulletin(title, description)
            {
                IconSource = iconSource,
                Tier = (Content.Tier) tier,
                Animation = animation,
            };
            Bulletins.Add(bulletin);

            ShowBulletins();
        }

        public void ClearBulletins()
        {
            Bulletins.Clear();

            if(!Accessibility.AccessibilityEnabled && PopupNavigation.Instance.PopupStack.Count > 0)
            {
                PopupNavigation.Instance.PopAllAsync();
            }
        }

        public void ShowBulletins()
        {
            if (App.IsForeground && Bulletins.Count > 0)
            {
                try
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await RaisePropertyChanged(() => Bulletins);
                        if (!Accessibility.AccessibilityEnabled)
                        {
                            // make sure none are currently open
                            if (_page == null)
                            {
                                _page = new BulletinPopupPage();
                                await PopupNavigation.Instance.PushAsync(_page);
                            }
                        }
                        else
                        {
                            foreach (var b in Bulletins)
                            {
                                await UserDialogs.Instance.AlertAsync(b.Description, b.Title);
                            }
                        }

                    });
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
            }
        }

        public void BulletinClosed(Bulletin bulletin)
        {
            _page?.InvokeSwipe();
        }
    }
}
