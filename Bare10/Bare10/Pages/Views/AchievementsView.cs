using System.Collections;
using Bare10.Pages.Custom.Base;
using Bare10.Pages.Views.Achievements;
using Bare10.ViewModels;
using Bare10.ViewModels.Base;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views
{
    public class AchievementsView : MvxContentView<TieredAchievementsViewModel>
    {
        private readonly AchievementTierDetailView _selectedView;

        public AchievementsView()
        {
            this.SetBinding(ItemsSourceProperty, nameof(ViewModel.Achievements));

            _selectedView = CreateSelectedView();

            CreateView();
        }

        private void CreateView()
        {
            var achievements = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
            };

            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    var view = new AchievementListItemViewCell().View;
                    view.BindingContext = item;
                    achievements.Children.Add(view);
                }
            }

            Content = new Grid()
            {
                Children = {
                    new ScrollView()
                    {
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        Content = achievements,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Always,
                    },
                    _selectedView,
                }
            };
        }

        private AchievementTierDetailView CreateSelectedView()
        {
            var view = new AchievementTierDetailView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                EasingIn = Easing.SpringOut,
                IsVisible = false,
                YOffset = App.ScreenHeight / 3f,
            };
            view.SetBinding(AchievementTierDetailView.ContentBindingContextProperty, nameof(ViewModel.Selected));
            view.SetBinding(BaseAnimatedView.AnimateProperty, nameof(ViewModel.ShowAchievementDetails));

            var swipeGesture = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Right
            };
            swipeGesture.Swiped += (sender, args) =>
            {
                if (Toolbar.Instance.Stack.Count > 0)
                    Toolbar.Instance.Pop();
            };
            view.GestureRecognizers.Add(swipeGesture);

            return view;
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(ICollection),
                typeof(AchievementsView),
                default(ICollection),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((AchievementsView) bindableObject).CreateView();
                }
            );

        public ICollection ItemsSource
        {
            get => (ICollection) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

    }
}
