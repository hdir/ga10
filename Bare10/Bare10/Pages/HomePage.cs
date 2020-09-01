using Bare10.Pages.Views;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.Utils.Views;
using Bare10.ViewModels;
using CarouselView.FormsPlugin.Abstractions;
using FFImageLoading.Svg.Forms;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using System.Collections.Generic;
using Bare10.Localization;
using Bare10.Pages.Custom;
using Bare10.ViewModels.Base;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Page = Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page;

namespace Bare10.Pages
{
    [MvxContentPagePresentation(Animated = true, WrapInNavigationPage = true, NoHistory = true)]
    public class HomePage : MvxContentPage<HomeViewModel>
    {
        public HomePage()
        {
            // Use safe space (pad below statusbar on iOS)
            Page.SetUseSafeArea(On<Xamarin.Forms.PlatformConfiguration.iOS>(), true);

            // Navigation Bar
            NavigationPage.SetTitleView(this, new ToolbarView());

            // Main Background
            BackgroundColor = Colors.Background;

            // info slide-down
            var notifyView = new NotifyView()
            {
                BindingContext = NotifyCenter.Instance,
            };
            notifyView.SetBinding(NotifyView.MessageBindingContextProperty, nameof(NotifyCenter.Messages));

            // Content Layout
            var layout = new Grid
            {
                RowSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto },
                },
                Children =
                {
                    { ContentView(), 0, 0 },
                    { TabPanel(), 0, 1 },
                    { notifyView, 0, 0},  // overlay notify center
                },
            };

            Content = layout;
            Utils.Accessibility.Unused(Content);
        }

        protected override bool OnBackButtonPressed()
        {
            if (Toolbar.Instance.Stack.Count > 0)
            {
                Toolbar.Instance.Pop();
                return true;
            }

            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                PopupNavigation.Instance.PopAsync();
                return true;
            }

            return base.OnBackButtonPressed();
        }

        private View ContentView()
        {
            var carousel = new CarouselViewControl
            {
                AnimateTransition = false,
                ShowArrows = false,
                IsSwipeEnabled = false,
                ItemsSource = new List<DataTemplate>
                {
                    new DataTemplate(HistoryView),
                    new DataTemplate(AchievementsView),
                    new DataTemplate(GoalsView),
                    new DataTemplate(ProfileView),
                }
            };
            carousel.SetBinding(CarouselViewControl.PositionProperty, new Binding(
                nameof(HomeViewModel.CurrentView), 
                BindingMode.TwoWay,
                new ViewToIndexConverter())
            );
            Utils.Accessibility.Unused(carousel);

            return carousel;
        }

        private View HistoryView()
        {
            var historyView = new HistoryView();
            historyView.SetBinding(BindingContextProperty, nameof(HomeViewModel.HistoryViewModel));
            Utils.Accessibility.Unused(historyView);
            return historyView;
        }

        private View AchievementsView()
        {
            var achievementsView = new AchievementsView();
            //var achievementsView = new TieredAchievementsView();
            achievementsView.SetBinding(BindingContextProperty, nameof(HomeViewModel.AchievementsViewModel));
            return achievementsView;
        }

        private View GoalsView()
        {
            var goalsView = new GoalsView();
            goalsView.SetBinding(BindingContextProperty, nameof(HomeViewModel.GoalsViewModel));
            Utils.Accessibility.Unused(goalsView);
            return goalsView;
        }

        private View ProfileView()
        {
            var profileView = new ProfileView();
            profileView.SetBinding(BindingContextProperty, nameof(HomeViewModel.ProfileViewModel));
            Utils.Accessibility.Unused(profileView);
            return profileView;
        }

        private View TabPanel()
        {
            var line = DefaultView.SectionLine;
            line.VerticalOptions = LayoutOptions.Start;

            var layout = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.TabPanelBackground,
                Margin = new Thickness(0, 0, 0, -40),   // Margin out of view for iOS X Safe Area Background
                Padding = new Thickness(0, 0, 0, 40),   // Padding content inside
                RowSpacing = 0,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                },
                Children =
                {
                    {
                        TabItem(
                            AppText.history_tab_item_title,
                            Images.TabHistoryActive,
                            Images.TabHistoryInactive,
                            HomeViewModel.View.Home
                        ),
                        0, 0
                    },
                    {
                        TabItem(
                            AppText.achievements_tab_item_title,
                            Images.TabAchievementsActive,
                            Images.TabAchievementsInactive,
                            HomeViewModel.View.Achievements
                        ),
                        1, 0
                    },
                    {
                        TabItem(
                            AppText.goals_tab_item_title,
                            Images.TabGoalsActive,
                            Images.TabGoalsInactive,
                            HomeViewModel.View.Goals
                        ),
                        2, 0
                    },
                    {
                        TabItem(
                            AppText.profile_tab_item_title,
                            Images.TabProfileActive,
                            Images.TabProfileInactive,
                            HomeViewModel.View.Profile
                        ),
                        3, 0
                    },
                    {line, 0, 4, 0, 1},
                }
            };

            Utils.Accessibility.Unused(layout);
            return layout;
        }

        private View TabItem(string title, ImageSource iconActive, ImageSource iconInactive, HomeViewModel.View view)
        {
            var iconSize = Sizes.Title;

            var iconNotSelected = new SvgCachedImage
            {
                Scale = Sizes.TabIconScale,
                Source = iconInactive,
                Aspect = Aspect.AspectFit,
            };
            Utils.Accessibility.Unused(iconNotSelected);
            iconNotSelected.SetBinding(IsVisibleProperty, nameof(ViewModel.CurrentView), 
                converter: new ValueConverterGroup()
                {
                    new ViewToBooleanConverter(view),
                    new InvertedBooleanConverter(),
                });

            var iconSelected = new SvgCachedImage
            {
                Scale = Sizes.TabIconScale,
                Source = iconActive,
                Aspect = Aspect.AspectFit,
                IsVisible = false,
            };
            Utils.Accessibility.Unused(iconSelected);
            iconSelected.SetBinding(IsVisibleProperty, nameof(ViewModel.CurrentView), 
                converter: new ViewToBooleanConverter(view));

            var lblTab = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                FontSize = Sizes.TextMicro,
                TextColor = Colors.TabPanelForeground,
                Margin = new Thickness(-20, 0),
                MaxLines = 1,
                LineBreakMode = LineBreakMode.NoWrap,
                Text = title,
            };
            Utils.Accessibility.Unused(lblTab);
            lblTab.SetBinding(Label.TextColorProperty, nameof(ViewModel.CurrentView),
                converter: new ValueConverterGroup()
                {
                    new ViewToBooleanConverter(view),
                    new BooleanToColorConverter(Colors.TextSpecial, Colors.TabPanelForeground),
                });

            var layout = new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 14f),
                RowSpacing = 2f,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() {Height = iconSize},
                    new RowDefinition() {Height = GridLength.Auto},
                },
                Children =
                {
                    {iconNotSelected, 0, 0},
                    {iconSelected, 0, 0},
                    {lblTab, 0, 1},
                }
            };
            layout.AddTouchCommand(new Binding(nameof(HomeViewModel.ChangeView)), view);

            Utils.Accessibility.InUse(layout, title, $"Velger {title}");

            return layout;
        }
    }
}
