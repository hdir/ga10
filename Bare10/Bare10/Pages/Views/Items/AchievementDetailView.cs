using System.Threading.Tasks;
using Bare10.Localization;
using Bare10.Pages.Custom.Base;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.ViewModels.ViewCellsModels;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;
using XFShapeView;

namespace Bare10.Pages.Views.Items
{
    //Legacy
    public class AchievementDetailView : BaseAnimatedView
    {
        private readonly View _achievementIcon;

        private const uint IconSize = 140;

        public AchievementDetailView()
        {
            BackgroundColor = Colors.Background;
            InputTransparent = true;
            Opacity = 0;

            _achievementIcon = CreateAchievementIcon();
            var details = new ShapeView()
            {
                VerticalOptions = LayoutOptions.Start,
                CornerRadius = Sizes.CornerRadius,
                BorderWidth = 1f,
                BorderColor = Colors.AchievementOutline,
                Content = AchievementTextLayout(),
            };

            var health = HealthBoxContainer(HealthTextLayout());
            health.Margin = new Thickness(0, -Sizes.CornerRadius, 0, 0);    // offset to overlay details
            health.SetBinding(IsVisibleProperty, nameof(AchievementViewModel.HasBeenAchieved));

            Content = new ScrollView()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = new Grid()
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(10, 30, 10, 10),
                    RowSpacing = 0,
                    RowDefinitions = new RowDefinitionCollection()
                    {
                        new RowDefinition() {Height = IconSize/2f},
                        new RowDefinition() {Height = GridLength.Auto},
                        new RowDefinition() {Height = GridLength.Auto},
                    },
                    Children =
                    {
                        {details, 0, 1},
                        {_achievementIcon, 0, 1, 0, 2},
                        {health, 0, 2},
                    }
                }
            };

            Content.BindingContextChanged += (sender, args) =>
            {
                if (Content is ScrollView scrollView)
                {
                    scrollView.SetScrolledPosition(0, 0);
                }
            };
        }

        public override async Task AnimateIn()
        {
            IsVisible = true;
            InputTransparent = false;

            Content.TranslationY = 100;
            _achievementIcon.Scale = 0.85f;

            await Task.WhenAll(
                this.FadeTo(1f, 200u, Easing.SinOut),
                Content.TranslateTo(0, 0, 250, EasingIn)
            );
            await _achievementIcon.ScaleTo(1f, 100u, easing: Easing.SpringOut);
        }

        public override async Task AnimateOut()
        {
            await Task.WhenAll(
                Content.TranslateTo(0, Height, AnimationTime, EasingOut),
                this.FadeTo(0f, 250u, Easing.CubicOut)
            );
            InputTransparent = true;
            IsVisible = false;
        }

        #region Create Views

        private View CreateAchievementIcon()
        {
            var iconIncomplete = new SvgCachedImage
            {
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Colors.Background,
                Aspect = Aspect.AspectFit,
                FadeAnimationForCachedImages = false,
            };
            iconIncomplete.SetBinding(CachedImage.SourceProperty, nameof(AchievementViewModel.IconIncomplete));
            iconIncomplete.SetBinding(IsVisibleProperty, nameof(AchievementViewModel.HasBeenAchieved), converter: new InvertedBooleanConverter());

            var iconComplete = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit,
                FadeAnimationForCachedImages = false,
            };
            iconComplete.SetBinding(CachedImage.SourceProperty, nameof(AchievementViewModel.IconComplete));
            iconComplete.SetBinding(IsVisibleProperty, nameof(AchievementViewModel.HasBeenAchieved));

            return new Grid()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = Device.RuntimePlatform == Device.Android ? Colors.Background : Color.Transparent,
                HeightRequest = IconSize,
                Children =
                {
                    iconIncomplete,
                    iconComplete,
                }
            };
        }

        private View AchievementTextLayout()
        {
            var spanTime = new Span();
            spanTime.SetBinding(Span.TextProperty, nameof(AchievementViewModel.TimeAchieved));

            var timeAchievedLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.AchievementDescriptionText,
                FontSize = Sizes.TextMicro,
                FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        new Span()
                        {
                            Text = AppText.achievement_time_received.ToUpper() + " "
                        },
                        spanTime,
                    }
                }
            };
            timeAchievedLabel.SetBinding(IsVisibleProperty, nameof(AchievementViewModel.HasBeenAchieved));

            var titleLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.AchievementDescriptionTitle,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Sizes.Title,
                FontAttributes = FontAttributes.Bold,
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(AchievementViewModel.Title));

            var description = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.AchievementDescriptionText,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Sizes.TextSmall,
                LineHeight = 1.2,
            };
            description.SetBinding(Label.TextProperty, nameof(AchievementViewModel.Description));

            const int innerPadding = 20;

            return new StackLayout
            {
                Spacing = 10,
                Padding = new Thickness(innerPadding, IconSize/2f + innerPadding, innerPadding, innerPadding + Sizes.CornerRadius + 20),
                Margin = new Thickness(0, 0, 0, -Sizes.CornerRadius),   // hide below health view
                Children =
                {
                    timeAchievedLabel,
                    titleLabel,
                    description,
                }
            };
        }

        private View HealthTextLayout()
        {
            var lblHealthDescription = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.AchievementHealthText,
                FontSize = Sizes.TextSmall + 1,
                FontAttributes = FontAttributes.Bold,
                LineHeight = 1.2,
                Margin = 30,
            };
            lblHealthDescription.SetBinding(Label.TextProperty, nameof(AchievementViewModel.HealthDescription));

            return lblHealthDescription;
        }

        private View HealthBoxContainer(View content)
        {
            const int heartIconSize = 40;

            var heartIcon = new SvgCachedImage
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, -heartIconSize/2f, 0, 0),
                HeightRequest = heartIconSize,
                WidthRequest = heartIconSize,
                Source = Images.HealthHeart,
            };
            heartIcon.SetBinding(IsVisibleProperty, nameof(AchievementViewModel.HasBeenAchieved));

            var roundedBottom = new ShapeView()
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                Color = Colors.AchievementHealthBackground,
                CornerRadius = Sizes.CornerRadius,
                HeightRequest = Sizes.CornerRadius * 2f,
                Margin = new Thickness(0, 0, 0, -Sizes.CornerRadius),
            };

            var contentContainer = new ShapeView()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.AchievementHealthBackground,
                Content = content,
            };

            return new Grid()
            {
                RowSpacing = 0,
                Children =
                {
                    roundedBottom,
                    contentContainer,
                    heartIcon,
                }
            };
        }

        #endregion
    }
}
