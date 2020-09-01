using System.Collections.Generic;
using System.Linq;
using Bare10.Localization;
using Bare10.Pages.Custom;
using Bare10.Pages.Views.Onboarding.Base;
using Bare10.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Bare10.Pages.Views.Onboarding
{
    public class OnboardingHeightView : OnboardingItemView<OnboardingHeightViewModel>
    {
        public OnboardingHeightView()
        {
            ScreenHeight = App.ScreenHeight;

            var lblHeight = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                FontSize = Sizes.Title,
                Text = AppText.onboarding_height_title,
                FontAttributes = FontAttributes.Bold,
            };

            Content.Spacing = 20;

            AddAnimatedView(lblHeight);
            AddAnimatedView(HeightPicker(), view => {
                // Yuck..
                if (view is Grid grid)
                {
                    foreach (var gridChild in grid.Children)
                    {
                        if (gridChild is StackLayout stacklayout)
                        {
                            foreach (var stacklayoutChild in stacklayout.Children)
                            {
                                if (stacklayoutChild is Entry entry)
                                {
                                    entry.Focus();
                                }
                            }
                        }
                    }
                }
            });
        }

        private View HeightPicker()
        {
            View picker;

            if (Device.RuntimePlatform == Device.Android)
            {
                var entry = new Entry()
                {
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.End,
                    TextColor = Colors.Text,
                    BackgroundColor = Color.Transparent,
                    Keyboard = Keyboard.Numeric,
                    MaxLength = 3,
                    Placeholder = "165",
                    PlaceholderColor = Color.White.MultiplyAlpha(0.8f),
                };
                entry.SetBinding(Entry.TextProperty, nameof(ViewModel.HeightText), BindingMode.TwoWay);

                var lbl = new Label()
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Text = "cm",
                };

                picker = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        entry,
                        lbl,
                    }
                };
            }
            else
            {
                var iospicker = new Picker()
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Colors.Text,
                    Title = "Høyde",
                    BackgroundColor = Color.Transparent,
                };
                iospicker.SetBinding(Picker.SelectedIndexProperty, nameof(ViewModel.SelectedIndex), BindingMode.TwoWay);

                foreach (var h in OnboardingHeightViewModel.Heights)
                    iospicker.Items.Add($"{h}cm");

                picker = iospicker;
            }

            var border = new BorderCanvasView()
            {
                Border = new Thickness(0, 1, 0, 1),
                BorderColor = Colors.LightBorderColor.MultiplyAlpha(0.5),
                InputTransparent = true,
            };

            return new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    border,
                    picker,
                }
            };
        }


    }

    public class OnboardingHeightViewModel : OnboardingItemViewModel
    {
        public static IEnumerable<int> Heights { get; } = Enumerable.Range(100, 201);

        public override bool IsValid => Height > 0;

        public OnboardingHeightViewModel(OnboardingDelegate controller) : base(controller)
        {
            SelectedIndex = Heights.IndexOf(165);
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                SetProperty(ref _selectedIndex, value);
                Height = Heights.ElementAt(value);
            }
        }

        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                SetProperty(ref _height, value);
                Settings.UserHeight = value;
            }
        }

        private string _heightText;
        public string HeightText
        {
            get => _heightText;
            set
            {
                SetProperty(ref _heightText, value);
                if (int.TryParse(value, out var height))
                {
                    Height = height;
                }
            }
        }
    }
}
