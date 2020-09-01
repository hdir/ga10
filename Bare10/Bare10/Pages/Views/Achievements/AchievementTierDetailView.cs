using System.Collections.Generic;
using System.Threading.Tasks;
using Bare10.Localization;
using Bare10.Pages.Custom;
using Bare10.Pages.Custom.Base;
using Bare10.Pages.Views.Achievements.ProgressView;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.ViewModels.Items;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;
using XFShapeView;

namespace Bare10.Pages.Views.Achievements
{
    public class AchievementTierDetailView : BaseAnimatedView<AchievementTierProgressViewModel>
    {
        protected new View Content { get; }

        public float YOffset { get; set; } = 500;

        private readonly View _tierTitle;
        private readonly View _progressionBars;

        private readonly View _bg;

        private readonly View[] _statusIcons = new View[2];

        public AchievementTierDetailView()
        {
            _bg = new Grid() { BackgroundColor = Colors.Background };

            _tierTitle = TitleView();

            _progressionBars = new TemplateView()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                Template = new AchievementTemplateSelector(),
            };
            _progressionBars.SetBinding(IsVisibleProperty, nameof(AchievementTierProgressViewModel.IsCompleted),
                converter: new InvertedBooleanConverter());

            var description = DescriptionView();

            const int iconSize = 36;

            var completed = CompletedView(iconSize, out _statusIcons[0]);
            completed.SetBinding(IsVisibleProperty, nameof(AchievementTierProgressViewModel.IsCompleted));

            var locked = LockedView(iconSize, out _statusIcons[1]);
            locked.SetBinding(IsVisibleProperty, nameof(AchievementTierProgressViewModel.IsLocked));


            var relativeLayout = new RelativeLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };

            relativeLayout.Children.Add(_tierTitle, 
                Constraint.RelativeToParent(p => 0),
                Constraint.RelativeToParent(p => 0),
                Constraint.RelativeToParent(p => p.Width)
                );

            relativeLayout.Children.Add(description, 
                Constraint.RelativeToParent(p => 0),
                Constraint.RelativeToView(_tierTitle, (p, v) => v.Y + v.Height + 20),
                Constraint.RelativeToParent(p => p.Width)
                );

            relativeLayout.Children.Add(completed, 
                Constraint.RelativeToParent(p => 0),
                Constraint.RelativeToView(description, (p, v) => v.Y + v.Height - iconSize * 0.5),
                Constraint.RelativeToParent(p => p.Width)
                );

            relativeLayout.Children.Add(locked, 
                Constraint.RelativeToParent(p => 0),
                Constraint.RelativeToView(description, (p, v) => v.Y + v.Height - iconSize * 0.5),
                Constraint.RelativeToParent(p => p.Width)
                );

            relativeLayout.Children.Add(_progressionBars, 
                Constraint.RelativeToParent(p => 0),
                Constraint.RelativeToView(description, (p, v) => v.Y + v.Height + 20),
                Constraint.RelativeToParent(p => p.Width)
                );

