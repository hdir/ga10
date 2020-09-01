using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Bare10.ViewModels.Base
{
    public abstract class ViewModelBase : MvxViewModel
    {
        protected readonly IMvxNavigationService NavigationService;

        protected ViewModelBase()
        {
            NavigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
        }
    }
}
