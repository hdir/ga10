using System;
using System.Linq;
using System.Threading.Tasks;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.ViewControllers
{
    public class AppearingView : Grid
    {
        public event Action Appearing;
        public event Action Disappearing;

        public event Action<bool> VisibilityChanged;

        public AppearingView()
        {
            SizeChanged += (sender, args) =>
            {
                if (Appear)
                    MakeAppear(this);
            };
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == null)
                return;

            if (propertyName.Equals(nameof(IsVisible)))
                VisibilityChanged?.Invoke(IsVisible);
        }

        public static readonly BindableProperty AppearProperty =
            BindableProperty.Create(
                nameof(Appear),
                typeof(bool),
                typeof(AppearingView),
                default(bool),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((AppearingView) bindableObject).AppearPropertyChanged((bool) oldValue, (bool) newValue);
                }
            );

        private void AppearPropertyChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue && IsVisible)
            {
                MakeAppear(this);
                Appearing?.Invoke();
            }
            else if (oldValue && !newValue && IsVisible)
            {
                MakeDisappear(this);
                Disappearing?.Invoke();
            }
        }

        public bool Appear
        {
            get => (bool) GetValue(AppearProperty);
            set => SetValue(AppearProperty, value);
        }

        public static readonly BindableProperty AnimationTimeProperty =
            BindableProperty.Create(
                nameof(AnimationTime),
                typeof(uint),
                typeof(AppearingView),
                250u
            );

        public uint AnimationTime
        {
            get => (uint) GetValue(AnimationTimeProperty);
            set => SetValue(AnimationTimeProperty, value);
        }

        public static readonly BindableProperty SpacingProperty =
            BindableProperty.Create(
                nameof(Spacing),
                typeof(double),
                typeof(AppearingView),
                default(double)
            );

        public double Spacing
        {
            get => (double) GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }


        public static async void MakeAppear(AppearingView parent)
        {
            var used = 0.0;

            var contentHeight = parent.Children.Sum(child => child.Height + parent.Spacing) - parent.Spacing;

            // set to initial
            await parent.FadeTo(0, 0);

            foreach (var element in parent.Children)
            {
                if (element is VisualElement view)
                {
                    await view.TranslateTo(0, 800, 0);
                }
            }

            // animate appearing
            var time = parent.AnimationTime;
            const int delay = 150;

#pragma warning disable 4014
            parent.FadeTo(1f, time, Easing.CubicInOut);
#pragma warning restore 4014

            for (var i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is VisualElement view)
                {
                    // await svg loaded
                    if (view is SvgCachedImage svg)
                        while (svg.IsLoading)
                            await Task.Delay(10);

                    await Task.WhenAny(
                        Task.Delay(delay),
                        view.TranslateTo(0, used + view.Height/2f - contentHeight/2f, time, Easing.CubicOut)
                    );
                    if (time > delay)
                        time -= delay;

                    used += view.Height + parent.Spacing;

                    await Task.Delay(delay);
                }
            }
        }

        public static async void MakeDisappear(AppearingView parent)
        {
            // animate disappearing
            var time = (parent.AnimationTime / 3) * 2;
            const int delay = 100;

#pragma warning disable 4014
            parent.FadeTo(0f, time, Easing.CubicInOut);
#pragma warning restore 4014

            foreach (var element in parent.Children)
            {
                if (element is VisualElement view)
                {
                    // await svg loaded
                    if (view is SvgCachedImage svg)
                        while (svg.IsLoading)
                            await Task.Delay(10);

                    await Task.WhenAny(
                        Task.Delay(delay),
                        view.TranslateTo(0, -800, time, Easing.CubicIn)
                    );
                    if (time > delay)
                        time -= delay;
                    await Task.Delay(delay);
                }
            }
        }
    }
}