            Content = new ScrollView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Content = relativeLayout,
            };

            base.Content = new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Children =
                {
                    _bg,
                    Content,
                }
            };
        }

        #region Views

        private View TitleView()
        {
            var title = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMicro,
            };
            title.SetBinding(Label.TextProperty, nameof(ViewModel.Title));

            return title;
        }

        private View DescriptionView()
        {
            var icon = new SvgCachedImage()
            {
                HeightRequest = 130,
                Aspect = Aspect.AspectFit,
                FadeAnimationEnabled = false,
            };
            icon.SetBinding(CachedImage.SourceProperty, nameof(ViewModel.Icon));
            icon.SetBinding(SvgCachedImage.ReplaceStringMapProperty, nameof(ViewModel.Tier),
                converter: new TierToReplacementStringMapConverter());

            var lblCongratulations = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.Text,
                FontSize = Sizes.TextSmall,
                FontAttributes = FontAttributes.Bold,
                LineHeight = 1.2,
                Text = $"Gratulerer {Settings.UserName}!",
            };
            lblCongratulations.SetBinding(IsVisibleProperty, nameof(ViewModel.IsCompleted));

            var lblDescription = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.Text,
                FontSize = Sizes.TextSmall,
                LineHeight = 1.2,
            };
            lblDescription.SetBinding(Label.TextProperty, nameof(ViewModel.Description));
            lblDescription.SetBinding(IsVisibleProperty, nameof(ViewModel.IsCompleted),
                converter: new InvertedBooleanConverter());

            var lblDescriptionCompleted = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.Text,
                FontSize = Sizes.TextSmall,
                LineHeight = 1.2,
            };
            lblDescriptionCompleted.SetBinding(Label.TextProperty, nameof(ViewModel.CompletedDescription));
            lblDescriptionCompleted.SetBinding(IsVisibleProperty, nameof(ViewModel.IsCompleted));

            var lblContainer = new ShapeView()
            {
                CornerRadius = Sizes.CornerRadius,
                BorderWidth = 1,
                BorderColor = Colors.Border,
                Color = Colors.AchievementDescriptionBackground,
                Content = new StackLayout()
                {
                    Spacing = 20,
                    Margin = new Thickness(30, 70, 30, 50),
                    Children =
                    {
                        lblCongratulations,
                        lblDescription,
                        lblDescriptionCompleted,
                    }
                },
                Margin = new Thickness(10, 0),
            };

            return new Grid()
            {
                RowSpacing = -50,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                },
                Children =
                {
                    {lblContainer, 0, 1},
                    {icon, 0, 0},
                }
            };
        }

        private View CompletedView(float iconSize, out View statusIcon)
        {
            var receivedDateSpan = new Span()
            {
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMicro,
                FontFamily = Fonts.Normal,
            };
            receivedDateSpan.SetBinding(Span.TextProperty,
                nameof(AchievementTierProgressViewModel.DateCompleted),
                converter: new DateToStringConverter("dd.MM.yyyy"));

            var receivedText = new FormattedString()
            {
                Spans =
                {
                    new Span()
                    {
                        Text = AppText.achievement_time_received + " ",
                        TextColor = Colors.TextFaded,
                        FontSize = Sizes.TextMicro,
                        FontFamily = Fonts.Normal,
                    },
                    receivedDateSpan
                }
            };

            statusIcon = new ShapeView()
            {
                WidthRequest = iconSize,
                HeightRequest = iconSize,
                ShapeType = ShapeType.Circle,
                Content = new SvgCachedImage()
                {
                    Source = Images.AchievementProgressCheckmark,
                    ReplaceStringMap = new Dictionary<string, string>() { { "#FF5737", "#00000000" } },
                    Margin = 2,
                }
            };
            statusIcon.SetBinding(ShapeView.ColorProperty, nameof(AchievementTierProgressViewModel.Tier),
                converter: new TierToColorConverter());

            var lblCompleted = new Label()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(40, 0),
                TextColor = Colors.Text,
                FontSize = Sizes.TextSmall,
                FontAttributes = FontAttributes.Bold,
                LineHeight = 1.3,
            };
            lblCompleted.SetBinding(Label.TextProperty, nameof(AchievementTierProgressViewModel.UnlockedText));

            return new StackLayout()
            {
                Spacing = 20,
                Children =
                {
                    statusIcon,
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FormattedText = receivedText,
                    },
                    new BorderCanvasView()
                    {
                        BorderColor = Colors.Border,
                        Margin = new Thickness(0, 3, 0, 0),
                        Border = new Thickness(0, 1, 0, 0),
                        HeightRequest = 15,
                    },
                    lblCompleted,
                }
            };
        }

        private View LockedView(float iconSize, out View statusIcon)
        {
            statusIcon = new ShapeView()
            {
                ShapeType = ShapeType.Circle,
                WidthRequest = iconSize,
                HeightRequest = iconSize,
                BorderColor = Color.FromHex("#9ca5a6"),
                Color = Colors.Background,
                BorderWidth = 1,
                Content = new SvgCachedImage()
                {
                    Source = Images.AchievementProgressLocked,
                    Aspect = Aspect.AspectFit,
                }
            };

            var previousTier = new Span()
            {
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMicro,
                FontFamily = Fonts.Normal,
            };
            previousTier.SetBinding(Span.TextProperty, nameof(AchievementTierProgressViewModel.Tier),
                converter: new TierToNameConverter(-1, true));

            var currentTier = new Span()
            {
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMicro,
                FontFamily = Fonts.Normal,
            };
            currentTier.SetBinding(Span.TextProperty, nameof(AchievementTierProgressViewModel.Tier),
                converter: new TierToNameConverter(toLower: true));

            var receivedText = new FormattedString()
            {
                Spans =
                {
                    new Span()
                    {
                        Text = "Du må klare ",
                        TextColor = Colors.TextFaded,
                        FontSize = Sizes.TextMicro,
                        FontFamily = Fonts.Normal,
                    },
                    previousTier,
                    new Span()
                    {
                        Text = " før du kan starte på ",
                        TextColor = Colors.TextFaded,
                        FontSize = Sizes.TextMicro,
                        FontFamily = Fonts.Normal,
                    },
                    currentTier,
                }
            };

            return new StackLayout()
            {
                Spacing = 10,
                Children =
                {
                    statusIcon,
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FormattedText = receivedText,
                    },
                }
            };
        }

        #endregion
        
        #region Animation

        public override async Task AnimateIn()
        {
            // Reset values
            IsVisible = true;
            Opacity = 1f;

            _bg.Opacity = 0;
            _tierTitle.Opacity = 0;
            _progressionBars.Opacity = 0;

            foreach (var icon in _statusIcons)
                if (icon != null) icon.Scale = 1;

            if (Content != null)
            {
                // Animate
                Content.TranslationY = YOffset;
                await Task.WhenAll(
                    _bg.FadeTo(1, AnimationTime, Easing.Linear),
                    Content.TranslateTo(0, 0, AnimationTime, EasingIn)
                );
                await Task.WhenAll(
                    _tierTitle.FadeTo(1, AnimationTime),
                    _progressionBars.FadeTo(1, AnimationTime)
                );
                foreach (var icon in _statusIcons)
                {
#pragma warning disable 4014
                    icon?.ScaleTo(1.15, 250u, Easing.SpringOut);
#pragma warning restore 4014
                }
            }
            else
            {
                // Only animate Background
                await this.FadeTo(1, AnimationTime, Easing.Linear);
            }
        }

        public override async Task AnimateOut()
        {
            // Reset values
            Opacity = 1f;
            IsVisible = true;

            if (Content != null)
            {
                // Animate Background
                Content.TranslationY = 0;
                await Task.WhenAll(
                    this.FadeTo(0, AnimationTime/2, Easing.Linear),
                    Content.TranslateTo(0, YOffset, AnimationTime, EasingIn)
                );
            }
            else
            {
                // Animate Background
                await this.FadeTo(0, AnimationTime, Easing.Linear);
            }

            Opacity = 0;
            IsVisible = false;
        }
        #endregion

        #region Bindable Properties

        public static readonly BindableProperty ContentBindingContextProperty =
            BindableProperty.Create(
                nameof(ContentBindingContext),
                typeof(object),
                typeof(AchievementTierDetailView),
                default(object),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((AchievementTierDetailView) bindableObject).ContentBindingContextPropertyChanged();
                }
            );

        private void ContentBindingContextPropertyChanged()
        {
            Content.BindingContext = ContentBindingContext;
        }

        public object ContentBindingContext
        {
            get => (object) GetValue(ContentBindingContextProperty);
            set => SetValue(ContentBindingContextProperty, value);
        }

        #endregion
    }
}