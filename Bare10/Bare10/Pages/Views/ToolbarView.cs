using System.Threading.Tasks;
using Bare10.Pages.Custom;
using Bare10.Pages.Custom.Base;
using Bare10.Resources;
using Bare10.Utils;
using Bare10.Utils.Converters;
using Bare10.Utils.Views;
using Bare10.ViewModels.Base;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Views
{
    public class ToolbarView : Grid
    {
        public ToolbarView()
        {
            BindingContext = Toolbar.Instance;

            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Center;
            Padding = new Thickness(10, 0);

            ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition(){Width = GridLength.Auto},
                new ColumnDefinition(){Width = GridLength.Star},
                new ColumnDefinition(){Width = GridLength.Auto},
            };

            Children.Add(Icon(), 0, 3, 0, 1);
            Children.Add(Title(), 0, 3, 0, 1);
            Children.Add(Share(), 2, 0);
            Children.Add(Back(), 0, 0);
        }

        public View Back()
        {
            var imgArrow = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Fill,
                WidthRequest = 30f,
                Aspect = Aspect.AspectFit,
                Source = Images.ToolbarArrow,
                ScaleX = -1,
            };
            imgArrow.ReplaceColor(Colors.Text);

            var layout = new FadeView()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 4, 4, 4),
                AnimationTime = 100,
                IsVisible = false,
                Content = imgArrow,
            };
            layout.AddTouchCommand(new Binding(nameof(Toolbar.BackButtonClicked)));
            layout.SetBinding(BaseAnimatedView.AnimateProperty, nameof(Toolbar.ShowBackButton));

            return layout;
        }

        public View Title()
        {
            var lblTitle = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                FontSize = Sizes.TextMedium,
            };
            lblTitle.SetBinding(Label.FormattedTextProperty, nameof(Toolbar.Title));
#if DEBUG
            lblTitle.AddTouchCommand(new Binding(nameof(Toolbar.ShowDebugPopup)));
#endif

            var layout = new FadeView()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Fill,
                AnimationTime = 100,
                IsVisible = false,
                Content = lblTitle,
            };
            layout.SetBinding(BaseAnimatedView.AnimateProperty, nameof(Toolbar.Title), 
                converter:new EmptyStringToBooleanConverter(false));

            return layout;
        }

        private View Icon()
        {
            var icon = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 22,
                Aspect = Aspect.AspectFit,
                Source = Images.ToolbarIcon,
            };
            icon.ReplaceColor(Colors.TextSpecial);
#if DEBUG
            icon.AddTouchCommand(new Binding(nameof(Toolbar.ShowDebugPopup)));
#endif

            //var layout = new FadeView()
            //{
            //    HorizontalOptions = LayoutOptions.Center,
            //    VerticalOptions = LayoutOptions.Fill,
            //    AnimationTime = 100,
            //    IsVisible = false,
            //    Content = icon,
            //};
            icon.SetBinding(OpacityProperty, nameof(Toolbar.ShowIcon), converter: new BooleanToDoubleConverter(1, 0));

            return icon;
        }

        public View Share()
        {
            var imgShare = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 24f,
                Aspect = Aspect.AspectFit,
                Source = Images.ToolbarShare,
            };
            imgShare.ReplaceColor(Colors.Text);

            var layout = new FadeView()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Fill,
                AnimationTime = 100,
                IsVisible = false,
                Padding = new Thickness(4, 0, 0, 0),
                Content = imgShare,
            };
            layout.SetBinding(BaseAnimatedView.AnimateProperty, nameof(Toolbar.ShowShareButton));
            layout.AddTouchCommand(new Binding(nameof(Toolbar.ShareButtonClicked)), tappedEvent: async () =>
                {
                    imgShare.ReplaceColor(Colors.TextSpecial);
                    await Task.Delay(1000);
                    imgShare.ReplaceColor(Colors.Text);
                }
            );

            return layout;
        }
    }
}
