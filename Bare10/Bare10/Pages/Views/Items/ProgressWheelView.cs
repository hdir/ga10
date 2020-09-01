using Bare10.Content;
using Bare10.Localization;
using Bare10.Pages.Custom.ProgressWheel;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.Utils.Effects;
using Bare10.ViewModels.Items;
using FFImageLoading.Svg.Forms;
using Lottie.Forms;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Items
{
    public class ProgressWheelView : MvxContentView<ProgressViewModel>
    {
        public ProgressWheelView()
        {
            var progressWheel = new ProgressWheelAnimatedView()
            {
                Margin = new Thickness(10, 0),
                MillisecondsToCompleteWheel = 6000,
                SegmentCount = GoalToSegmentCountConverter.ToSegments(Settings.CurrentGoal),
                TotalProgress = Settings.CurrentGoal.RequiredMinutes(),

            };
            progressWheel.SetBinding(
                ProgressWheelAnimatedView.TargetProgressProperty, 
                nameof(ViewModel.MinutesBriskWalked));

            progressWheel.Effects.Add(new ViewLifeCycleEffect(loaded: (sender, args) =>
            {
                ViewModel.GoalCompleted = ViewModel.GoalCompleted ==
                                          Settings.CurrentGoal.RequiredMinutes() >=
                                          progressWheel.TargetProgress;
                progressWheel.SetInitialProgress((float) ViewModel?.MinutesBriskWalked);
            }));

            var completeAnimation = new AnimationView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Animation = "check.json",
                Loop = false,
                Margin = new Thickness(0, 0, 0, 10),
                IsPlaying = true,
            };
            completeAnimation.SetBinding(IsVisibleProperty, nameof(ViewModel.GoalCompleted));

            progressWheel.OnAnimationFinished += () =>
            {
                ViewModel.GoalCompleted = progressWheel.CurrentProgress >= Settings.CurrentGoal.RequiredMinutes();

                if (ViewModel.GoalCompleted)
                    completeAnimation.Play();
            };

            var incomplete = IncompleteContent();
            incomplete.SetBinding(IsVisibleProperty, nameof(ViewModel.GoalCompleted), converter: new InvertedBooleanConverter());
            var complete = CompleteContent();
            complete.SetBinding(IsVisibleProperty, nameof(ViewModel.GoalCompleted));


            Content = new Grid()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){ Height = GridLength.Star },
                    new RowDefinition(){ Height = GridLength.Star },
                },
                Children =
                {
                    {progressWheel, 0, 1, 0, 2},
                    {incomplete, 0, 1, 0, 2},
                    {completeAnimation, 0, 1, 0, 2},    // tmp while check.json is not cropped
                    //{completeAnimation, 0, 0 },
                    {complete, 0, 1 },
                }
            };
        }

        private View IncompleteContent()
        {
            var fontSize = Sizes.Title + 10;

            var spanGoalTimes = new Span()
            {
                TextColor = Colors.Text,
                FontSize = fontSize,
                Text = GoalToTimesTextValueConverter.ToText(Settings.CurrentGoal),
            };

            var lblGoal = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        spanGoalTimes,
                        new Span()
                        {
                            Text = $"10",
                            TextColor = Colors.TextSpecial,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = fontSize,
                        },
                        new Span()
                        {
                            Text = $" min",
                            TextColor = Colors.Text,
                            FontSize = fontSize,
                        }
                    }
                }
            };

            return new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Text = AppText.history_todays_goal_title.ToUpper(),
                        FontSize = Sizes.TextMicro
                    },
                    new SvgCachedImage()
                    {
                        Source = Images.OnboardingShoe,
                        HeightRequest = 28,
                        Aspect = Aspect.AspectFit,
                    },
                    lblGoal,
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Text = AppText.brisk_walk.ToUpper(),
                        FontSize = Sizes.TextMicro
                    },
                }
            };
        }

        private View CompleteContent()
        {
            return new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 10),
                Children =
                {
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Text = AppText.history_todays_goal_completed_title.Replace("{name}", Settings.UserName),
                        FontSize = Sizes.TextSmall,
                        FontAttributes = FontAttributes.Bold,
                    },
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Text = AppText.history_todays_goal_completed_text,
                        FontSize = Sizes.TextSmall
                    },
                }
            };
        }
    }
}
