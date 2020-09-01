using Bare10.Localization;
using Bare10.Pages.Custom.ProgressBar;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.ViewModels.Items;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Achievements.ProgressView
{
    public class ThirtyMinutesView : AchievementProgressViewBase
    {
        protected override View ProgressView()
        {
            var subProgressBar = new StripedProgressBarCanvasView()
            {
                HeightRequest = 80,
                TotalProgress = 30,
                TextColor = Colors.Text,
                ProgressColor = Colors.ProgressWheelForeground,
                ProgressBackgroundColor = Colors.ProgressWheelBackground,
                TextSize = Sizes.ProgressBarText,
            };
            subProgressBar.SetBinding(ProgressBarCanvasViewBase.ProgressProperty, 
                nameof(AchievementTierProgressViewModel.SubProgress));

            var totalProgressBar = new FinishLineProgressBarCanvasView()
            {
                HeightRequest = 60,
                TextColor = Colors.Text,
                ProgressColor = Colors.Text,
                ProgressBackgroundColor = Colors.ProgressWheelBackground,
                TextSize = Sizes.ProgressBarText,
            };
            totalProgressBar.SetBinding(ProgressBarCanvasViewBase.ProgressProperty,
                nameof(AchievementTierProgressViewModel.Progress));
            totalProgressBar.SetBinding(ProgressBarCanvasViewBase.TotalProgressProperty,
                nameof(AchievementTierProgressViewModel.ProgressGoal));
            totalProgressBar.SetBinding(FinishLineProgressBarCanvasView.LabelCountProperty,
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
                        Text = AppText.achievement_tier_minutes,
                    },
                    subProgressBar,
                    new Label()
                    {
                        Margin = new Thickness(0, 10, 0, 0),
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.TextFaded,
                        FontSize = Sizes.TextSmall,
                        Text = AppText.achievement_tier_minutes_total,
                    },
                    totalProgressBar,
                },
            };
        }
    }
}