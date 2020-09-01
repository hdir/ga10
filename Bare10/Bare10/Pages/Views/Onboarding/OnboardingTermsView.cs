using Bare10.Localization;
using Bare10.Pages.Custom;
using Bare10.Pages.Popups;
using Bare10.Pages.Views.Onboarding.Base;
using Bare10.Resources;
using Bare10.Utils.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using XFShapeView;

namespace Bare10.Pages.Views.Onboarding
{
    public class OnboardingTermsView : OnboardingItemView<OnboardingTermsViewModel>
    {
        public OnboardingTermsView()
        {
            ScreenHeight = App.ScreenHeight;

            var lblTitle = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                FormattedText = new FormattedString(),
                WidthRequest = 200,
                Text = AppText.onboarding_terms_text,
                TextColor = Colors.Text,
                FontAttributes = FontAttributes.Bold,
                FontSize = Sizes.Title,
            };

            var termsView = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(20, 20, 20, 40),
                Spacing = 10,
                Children =
                {
                    TermListItemView(AppText.onboarding_term_1),
                    TermListItemView(AppText.onboarding_term_2),
                    TermListItemView(AppText.onboarding_term_3),
                }
            };

            var lblStatisticsTextAccept = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Start,
                TextColor = Colors.Text,
                FontSize = Sizes.TextSmall,
                Text = AppText.onboarding_statistics_accept,
            };

            var spanTerms = new Span()
            {
                Text = AppText.onboarding_terms_accept_terms,
                TextDecorations = TextDecorations.Underline,
                TextColor = Colors.TextSpecial,
                FontSize = Sizes.TextSmall,
            };
            spanTerms.AddTouchCommand(new Binding(nameof(ViewModel.TermsClickedCommand)));

            var lblTermsTextAccept = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0,
                Margin = 0,
                Children =
                {
                    new Label()
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        VerticalTextAlignment = TextAlignment.Start,
                        TextColor = Colors.Text,
                        FontSize = Sizes.TextSmall,
                        FormattedText = new FormattedString()
                        {
                            Spans =
                            {
                                new Span()
                                {
                                    Text = AppText.onboarding_terms_accept,
                                    FontSize = Sizes.TextSmall,
                                },
                                spanTerms,
                            }
                        },
                    }
                }
            };

            var statisticsSwitch = new Switch()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 28,
            };
            statisticsSwitch.SetBinding(Switch.IsToggledProperty, nameof(ViewModel.StatisticsAccepted), BindingMode.TwoWay);

            var statisticsLayout = new Grid()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(16, 5),
                ColumnSpacing = 22,
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() {Width = GridLength.Star},
                    new ColumnDefinition() {Width = GridLength.Auto},
                },
                Children =
                {
                    { lblStatisticsTextAccept, 0, 0 },
                    { statisticsSwitch, 1, 0 },
                }
            };


            var termsSwitch = new Switch()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 28,
            };
            termsSwitch.SetBinding(Switch.IsToggledProperty, nameof(ViewModel.TermsAccepted), BindingMode.TwoWay);

            var termsLayout = new Grid()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(16, 5),
                ColumnSpacing = 22,
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() {Width = GridLength.Star},
                    new ColumnDefinition() {Width = GridLength.Auto},
                },
                Children =
                {
                    { lblTermsTextAccept, 0, 0 },
                    { termsSwitch, 1, 0 },
                }
            };

            AddAnimatedView(lblTitle);
            AddAnimatedView(termsView);
            AddAnimatedView(statisticsLayout);
            AddAnimatedView(termsLayout);
            //AddAnimatedView(new StackLayout()
            //{
            //    Orientation = StackOrientation.Vertical,
            //    Children =
            //    {
            //        statisticsLayout,
            //        termsLayout,
            //    }
            //});
        }

        private static View TermListItemView(string text)
        {
            var lbl = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "\u2713",
                TextColor = Colors.TextSpecial,
                FontSize = Sizes.TextLarge,
            };

            var lblText = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                Text = text,
                FontSize = Sizes.TextSmall+2,
            };

            return new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                ColumnSpacing = 15,
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Star},
                },
                Children =
                {
                    { lbl, 0 , 0 },
                    { lblText, 1 , 0 },
                }
            };
        }

    }

    public class OnboardingTermsViewModel : OnboardingItemViewModel
    {
        public OnboardingTermsViewModel(OnboardingDelegate controller) : base(controller)
        {
        }
        public IMvxCommand CheckBoxClickedClicked => new MvxCommand(() =>
        {
            TermsAccepted = !TermsAccepted;
        });

        public IMvxCommand TermsClickedCommand => new MvxAsyncCommand(async () =>
        {
            Analytics.TrackEvent(TrackingEvents.ItemTapped,
                new TrackingEvents.ItemTappedArgs(TrackingEvents.ItemsToTap.Terms));
            await PopupNavigation.Instance.PushAsync(new TermsPopupPage());
        });

        private bool _termsAccepted;
        public bool TermsAccepted
        {
            get => _termsAccepted;
            set
            {
                SetProperty(ref _termsAccepted, value);
                IsValid = value;
            }
        }

        public bool StatisticsAccepted
        {
            get => Settings.AllowTracking;
            set
            {
                Settings.AllowTracking = value;
                AppCenter.SetEnabledAsync(value);
            }
        }
    }
}
