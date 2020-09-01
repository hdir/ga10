using System;
using System.Linq;
using Bare10.iOS.Effects;
using Bare10.Utils.Effects;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName(BaseEffect.EffectGroupName)]
[assembly: ExportEffect(typeof(IosLifeCycleEffect), ViewLifeCycleEffect.EffectName)]
namespace Bare10.iOS.Effects
{
    public class IosLifeCycleEffect : PlatformEffect
    {
        private const NSKeyValueObservingOptions ObservingOptions = NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.OldNew | NSKeyValueObservingOptions.Prior;

        private ViewLifeCycleEffect _viewLifeCycleEffect;
        private IDisposable _isLoadedObserverDisposable;

        protected override void OnAttached()
        {
            _viewLifeCycleEffect = Element.Effects.OfType<ViewLifeCycleEffect>().FirstOrDefault();

            var nativeView = Control ?? Container;
            _isLoadedObserverDisposable = nativeView?.AddObserver("superview", ObservingOptions, IsViewLoadedObserver);
        }

        protected override void OnDetached()
        {
            try
            {
                _viewLifeCycleEffect.RaiseUnloaded(Element);
                _isLoadedObserverDisposable.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void IsViewLoadedObserver(NSObservedChange nsObservedChange)
        {
            if (nsObservedChange == null)
                return;

            if (nsObservedChange.NewValue != null && !nsObservedChange.NewValue.Equals(NSNull.Null))
                _viewLifeCycleEffect?.RaiseLoaded(Element);
            else if (nsObservedChange.OldValue != null && !nsObservedChange.OldValue.Equals(NSNull.Null))
                _viewLifeCycleEffect?.RaiseUnloaded(Element);
            else
                _viewLifeCycleEffect.RaiseLoaded(Element);
        }
    }
}