using System.Reflection;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Widget;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Android.App.Application;

[assembly: ExportRenderer(typeof(Label), typeof(Bare10.Droid.Renderers.LabelRenderer))]
namespace Bare10.Droid.Renderers
{
    public class LabelRenderer : Xamarin.Forms.Platform.Android.LabelRenderer
    {
        public LabelRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;
            try
            {
                Control.SetTextSize(ComplexUnitType.Dip, (float) e.NewElement.FontSize);
                UpdateFormattedText();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void UpdateFormattedText()
        {
            if (Element?.FormattedText == null)
                return;

            var extensionType = typeof(FormattedStringExtensions);
            var type = extensionType.GetNestedType("FontSpan", BindingFlags.NonPublic);
            var ss = new SpannableString(Control.TextFormatted);
            var spans = ss.GetSpans(0, ss.ToString().Length, Class.FromType(type));
            foreach (var span in spans)
            {
                var font = (Font)type.GetProperty("Font").GetValue(span, null);
                if ((font.FontFamily ?? Element.FontFamily) != null)
                {
                    var start = ss.GetSpanStart(span);
                    var end = ss.GetSpanEnd(span);
                    var flags = ss.GetSpanFlags(span);
                    ss.RemoveSpan(span);
                    var newSpan = new CustomTypefaceSpan(Control, Element, font);
                    ss.SetSpan(newSpan, start, end, flags);
                }
            }
            Control.TextFormatted = ss;
        }
    }

    public class CustomTypefaceSpan : MetricAffectingSpan
    {
        private readonly Typeface _typeFace;
        private readonly TextView _textView;
        private Font _font;

        public CustomTypefaceSpan(TextView textView, Label label, Font font)
        {
            _textView = textView;
            _font = font;
            _typeFace = textView.Typeface;
        }

        private static string GetFontName(string fontFamily, FontAttributes fontAttributes)
        {
            var postfix = "Regular";
            var bold = fontAttributes.HasFlag(FontAttributes.Bold);
            var italic = fontAttributes.HasFlag(FontAttributes.Italic);
            if (bold && italic) { postfix = "BoldItalic"; }
            else if (bold) { postfix = "Bold"; }
            else if (italic) { postfix = "Italic"; }

            return $"{fontFamily}-{postfix}.ttf";
        }

        public override void UpdateDrawState(TextPaint paint)
        {
            ApplyCustomTypeFace(paint);
        }

        public override void UpdateMeasureState(TextPaint paint)
        {
            ApplyCustomTypeFace(paint);
        }

        private void ApplyCustomTypeFace(Paint paint)
        {
            paint.SetTypeface(_typeFace);
            paint.TextSize = TypedValue.ApplyDimension(ComplexUnitType.Dip, _font.ToScaledPixel(), _textView.Resources.DisplayMetrics);
        }
    }
}