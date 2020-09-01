using Bare10.Localization;
using Bare10.Pages.Custom.ProgressBar;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.ViewModels.Items;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Achievements.ProgressView
{
    public class ReachedGoalView : AchievementProgressViewBase
    {
        protected override View ProgressView()
        {
            var progressBar = new FinishLineProgressBarCanvasView()
            {
                HeightRequest = 60,
                TextColor = Colors.ProgressWheelForeground,
                ProgressColor = Colors.ProgressWheelForeground,
                ProgressBackgroundColor = Colors.ProgressWheelBackground,
                TextSize = Sizes.ProgressBarText,
            };
            progressBar.SetBinding(ProgressBarCanvasViewBase.ProgressProperty,
                nameof(AchievementTierProgressViewModel.Progress));
            progressBar.SetBinding(ProgressBarCanvasViewBase.TotalProgressProperty,
                nameof(AchievementTierProgressViewModel.ProgressGoal));
            progressBar.SetBinding(FinishLineProgressBarCanvasView.LabelCountProperty,
                nameof(AchievementTierProgressViewModel.ProgressGoal),
                converter: new ProgressToLabelCountConverter());

            return new StackLayout()
            {
                Margin = new Thickness(40, 0),
                Spacing = 8,
                Children =
                {
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.TextFaded,
                        FontSize = Sizes.TextSmall,
                        Text = AppText.achievement_tier_goals,
                    },
                    progressBar,
                },
            };
        }
    }
}