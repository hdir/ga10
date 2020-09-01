using System;
using Xamarin.Forms;

namespace Bare10.Utils.Effects
{
    public class ViewLifeCycleEffect : BaseEffect
    {
        public const string EffectName = "LifecycleEffect";

        public event EventHandler<EventArgs> Loaded;
        public event EventHandler<EventArgs> Unloaded;

        public ViewLifeCycleEffect(EventHandler<EventArgs> loaded = null, EventHandler<EventArgs> unloaded = null) : base($"{EffectName}")
        {
            Loaded = loaded;
            Unloaded = unloaded;
        }

        public void RaiseLoaded(Element element) => Loaded?.Invoke(element, EventArgs.Empty);
        public void RaiseUnloaded(Element element) => Unloaded?.Invoke(element, EventArgs.Empty);
    }
}
