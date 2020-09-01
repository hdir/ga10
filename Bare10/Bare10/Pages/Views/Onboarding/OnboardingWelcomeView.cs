using Bare10.Localization;
using Bare10.Pages.Custom;
using Bare10.Pages.Views.Onboarding.Base;
using Bare10.Resources;
using Bare10.Utils;
using Bare10.Utils.Views;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Onboarding
{
    public class OnboardingWelcomeView : OnboardingItemView<OnboardingWelcomeViewModel>
    {
        public OnboardingWelcomeView()
        {
            ScreenHeight = App.ScreenHeight;

            var icShoe = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit,
                HeightRequest = 64,
                Source = Images.OnboardingShoe,
            };

            var lblWelcomeText = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,FontSize = Sizes.Title,
                FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        new Span()
                        {
                            Text = AppText.onboarding_first_title + "\n",
                            FontSize = Sizes.Title,
                        },
                        new Span()
                        {
                            Text = AppText.onboarding_first_text,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = Sizes.TextLarge,
                        },
                    }
                },
            };

            var lblNameTitle = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                Text = AppText.onboarding_entry_title,
                FontSize = Sizes.Title,
            };

            var entry = new Entry()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.TextSpecial,
                FontAttributes = FontAttributes.Bold,
                FontSize = Sizes.TextLarge,
                Keyboard = Keyboard.Text,
                IsTextPredictionEnabled = false,
                Margin = 0,
            };
            entry.SetBinding(Entry.TextProperty, nameof(ViewModel.UserName));
            entry.SetBinding(Entry.ReturnCommandProperty, nameof(ViewModel.NextClickedCommand));

            var nextArrow = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = Sizes.TextSmall - 2,
                Aspect = Aspect.AspectFit,
                Source = Images.ToolbarArrow,
                Margin = new Thickness(-10f, -10f, 0, -10f),
            };
            nextArrow.ReplaceColor(Colors.TextSpecial);
            nextArrow.SetBinding(IsVisibleProperty, nameof(ViewModel.IsValid));
            nextArrow.AddTouchCommand(new Binding(nameof(ViewModel.NextClickedCommand)));

            var line = new BorderCanvasView()
            {
                Border = new Thickness(0, 0, 0, 1),
                BorderColor = Colors.LightBorderColor.MultiplyAlpha(0.5),
                InputTransparent = true,
            };

            var entryContainer = new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20),
                RowSpacing = 10,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){Height = GridLength.Auto},
                    new RowDefinition(){Height = GridLength.Auto},
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition(){Width = GridLength.Star},
                    new ColumnDefinition(){Width = 40},
                },
                Children =
                {
                    {line, 0, 2, 1, 2},
                    {lblNameTitle, 0, 0},
                    {entry, 0, 1},
                    {nextArrow, 1, 1},
                }
            };

            entry.Focused += (sender, args) =>
            {
                icShoe.FadeTo(0);
                lblWelcomeText.FadeTo(0);
                entryContainer.TranslateTo(0, -120);
                entryContainer.RowSpacing = 40;
            };

            entry.Unfocused += (sender, args) =>
            {
                icShoe.FadeTo(1, 400, Easing.CubicIn);
                lblWelcomeText.FadeTo(1, 400, Easing.CubicIn);
                entryContainer.TranslateTo(0, 0);
                entryContainer.RowSpacing = 10;
            };

            Content.Spacing = 20;

            AddAnimatedView(icShoe);
            AddAnimatedView(lblWelcomeText);
            AddAnimatedView(entryContainer);
        }
    }

    public class OnboardingWelcomeViewModel : OnboardingItemViewModel
    {
        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                IsValid = !string.IsNullOrEmpty(_userName);
            }
        }

        public OnboardingWelcomeViewModel(OnboardingDelegate controller) : base(controller)
        {
        }
    }
}
