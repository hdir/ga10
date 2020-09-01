using System.Threading.Tasks;
using Bare10.Pages.Custom.Base;
using Xamarin.Forms;

namespace Bare10.Pages.Custom
{
    public class AnimatedSlideView : BaseAnimatedView
    {
        public bool UseHeight { get; set; }
        public bool UseWidth { get; set; }

        public override async Task AnimateIn()
        {
            if (UseHeight && VerticalDistance <= 0 && Height > 0)
                VerticalDistance = Height;
            if (UseWidth && HorizontalDistance <= 0 && Width > 0)
                HorizontalDistance = Width;

            await Content.TranslateTo(HorizontalDistance, VerticalDistance, AnimationTime, EasingIn);
        }

        public override async Task AnimateOut()
        {
            await Content.TranslateTo(0, 0, AnimationTime, EasingOut);
        }

        public static readonly BindableProperty HorizontalDistanceProperty =
            BindableProperty.Create(
                nameof(HorizontalDistance),
                typeof(double),
                typeof(AnimatedSlideView),
                default(double)
            );

        public double HorizontalDistance
        {
            get => (double) GetValue(HorizontalDistanceProperty);
            set => SetValue(HorizontalDistanceProperty, value);
        }

        public static readonly BindableProperty VerticalDistanceProperty =
            BindableProperty.Create(
                nameof(VerticalDistance),
                typeof(double),
                typeof(AnimatedSlideView),
                default(double)
            );

        public double VerticalDistance
        {
            get => (double) GetValue(VerticalDistanceProperty);
            set => SetValue(VerticalDistanceProperty, value);
        }
    }
}
