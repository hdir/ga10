using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Bare10.Localization;
using Bare10.Pages.Custom;
using Bare10.Pages.Custom.ProgressBar;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.Utils.Views;
using Bare10.ViewModels.Items;
using Bare10.ViewModels.ViewCellsModels;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Achievements
{
    public class AchievementListItemViewCell : ViewCell<TieredAchievementViewModel>
    {
        public AchievementListItemViewCell()
        {
            var border = new BorderCanvasView(false)
            {
                Border = new Thickness(0, 1, 0, 0),
                BorderColor = Colors.LightBorderColor.MultiplyAlpha(0.2),
            };

            var title = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Sizes.TextMedium,
                TextColor = Colors.Text,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.Transparent,
            };
            title.SetBinding(Label.TextProperty, nameof(ViewModel.Title));

            var description = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                FontSize = Sizes.TextSmall,
                TextColor = Colors.TextFaded,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Transparent,
            };
            description.SetBinding(Label.TextProperty, nameof(ViewModel.Description));

            var achievements = new TierScrollView()
            {
                Divisions = 3,
                ItemTemplate = new DataTemplate(typeof(TierScrollViewCell)),
            };
            achievements.SetBinding(TierScrollView.ItemSourceProperty, nameof(ViewModel.Tiers));
            achievements.SetBinding(TierScrollView.ItemTappedCommandProperty, nameof(ViewModel.TierSelectedCommand));

            View = new Grid()
            {
                HeightRequest = 280,
                Children =
                {
                    border,
                    new Grid()
                    {
                        Padding = 20,
                        RowDefinitions = new RowDefinitionCollection()
                        {
                            new RowDefinition(){ Height = GridLength.Auto},
                            new RowDefinition(){ Height = GridLength.Auto},
                            new RowDefinition(){ Height = GridLength.Star},
                        },
                        Children =
                        {
                            { title, 0, 0 },
                            { description, 0, 1 },
                            { achievements, 0, 2 },
                        }
                    },
                }
            };
        }
    }

    public class TierScrollViewCell : MvxContentView<AchievementTierProgressViewModel>
    {
        public TierScrollViewCell()
        {
            var icon = new SvgCachedImage()
            {   
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Aspect = Aspect.AspectFit,
                Margin = 4,
            };
            icon.SetBinding(CachedImage.SourceProperty, nameof(ViewModel.Icon));
            icon.SetBinding(SvgCachedImage.ReplaceStringMapProperty, nameof(ViewModel.Tier),
                converter: new TierToReplacementStringMapConverter());

            var lblTier = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Sizes.TextMicro,
                TextColor = Colors.Text,
            };
            lblTier.SetBinding(Label.TextProperty, nameof(ViewModel.Title));


            var receivedDateSpan = new Span()
            {
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMicro - 2,
                FontFamily = Fonts.Normal,
            };
            receivedDateSpan.SetBinding(Span.TextProperty,
                nameof(ViewModel.DateCompleted),
                converter: new DateToStringConverter("dd.MM.yyyy"));

            var lblReceived = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Sizes.TextMicro - 2,
                TextColor = Colors.TextFaded,
                FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        new Span()
                        {
                            Text = AppText.achievement_time_received + " ",
                            TextColor = Colors.TextFaded,
                            FontSize = Sizes.TextMicro - 2,
                            FontFamily = Fonts.Normal,
                        },
                        receivedDateSpan
                    }
                }
            };
            lblReceived.SetBinding(OpacityProperty, nameof(ViewModel.IsCompleted),
                converter: new BooleanToDoubleConverter(1f, 0f));

            var miniProgress = new ProgressBarCanvasView()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 5,
                WidthRequest = 64,
                ProgressBackgroundColor = Colors.ProgressWheelBackground,
                ProgressColor = Colors.ProgressWheelForeground,
                Margin = new Thickness( 0, 2f, 0, 0),
            };
            miniProgress.SetBinding(OpacityProperty, nameof(ViewModel.IsCompleted),
                converter: new BooleanToDoubleConverter(0f, 1f));
            //miniProgress.SetBinding(OpacityProperty, nameof(ViewModel.ProgressNormalized),
            //    converter: new ProgressToDoubleConverter(false, false));
            miniProgress.SetBinding(ProgressBarCanvasViewBase.ProgressProperty, nameof(ViewModel.ProgressNormalized));

            var progressContainer = new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Children =
                {
                    lblReceived,
                    miniProgress,
                }
            };
            progressContainer.SetBinding(OpacityProperty, nameof(ViewModel.IsLocked),
                converter: new BooleanToDoubleConverter(0f, 1f));

            Content = new Grid()
            {
                Margin = 4,
                RowSpacing = 2,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){Height = GridLength.Star},
                    new RowDefinition(){Height = Sizes.TextMicro+2},
                    new RowDefinition(){Height = Sizes.TextMicro+2},
                },
                Children =
                {
                    { icon, 0, 0 },
                    { lblTier, 0, 1 },
                    { progressContainer, 0, 2 }, 
                },
            };
        }
    }

    public class TierScrollView : ScrollView
    {
        public double ChildWidth =>
            (Width - Padding.HorizontalThickness - Margin.HorizontalThickness) / Divisions;

        private new StackLayout Content { get; } = new StackLayout()
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Orientation = StackOrientation.Horizontal,
            Spacing = 0,
        };

        public TierScrollView()
        {
            base.Content = Content;
            Orientation = ScrollOrientation.Horizontal;

            SizeChanged += (sender, args) =>
            {
                SetupChildren();
            };
        }

        private void SetupChildren()
        {
            if (ItemSource is ICollection<AchievementTierProgressViewModel> tiers)
            {
                Content.Children.Clear();

                foreach (var tier in tiers)
                {
                    var container = new ContentView()
                    {
                        WidthRequest = ChildWidth,
                    };
                    container.AddTouch((sender, args) =>
                    {
                        ItemTappedCommand?.Execute(tier);
                    });

                    if (ItemTemplate?.CreateContent() is View content)
                    {
                        content.BindingContext = tier;
                        container.Content = content;
                    }
                    else
                    {
                        container.Content = new Label { Text = tier.ToString() };
                    }
                    Content.Children.Add(container);
                }
                InvalidateLayout();
            }
        }

        public static readonly BindableProperty ItemSourceProperty =
            BindableProperty.Create(
                nameof(ItemSource),
                typeof(ICollection),
                typeof(TierScrollView),
                default(ICollection),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((TierScrollView) bindableObject).SetupChildren();
                }
            );

        public ICollection ItemSource
        {
            get => (ICollection) GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }


        public static readonly BindableProperty DivisionsProperty =
            BindableProperty.Create(
                nameof(Divisions),
                typeof(ushort),
                typeof(TierScrollView),
                (ushort) 2,
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((TierScrollView) bindableObject).SetupChildren();
                }
            );

        public ushort Divisions
        {
            get => (ushort) GetValue(DivisionsProperty);
            set => SetValue(DivisionsProperty, value);
        }



        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(TierScrollView),
                default(DataTemplate),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((TierScrollView)bindableObject).SetupChildren();
                }
            );

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty ItemTappedCommandProperty =
            BindableProperty.Create(
                nameof(ItemTappedCommand),
                typeof(ICommand),
                typeof(TierScrollView),
                default(ICommand)
            );

        public ICommand ItemTappedCommand
        {
            get => (ICommand) GetValue(ItemTappedCommandProperty);
            set => SetValue(ItemTappedCommandProperty, value);
        }
    }

    public class ViewCell<T> : ViewCell where T : class
    {
        public T ViewModel { get; private set; }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is T vm)
            {
                ViewModel = vm;
            }
            else
            {
                throw new InvalidCastException($"{BindingContext.GetType()} is not {typeof(T)}.");
            }
        }
    }
}
