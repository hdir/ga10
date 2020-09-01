using Bare10.Pages.Card;
using Bare10.Resources;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.Utils.Converters;
using Bare10.Utils.Views;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using Lottie.Forms;
using MLToolkit.Forms.SwipeCardView;
using MLToolkit.Forms.SwipeCardView.Core;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace Bare10.Pages.Popups
{
    public class BulletinPopupPage : PopupPage
    {
        private readonly SwipeCardView _swipeView;

        public BulletinPopupPage()
        {
            BackgroundColor = Colors.BulletinPopupBackground;

            _swipeView = new SwipeCardView()
            {
                Padding = new Thickness(30, 20, 30, 45),
                ItemTemplate = new DataTemplate(ContentView),
                IsVisible = true,
                BindingContext = BulletinService.Instance,
            };
            _swipeView.SetBinding(SwipeCardView.ItemsSourceProperty, nameof(BulletinService.Bulletins));

            _swipeView.Swiped += (sender, args) =>
            {
                var bulletin = (Bulletin)args.Item;

                if (BulletinService.Instance.Bulletins.IndexOf(bulletin) >= BulletinService.Instance.Bulletins.Count - 1)
                {
                    BulletinService.Instance.ClearBulletins();
                }
            };

            Content = _swipeView;
        }

        public async void InvokeSwipe()
        {
            await _swipeView.InvokeSwipe(SwipeCardDirection.Right);
        }

        private View ContentView()
        {
            var imgIcon = new SvgCachedImage()
            {
                HeightRequest = 140,
                WidthRequest = 140,
                Aspect = Aspect.AspectFit,
            };
            imgIcon.SetBinding(CachedImage.SourceProperty, nameof(Bulletin.Icon));
            imgIcon.SetBinding(SvgCachedImage.ReplaceStringMapProperty, nameof(Bulletin.Tier),
                converter: new TierToReplacementStringMapConverter());
            imgIcon.SetBinding(IsVisibleProperty, nameof(Bulletin.Icon),
                converter: new NullToBooleanConverter());

            var animationIcon = new AnimationView
            {
                HeightRequest = 140,
                WidthRequest = 140,
                Loop = false,
                IsPlaying = true,
            };
            animationIcon.SetBinding(AnimationView.AnimationProperty, nameof(Bulletin.Animation));
            animationIcon.SetBinding(IsVisibleProperty, nameof(Bulletin.Animation),
                converter: new NullToBooleanConverter());

            var lblTitle = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.TextSpecial,
                FontAttributes = FontAttributes.Bold,
                FontSize = Sizes.TextLarge,
            };
            lblTitle.SetBinding(Label.TextProperty, nameof(Bulletin.Title));

            var lblText = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.InfoPopupText,
                FontSize = Sizes.TextSmall,
                LineHeight = 1.2f,
            };
            lblText.SetBinding(Label.TextProperty, nameof(Bulletin.Description));

            var content = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(40),
                Spacing = 20,
                Children =
                {
                    imgIcon,
                    //animationIcon,
                    new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = 0,
                        Children =
                        {
                            lblTitle,
                            DefaultView.UnderLine,
                        }
                    },
                    lblText,
                }
            };

            var cardView = new CardView(content)
            {
                TranslationY = 20,
                Scale = 0.90f,
                //BindingContext = item,
                AnchorY = 1, // anchor to bottom
            };
            cardView.SetBinding(CardView.CloseButtonCommandProperty, nameof(Bulletin.CloseCommand));

            return cardView;
        }
    }
}
