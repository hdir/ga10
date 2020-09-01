using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Onboarding.Base
{
    public abstract class OnboardingItemViewModel : MvxViewModel
    {
        private readonly OnboardingDelegate _controller;

        protected OnboardingItemViewModel(OnboardingDelegate controller)
        {
            _controller = controller;
            NextClickedCommand = new Command(controller.OnNextClicked);
        }

        public ICommand NextClickedCommand { get; }

        protected bool _isValid;
        public virtual bool IsValid
        {
            get => _isValid;
            set
            {
                SetProperty(ref _isValid, value);
                _controller.ValidChanged(value);
            }
        }

        private bool _isCurrentlyVisible;
        public bool IsCurrentlyVisible
        {
            get => _isCurrentlyVisible;
            set => SetProperty(ref _isCurrentlyVisible, value);
        }

        private uint _animationTime = 800u;
        public uint AnimationTime
        {
            get => _animationTime;
            set => SetProperty(ref _animationTime, value);
        }

        public async Task Open()
        {
            IsCurrentlyVisible = true;
            await Task.Delay((int)AnimationTime);
        }

        public async Task Close()
        {
            IsCurrentlyVisible = false;
            await Task.Delay((int)AnimationTime);
        }
    }

    public interface OnboardingDelegate
    {
        void ValidChanged(bool isValid);
        void OnNextClicked();
    }
}