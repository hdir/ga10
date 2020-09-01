using System.Threading.Tasks;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.Base
{
    public abstract class BaseAnimatedView<T> : BaseAnimatedView where T : IMvxViewModel
    {
        public T ViewModel { get; private set; }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is T vm)
                ViewModel = vm;
        }
    }

    public abstract class BaseAnimatedView : ContentView
    {
        public abstract Task AnimateIn();
        public abstract Task AnimateOut();

        protected async void AnimatePropertyChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                await AnimateIn();
            }
            else if (oldValue && !newValue)
            {
                await AnimateOut();
            }
        }

        public static readonly BindableProperty AnimateProperty =
            BindableProperty.Create(
                nameof(Animate),
                typeof(bool),
                typeof(BaseAnimatedView),
                default(bool),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((BaseAnimatedView)bindableObject).AnimatePropertyChanged((bool)oldValue, (bool)newValue);
                }
            );

        public bool Animate
        {
            get => (bool)GetValue(AnimateProperty);
            set => SetValue(AnimateProperty, value);
        }

        public static readonly BindableProperty EasingInProperty =
            BindableProperty.Create(
                nameof(EasingIn),
                typeof(Easing),
                typeof(BaseAnimatedView),
                Easing.CubicInOut
            );

        public Easing EasingIn
        {
            get => (Easing)GetValue(EasingInProperty);
            set => SetValue(EasingInProperty, value);
        }

        public static readonly BindableProperty EasingOutProperty =
            BindableProperty.Create(
                nameof(EasingOut),
                typeof(Easing),
                typeof(BaseAnimatedView),
                Easing.CubicInOut
            );

        public Easing EasingOut
        {
            get => (Easing)GetValue(EasingOutProperty);
            set => SetValue(EasingOutProperty, value);
        }

        public static readonly BindableProperty AnimationTimeProperty =
            BindableProperty.Create(
                nameof(AnimationTime),
                typeof(uint),
                typeof(BaseAnimatedView),
                250u
            );

        public uint AnimationTime
        {
            get => (uint)GetValue(AnimationTimeProperty);
            set => SetValue(AnimationTimeProperty, value);
        }
    }
}
