using System;
using System.Windows.Input;
using Bare10.Pages.Custom;
using Bare10.Resources;
using Bare10.Utils.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Card
{
    public class CardView : Frame
    {
        public const uint FooterHeight = 80;

        public Action CloseButtonClicked { get; set; }

        public CardView(View content)
        {
            HeightRequest = 200;
            CornerRadius = Sizes.CornerRadius;
            BackgroundColor = Colors.InfoPopupBackground;
            HasShadow = true;
            BorderColor = Device.RuntimePlatform == Device.Android
                ? Color.FromRgba(0, 0, 0, 0.3f)
                : Color.Transparent;

            var btnClose = DefaultView.CloseButton();
            btnClose.HorizontalOptions = LayoutOptions.Center;
            btnClose.VerticalOptions = LayoutOptions.End;
            btnClose.Margin = new Thickness(0, 0, 0, 20);
            btnClose.AddTouch((sender, args) => OnClose());
            Padding = 0;

            content.Margin = new Thickness(
                content.Margin.Left, 
                content.Margin.Top, 
                content.Margin.Right, 
                Math.Max(CornerRadius, content.Margin.Bottom));

            Content = new Grid()
            {
                Padding = 0,
                Children =
                {
                    content,
                    new GradientView(Colors.InfoPopupBackground)
                    {
                        VerticalOptions = LayoutOptions.End,
                        HeightRequest = FooterHeight,
                        Orientation = StackOrientation.Vertical,
                        InputTransparent = true,
                        ScaleY = -1,
                        Margin = new Thickness(2, CornerRadius),
                    },
                    btnClose,
                }
            };
        }

        private void OnClose()
        {
            CloseButtonClicked?.Invoke();
            CloseButtonCommand?.Execute(this);
        }

        public static readonly BindableProperty CloseButtonCommandProperty =
            BindableProperty.Create(
                nameof(CloseButtonCommand),
                typeof(ICommand),
                typeof(CardView),
                default(ICommand)
            );

        public ICommand CloseButtonCommand
        {
            get => (ICommand) GetValue(CloseButtonCommandProperty);
            set => SetValue(CloseButtonCommandProperty, value);
        }
    }
}