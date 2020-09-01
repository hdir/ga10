using System;
using Bare10.Pages.Custom;
using Bare10.Pages.Views.Onboarding;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.Utils.Effects;
using Bare10.ViewModels;
using CarouselView.FormsPlugin.Abstractions;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;
using XFShapeView;

namespace Bare10.Pages
{
    [MvxContentPagePresentation(NoHistory = true, WrapInNavigationPage = false)]
    public class OnboardingPage : MvxContentPage<OnboardingViewModel>
    {
        private readonly StackLayout _indicators;

        public OnboardingPage()
        {
            var bgTop = new OrganicBubblesCanvasView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                Color = Colors.OnboardingOrganicColor,
                Margin = new Thickness(-300, -140, -30, 0),
                HeightRequest = 300,
                RotationTime = 10000,
                Rotation = -20,
            };
            var bgBottom = new OrganicBubblesCanvasView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                Color = Colors.OnboardingOrganicColor,
                Margin = new Thickness(20, 0, -300,-200),
                HeightRequest = 400,
                RotationTime = 10000,
                Rotation = -60,
            };

            var carousel = new CarouselViewControl()
            {
                AnimateTransition = false,
                ShowArrows = false,
                IsSwipeEnabled = false,
                Opacity = 0, //until loaded
                Orientation = CarouselViewOrientation.Vertical,
                InterPageSpacing = 1,
                Margin = new Thickness(30, -30, 30, 0),
                ItemTemplate = new OnboardingDataTemplateSelector(),
            };
            carousel.SetBinding(CarouselViewControl.PositionProperty, nameof(ViewModel.CarouselPosition));
            carousel.SetBinding(CarouselViewControl.ItemsSourceProperty, nameof(ViewModel.Pages));

            var next = ButtonNext();
            next.HorizontalOptions = LayoutOptions.End;

            _indicators = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                Orientation = StackOrientation.Horizontal,
                Spacing = 4,
                Margin = 20,
            };

            Content = new Grid()
            {
                BackgroundColor = Colors.Background,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){ Height = GridLength.Star},
                    new RowDefinition(){ Height = GridLength.Auto},
                },
                Children =
                {
                    {bgTop, 0, 1, 0, 2},
                    {bgBottom, 0, 1, 0, 2},
                    {carousel, 0, 1, 0, 2},
                    {_indicators, 0, 0},
                    {next, 0, 1},
                }
            };

            carousel.Effects.Add(new ViewLifeCycleEffect(loaded: (sender, args) =>
            {
                ViewModel.ViewLoaded(); // shows first page
                carousel.Opacity = 1f;  // makes view visible again
            }));
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                _indicators.Children.Clear();
                for (var i = 0; i < ViewModel.Pages.Count; i++)
                {
                    _indicators.Children.Add(Indicator(i));
                }
            }
        }

        private View ButtonNext()
        {
            const int btnHeight = 46;
            const int btnWidth = 140;

            var layout = new Frame
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = btnHeight,
                WidthRequest = btnWidth,
                Padding = 0,
                Margin = 30,
                CornerRadius = btnHeight / 2f,
                HasShadow = false,
                TranslationY = 100,
            };
            layout.SetBinding(BackgroundColorProperty, new Binding(
                nameof(ViewModel.ButtonIsValid),
                BindingMode.OneWay,
                new BooleanToColorConverter(Colors.OnboardingButton, Colors.OnboardingButtonInactive)
            ));

            var btn = new Button()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Transparent,
                TextColor = Colors.Text,
                FontSize = Sizes.TextMedium,
                Text = "Neste",
                IsEnabled = false,
            };
            btn.SetBinding(Button.CommandProperty, nameof(ViewModel.NextClickedCommmand));
            btn.SetBinding(IsEnabledProperty, nameof(ViewModel.IsBusy),
                converter: new InvertedBooleanConverter());

            btn.Pressed += (sender, args) => { layout.Scale = 0.95f; };
            btn.Released += (sender, args) => { layout.Scale = 1f; };

            btn.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(IsEnabled))
                {
                    if (btn.IsEnabled)
                    {
                        layout.TranslateTo(0, 0, 600, Easing.SpringOut);
                        layout.FadeTo(1, 600);
                    }
                    else
                    {
                        layout.TranslateTo(0, 100, 200, Easing.SinIn);
                        layout.FadeTo(0, 200);
                    }
                }
            };

            layout.Content = btn;

            return layout;
        }

        private View Indicator(int pageNumber)
        {
            const int size = 6;

            var bg = new ShapeView()
            {
                HeightRequest = size,
                WidthRequest = size,
                Margin = .5,
                ShapeType = ShapeType.Circle,
                Color = Colors.Border,
            };

            var fg = new ShapeView()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = size,
                WidthRequest = size,
                ShapeType = ShapeType.Circle,
                Color = Colors.TextSpecial,
            };
            fg.SetBinding(IsVisibleProperty, nameof(ViewModel.CarouselPosition),
                BindingMode.OneWay, new IntToBooleanConverter(pageNumber));

            return new Grid()
            {
                HeightRequest = size,
                WidthRequest = size,
                Children =
                {
                    { bg, 0, 0 },
                    { fg, 0, 0 }
                }
            };
        }
    }

    public class OnboardingDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            switch (item.GetType())
            {
                case Type welcomeType when welcomeType == typeof(OnboardingWelcomeViewModel):
                    return new DataTemplate(typeof(OnboardingWelcomeView));
                case Type welcomeType when welcomeType == typeof(OnboardingHeightViewModel):
                    return new DataTemplate(typeof(OnboardingHeightView));
                case Type welcomeType when welcomeType == typeof(OnboardingTermsViewModel):
                    return new DataTemplate(typeof(OnboardingTermsView));
                case Type welcomeType when welcomeType == typeof(OnboardingOutroViewModel):
                    return new DataTemplate(typeof(OnboardingOutroView));
            }

            return new DataTemplate(Fallback);
        }

        private static View Fallback()
        {
            return new Grid()
            {
                BackgroundColor = Color.Magenta,
                Children =
                {
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Text = "View missing",
                        TextColor = Color.Black,
                    }
                }
            };
        }
    }
}
