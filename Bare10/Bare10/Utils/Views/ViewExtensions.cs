using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bare10.Utils.Views
{
    public static class ViewExtensions
    {
        public static void AddTouch(this View view, EventHandler tapped)
        {
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += tapped;
            view.GestureRecognizers.Add(tgr);
        }

        public static void AddTouch(this Span span, EventHandler tapped)
        {
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += tapped;
            span.GestureRecognizers.Add(tgr);
        }

        public static void AddTouchCommand(this View view, BindingBase command, object parameter = null, Action tappedEvent = null)
        {
            var tgr = new TapGestureRecognizer();
            tgr.SetBinding(TapGestureRecognizer.CommandProperty, command);

            if(parameter != null)
                tgr.CommandParameter = parameter;

            if (tappedEvent != null)
                tgr.Tapped += (sender, args) => tappedEvent?.Invoke();

            view.GestureRecognizers.Add(tgr);
        }

        public static void AddTouchCommand(this Span view, BindingBase command, object parameter = null)
        {
            var tgr = new TapGestureRecognizer();
            tgr.SetBinding(TapGestureRecognizer.CommandProperty, command);
            if(parameter != null)
            {
                tgr.CommandParameter = parameter;
            }
            view.GestureRecognizers.Add(tgr);
        }

        public static void AddTouchCommand(this View view, BindingBase command, BindingBase parameter)
        {
            var tgr = new TapGestureRecognizer();
            tgr.SetBinding(TapGestureRecognizer.CommandProperty, command);
            tgr.SetBinding(TapGestureRecognizer.CommandParameterProperty, parameter);
            view.GestureRecognizers.Add(tgr);
        }

        public static Task<bool> ColorTo(this VisualElement self, Color fromColor, Color toColor, Action<Color> callback, uint length = 250, Easing easing = null)
        {
            Color Transform(double t) => 
                Color.FromRgba(fromColor.R + t * (toColor.R - fromColor.R), 
                    fromColor.G + t * (toColor.G - fromColor.G), fromColor.B + t * (toColor.B - fromColor.B), 
                    fromColor.A + t * (toColor.A - fromColor.A)
                );

            return ColorAnimation(self, "ColorTo", Transform, callback, length, easing);
        }

        public static void CancelAnimation(this VisualElement self)
        {
            self.AbortAnimation("ColorTo");
        }

        static Task<bool> ColorAnimation(VisualElement element, string name, Func<double, Color> transform, Action<Color> callback, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate<Color>(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
    }
}
