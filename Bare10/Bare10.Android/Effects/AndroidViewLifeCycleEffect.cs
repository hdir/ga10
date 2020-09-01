using System.Linq;
using Bare10.Droid.Effects;
using Bare10.Utils.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ResolutionGroupName(BaseEffect.EffectGroupName)]
[assembly: ExportEffect(typeof(AndroidViewLifecycleEffect), ViewLifeCycleEffect.EffectName)]
namespace Bare10.Droid.Effects
{
    public class AndroidViewLifecycleEffect : PlatformEffect
    {
        private View _nativeView;
        private ViewLifeCycleEffect _viewLifeCycleEffect;

        protected override void OnAttached()
        {
            _viewLifeCycleEffect = Element.Effects.OfType<ViewLifeCycleEffect>().FirstOrDefault();

            _nativeView = Control ?? Container;
            _nativeView.ViewAttachedToWindow += OnViewAttachedToWindow;
            _nativeView.ViewDetachedFromWindow += OnViewDetachedFromWindow;
        }

        protected override void OnDetached()
        {
            _viewLifeCycleEffect.RaiseUnloaded(Element);
            _nativeView.ViewAttachedToWindow -= OnViewAttachedToWindow;
            _nativeView.ViewDetachedFromWindow -= OnViewDetachedFromWindow;
        }

        private void OnViewAttachedToWindow(object sender, View.ViewAttachedToWindowEventArgs e) => _viewLifeCycleEffect?.RaiseLoaded(Element);
        private void OnViewDetachedFromWindow(object sender, View.ViewDetachedFromWindowEventArgs e) => _viewLifeCycleEffect?.RaiseUnloaded(Element);
    }
}
