using Bare10.Localization;
using Bare10.Pages.Custom;
using Bare10.Pages.Custom.Base;
using Bare10.Resources;
using Bare10.Utils;
using Bare10.Utils.Converters;
using Bare10.Utils.Views;
using Bare10.ViewModels.ViewCellsModels;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Goals
{
    internal class GoalViewCell : MvxContentView<GoalViewModel>
    {
        private const int CellPadding = 18;
        private const int IconSize = 96;

        public GoalViewCell()
        {
            var icon = CreateIcon();
            var lblTitle = CreateTitle();
            var lblText = CreateText();

            var description = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    lblTitle,
                    lblText,
                }
            };

            var btnConfirm = new Grid()
            {
                WidthRequest = IconSize,
                BackgroundColor = Colors.GoalBackgroundActive,
                Margin = new Thickness(0, 0, -10, 0), // for the bounce animation
                Padding = new Thickness(4, 4, 14, 4),
                Children =
                {
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        Text = AppText.goals_page_confirm_goal,
                        FontSize = Sizes.TextMedium,
                    },
                }
            };
            btnConfirm.AddTouchCommand(new Binding(nameof(ViewModel.ChooseCommand)));

            var layout = new Grid
            {
                Margin = new Thickness(0, 0, -IconSize, 0),
                ColumnSpacing = CellPadding,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = IconSize },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = IconSize },
                },
                Children =
                {
                    { icon, 0, 0 },
                    { description, 1, 0 },
                    { btnConfirm, 2, 0 },
                }
            };

            var animated = new AnimatedSlideView()
            {
                HorizontalDistance = -IconSize,
                Content = layout,
                EasingIn = Easing.SpringOut,
                AnimationTime = 400u,
            };
            animated.SetBinding(BaseAnimatedView.AnimateProperty, nameof(ViewModel.CanChangeGoal));

            var roundedRect = new Frame()
            {
                HeightRequest = IconSize + 2 * CellPadding,
                CornerRadius = Sizes.CornerRadius,
                BackgroundColor = Colors.GoalBackgroundInactive,
                Padding = new Thickness(CellPadding, 0, 0, 0),
                Content = animated,
                Margin = new Thickness(0, 5, 0, 5),
                HasShadow = false,
            };
            roundedRect.SetBinding(IsClippedToBoundsProperty, nameof(ViewModel.IsCurrentGoal));

            roundedRect.PropertyChanged += async (sender, args) =>
            {
                if (args.PropertyName == nameof(IsClippedToBounds))
                {
                    if (sender is Frame frame)
                    {
                        await frame.ColorTo(
                            frame.BackgroundColor,
                            frame.IsClippedToBounds ? Colors.GoalBackgroundActive : Colors.GoalBackgroundInactive,
                            color =>
                            {
                                frame.BackgroundColor = color;
                            },
                            400u,
                            Easing.SinOut
                        );
                    }
                }
            };

            Content = roundedRect;

            //Accessibility.InUse(this, new Binding(nameof(ViewModel.Description)));
        }

        private View CreateText()
        {
            var lblDescription = new Label
            {
                FontSize = Sizes.TextSmall,
                TextColor = Colors.GoalTextActive,
            };
            lblDescription.SetBinding(Label.TextColorProperty, nameof(ViewModel.IsCurrentGoal),
                converter: new BooleanToColorConverter(Colors.GoalTextActive, Colors.GoalTextInactive));
            lblDescription.SetBinding(Label.TextProperty, nameof(ViewModel.Description));

            return lblDescription;
        }

        private View CreateTitle()
        {
            var lblTitle = new Label
            {
                FontSize = Sizes.TextMedium,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.GoalTitleActive,
            };
            lblTitle.SetBinding(Label.TextColorProperty, nameof(ViewModel.IsCurrentGoal),
                converter: new BooleanToColorConverter(Colors.GoalTitleActive, Colors.GoalTitleInactive));
            lblTitle.SetBinding(Label.TextProperty, nameof(ViewModel.Title));

            return lblTitle;
        }

        private View CreateIcon()
        {
            var svgCircle = new SvgCachedImage
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = IconSize,
                WidthRequest = IconSize,
                Aspect = Aspect.AspectFit,
            };
            svgCircle.SetBinding(CachedImage.SourceProperty, nameof(ViewModel.Goal),
                converter: new GoalToBorderIconConverter());
            svgCircle.SetBinding(SvgCachedImage.ReplaceStringMapProperty, nameof(ViewModel.IsCurrentGoal),
                converter: new BooleanToReplacementStringMapConverter(Colors.GoalIconActive, Colors.GoalIconInactive));

            var svgShoe = new SvgCachedImage
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = IconSize / 3f,
                WidthRequest = IconSize / 3f,
                Source = Images.OnboardingShoe,
                Aspect = Aspect.AspectFit,
            };
            svgShoe.ReplaceColor(Colors.GoalTextInactive, replaceText: "\"#fff\"");

            return new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    svgCircle,
                    svgShoe,
                }
            };
        }
    }
}
