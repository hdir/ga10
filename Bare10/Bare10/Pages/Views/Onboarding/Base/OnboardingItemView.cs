using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Onboarding.Base
{

    public abstract class OnboardingItemView<T> : MvxContentView<T> where T : OnboardingItemViewModel
    {
        public new StackLayout Content { get; }

        #region Properties
        public int ScreenHeight { get; set; } = 1000;
        public ushort StaggerTime { get; set; } = 100;
        public Easing EasingIn { get; set; } = Easing.CubicOut;
        public Easing EasingOut { get; set; } = Easing.CubicIn;
        #endregion

        private Dictionary<View, Action<View>> _callbacks = new Dictionary<View, Action<View>>();

        protected OnboardingItemView()
        {
            Content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
            };

            this.SetBinding(AnimateProperty, nameof(ViewModel.IsCurrentlyVisible));
            this.SetBinding(AnimationTimeProperty, nameof(ViewModel.AnimationTime));

            base.Content = Content;
        }

        public void AddAnimatedView(View view, Action<View> onAnimationFinished = null)
        {
            view.Opacity = 0;
            Content.Children.Add(view);

            if (onAnimationFinished != null)
            {
                _callbacks.Add(view, onAnimationFinished);
            }
        }

        private async Task AnimateIn()
        {
            if (Content.Children.Count == 0)
                return;

            // reset
            foreach (var child in Content.Children)
            {
                child.TranslationY = (ScreenHeight * 0.5f) - (Device.RuntimePlatform == Device.Android ? 0 : GetScreenCoordinates(child).Y);
                child.Opacity = 0;
            }

            // animate in
            foreach (var child in Content.Children)
            {
                _ = Task.Run(async () =>
                {
                    await Task.WhenAll(child.TranslateTo(0, 0, AnimationTime, EasingIn),
                        child.FadeTo(1, AnimationTime));
                    if (_callbacks.ContainsKey(child))
                    {
                        _callbacks[child]?.Invoke(child);
                    }
                }).ConfigureAwait(false);

                await Task.Delay(StaggerTime);
            }
        }

        private async Task AnimateOut()
        {
            if (Content.Children.Count == 0)
                return;

            // reset
            foreach (var child in Content.Children)
            {
                child.TranslationY = 0;
                child.Opacity = 1;
            }

            var time = (ushort)Math.Max(AnimationTime - Content.Children.Count * StaggerTime, StaggerTime);

            // animate out
            foreach (var child in Content.Children)
            {
#pragma warning disable 4014
                child.TranslateTo(0, -ScreenHeight, time, EasingOut);
                await Task.Delay(StaggerTime);
                child.FadeTo(0, time);
#pragma warning restore 4014
            }
        }

        #region Bindable Properties

        public static readonly BindableProperty AnimateProperty =
            BindableProperty.Create(
                nameof(Animate),
                typeof(bool),
                typeof(OnboardingItemView<T>),
                default(bool),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((OnboardingItemView<T>) bindableObject).AnimatePropertyChanged((bool) oldValue, (bool) newValue);
                }
            );

        private async void AnimatePropertyChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
                await AnimateIn();
            else if (oldValue && !newValue)
                await AnimateOut();
        }

        public bool Animate
        {
            get => (bool) GetValue(AnimateProperty);
            set => SetValue(AnimateProperty, value);
        }

        public static readonly BindableProperty AnimationTimeProperty =
            BindableProperty.Create(
                nameof(AnimationTime),
                typeof(uint),
                typeof(OnboardingItemView<T>),
                350u
            );

        public uint AnimationTime
        {
            get => (uint) GetValue(AnimationTimeProperty);
            set => SetValue(AnimationTimeProperty, value);
        }

        #endregion
        
        public (double X, double Y) GetScreenCoordinates(VisualElement view)
        {
            // A view's default X- and Y-coordinates are LOCAL with respect to the boundaries of its parent,
            // and NOT with respect to the screen. This method calculates the SCREEN coordinates of a view.
            // The coordinates returned refer to the top left corner of the view.

            // Initialize with the view's "local" coordinates with respect to its parent
            double screenCoordinateX = view.X;
            double screenCoordinateY = view.Y;

            // Get the view's parent (if it has one...)
            var parent = view.Parent;

            // Loop through all parents
            while (parent is VisualElement v)
            {
                // Add in the coordinates of the parent with respect to ITS parent
                screenCoordinateX += v.X;
                screenCoordinateY += v.Y;

                parent = parent.Parent;
            }

            // Return the final coordinates...which are the global SCREEN coordinates of the view
            return (screenCoordinateX, screenCoordinateY);
        }
    }
}