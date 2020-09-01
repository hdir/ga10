using Bare10.Resources;
using Bare10.ViewModels.Base;
using Microsoft.AppCenter.Analytics;

namespace Bare10.ViewModels.Accessibility
{
    public class ProfileViewModel : ViewModelBase
    {
        #region Properties
        public string Name
        {
            get => Settings.UserName;
            set
            {
                if(value != Settings.UserName)
                {
                    Analytics.TrackEvent(TrackingEvents.NameChanged);
                }
                Settings.UserName = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public bool WillSendNotifications
        {
            get => Settings.WillSendNotifications;
            set
            {
                Settings.WillSendNotifications = value;
                RaisePropertyChanged(() => WillSendNotifications);
            }
        }

        #endregion
    }
}
