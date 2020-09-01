using Xamarin.Forms;

namespace Bare10.Utils.Effects
{   
    public abstract class BaseEffect : RoutingEffect
    {
        public const string EffectGroupName = "Bare10Effects";

        protected BaseEffect(string effectName) : base($"{EffectGroupName}.{effectName}")
        {
        }
    }
}
