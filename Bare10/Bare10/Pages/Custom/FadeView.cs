using System.Threading.Tasks;
using Bare10.Pages.Custom.Base;
using Xamarin.Forms;

namespace Bare10.Pages.Custom
{
    public class FadeView : BaseAnimatedView
    {
        public FadeView()
        {
            Opacity = 0;
        }

        public override async Task AnimateIn()
        {
            IsVisible = true;
            await Task.WhenAll(
                Content.FadeTo(1f, AnimationTime, Easing.Linear),
                this.FadeTo(1f, AnimationTime));
        }

        public override async Task AnimateOut()
        {
            await Content.FadeTo(0f, AnimationTime, Easing.Linear);
            IsVisible = false;
        }
    }
}
