using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using Bare10.Pages.Custom.Base;
using Bare10.Resources;
using Bare10.Utils;
using Bare10.Utils.Converters;
using Bare10.Utils.Views;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom
{
    public class NotifyView : BaseAnimatedView
    {
        public new StackLayout Content { get; }

        public NotifyView()
        {
            VerticalOptions = LayoutOptions.Start;
            IsClippedToBounds = false;

            Content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
            };

            base.Content = Content;
        }

        public override async Task AnimateIn()
        {
            IsVisible = true;
            await this.TranslateTo(0, 0, easing: Easing.SinOut);
        }

        public override async Task AnimateOut()
        {
            await this.TranslateTo(0, -Height, easing: Easing.SinIn);
            IsVisible = false;
        }

        private async void CreateContent()
        {
            if (MessageBindingContext?.Count > 0)
            {
                Content.Children.Clear();

                foreach (var element in MessageBindingContext)
                {
                    if (element is NotifyCenter.MessageDetails message)
                    {
                        var view = MessageView();
                        view.BindingContext = message;

                        Content.Children.Add(view);
                    }
                }

                InvalidateLayout();

                await AnimateIn();
            }
            else
            {
                await AnimateOut();
            }
        }

        private View MessageView()
        {
            var loadingIndicator = new ActivityIndicator()
            {
                Color = Colors.Text,
                IsRunning = true,
                HeightRequest = Sizes.TextSmall,
                WidthRequest = Sizes.TextSmall,
            };
            loadingIndicator.SetBinding(IsVisibleProperty, nameof(NotifyCenter.MessageDetails.Type),
                converter: new ObjectToBooleanConverter(NotifyCenter.InfoType.Loading));

            var infoIndicator = new SvgCachedImage()
            {
                IsVisible = false,
                Source = Images.HistoryInfo,
                HeightRequest = Sizes.TextSmall,
                WidthRequest = Sizes.TextSmall,
            };
            infoIndicator.ReplaceColor(Colors.Text);
            infoIndicator.SetBinding(IsVisibleProperty, nameof(NotifyCenter.MessageDetails.Type),
                converter: new ObjectToBooleanConverter(NotifyCenter.InfoType.Info));

            var errorIndicator = new SvgCachedImage()
            {
                IsVisible = false,
                Source = Images.PopupCloseSmall,
                HeightRequest = Sizes.TextSmall,
                WidthRequest = Sizes.TextSmall,
            };
            errorIndicator.ReplaceColor(Colors.Text);
            errorIndicator.SetBinding(IsVisibleProperty, nameof(NotifyCenter.MessageDetails.Type),
                converter: new ObjectToBooleanConverter(NotifyCenter.InfoType.Error));

            var warningIndicator = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                IsVisible = false,
                Text = "\u26A0",
                FontSize = Sizes.TextMicro + 1,
            };
            warningIndicator.SetBinding(IsVisibleProperty, nameof(NotifyCenter.MessageDetails.Type),
                converter: new ObjectToBooleanConverter(NotifyCenter.InfoType.Warning));

            var lbl = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Colors.Text,
                FontSize = Sizes.TextMicro + 1,
            };
            lbl.SetBinding(Label.TextProperty, nameof(NotifyCenter.MessageDetails.Text));

            var layout = new ContentView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Content = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Fill,
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(0, 12),
                    Spacing = 10,
                    Children =
                    {
                        loadingIndicator,
                        infoIndicator,
                        errorIndicator,
                        warningIndicator,
                        lbl,
                    }
                }
            };
            layout.AddTouchCommand(new Binding(nameof(NotifyCenter.MessageDetails.ClickedCommand)));
            layout.SetBinding(BackgroundColorProperty, nameof(NotifyCenter.MessageDetails.Type),
                converter: new InfoLevelToColorConverter());

            return layout;
        }

        public static readonly BindableProperty MessageBindingContextProperty =
            BindableProperty.Create(
                nameof(MessageBindingContext),
                typeof(ICollection),
                typeof(NotifyView),
                default(ICollection),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((NotifyView)bindableObject).MessageBindingContextPropertyChanged((ICollection)oldValue, (ICollection)newValue);
                }
            );

        private void MessageBindingContextPropertyChanged(ICollection oldValue, ICollection newValue)
        {
            if (oldValue is INotifyPropertyChanged oldNotify)
                oldNotify.PropertyChanged -= OnCollectionChanged;
            if (newValue is INotifyPropertyChanged newNotify)
                newNotify.PropertyChanged += OnCollectionChanged;
            CreateContent();
        }

        private void OnCollectionChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateContent();
        }

        public ICollection MessageBindingContext
        {
            get => (ICollection) GetValue(MessageBindingContextProperty);
            set => SetValue(MessageBindingContextProperty, value);
        }

    }
}
