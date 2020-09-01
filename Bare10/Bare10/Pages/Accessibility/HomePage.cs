using Bare10.Localization;
using Bare10.Resources;
using Bare10.ViewModels.Accessibility;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;


namespace Bare10.Pages.Accessibility
{
    [MvxContentPagePresentation(Animated = false, WrapInNavigationPage = true, NoHistory = true)]
    public class HomePage : MvxContentPage<HomeViewModel>
    {
        public HomePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            BackgroundColor = Colors.Background;

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    HelloName,
                    LoadingStatus(),
                    TodaysBriskWalking(),
                    TodaysRegularWalking(),
                    TodaysGoalProgress(),
                    HistoryButton(),
                    AchievementsButton(),
                    GoalsButton(),
                    ProfileButton(),
                },
            };
            //root object is not part of accessibility navigation
            Utils.Accessibility.Unused(layout);

            var scrollLayout = new ScrollView
            {
                Content = layout,
            };
            Utils.Accessibility.Unused(scrollLayout);

            Content = scrollLayout;
        }

        #region Views

        private View HelloName => Utils.Accessibility.CreateLabelWithoutName(
            nameof(HomeViewModel.HelloMessage));

        private View LoadingStatus() => Utils.Accessibility.CreateLabel(
            AppText.history_loading_message_label,
            nameof(HomeViewModel.LoadingStatusText));

        private View TodaysBriskWalking() => Utils.Accessibility.CreateLabel(
            AppText.brisk_walk,
            nameof(HomeViewModel.TodaysBriskWalkingMinutes));

        private View TodaysRegularWalking() => Utils.Accessibility.CreateLabel(
            AppText.regular_walk,
            nameof(HomeViewModel.TodaysRegularWalkingMinutes));

        private View TodaysGoalProgress() => Utils.Accessibility.CreateLabel(
            AppText.accessibility_history_daily_goal_label,
            nameof(HomeViewModel.TodaysGoalProgress));

        private View HistoryButton() => Utils.Accessibility.CreateButton(
            AppText.accessibility_history_button_title,
            nameof(HomeViewModel.NavigateToHistory));

        private View AchievementsButton() => Utils.Accessibility.CreateButton(
            AppText.achievements_tab_item_title,
            nameof(HomeViewModel.NavigateToAchievements));

        private View GoalsButton() => Utils.Accessibility.CreateButton(
            AppText.goals_tab_item_title,
            nameof(HomeViewModel.NavigateToGoals));

        private View ProfileButton() => Utils.Accessibility.CreateButton(
            AppText.profile_tab_item_title,
            nameof(HomeViewModel.NavigateToProfile));

        #endregion

    }
}
