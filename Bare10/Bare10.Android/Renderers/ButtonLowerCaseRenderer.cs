using Android.Content;
using Bare10.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(ButtonLowerCaseRenderer))]
namespace Bare10.Droid.Renderers
{
    public class ButtonLowerCaseRenderer : ButtonRenderer
    {
        public ButtonLowerCaseRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            Control?.SetAllCaps(false);
        }
    }
}