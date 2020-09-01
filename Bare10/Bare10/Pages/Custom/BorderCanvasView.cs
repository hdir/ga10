using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom
{
    public class BorderCanvasView : SKCanvasView
    {
        public BorderCanvasView(bool scale = true)
        {
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;
            IgnorePixelScaling = scale;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            // left
            using (var paint = MakePaint((float)Border.Left))
            {
                var w = (float)Border.Left / 2f;
                var p0 = new SKPoint(w, info.Height);
                var p1 = new SKPoint(w, 0);
                if (w > 0)
                    canvas.DrawLine(p0, p1, paint);
            }

            // top
            using (var paint = MakePaint((float)Border.Top))
            {
                var w = (float)Border.Top / 2f;
                var p0 = new SKPoint(0, 0 + w);
                var p1 = new SKPoint(info.Width, 0 + w);
                if (w > 0)
                    canvas.DrawLine(p0, p1, paint);
            }

            // right
            using (var paint = MakePaint((float)Border.Right))
            {
                var w = (float)Border.Right / 2f;
                var p0 = new SKPoint(info.Width - w, 0);
                var p1 = new SKPoint(info.Width - w, info.Height);
                if (w > 0)
                    canvas.DrawLine(p0, p1, paint);
            }

            // bottom
            using (var paint = MakePaint((float)Border.Bottom))
            {
                var w = (float)Border.Bottom / 2f;
                var p0 = new SKPoint(info.Width, info.Height - w);
                var p1 = new SKPoint(0, info.Height - w);
                if (w > 0)
                    canvas.DrawLine(p0, p1, paint);
            }
        }

        private SKPaint MakePaint(float width)
        {
            return new SKPaint()
            {
                StrokeWidth = width,
                Color = BorderColor.ToSKColor(),
                IsAntialias = false,
                Style = SKPaintStyle.Stroke,
                IsStroke = true,
                StrokeCap = SKStrokeCap.Square,
            };
        }

        #region Bindable Properties

        public static readonly BindableProperty BorderProperty =
            BindableProperty.Create(
                nameof(Border),
                typeof(Thickness),
                typeof(BorderCanvasView),
                default(Thickness),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((BorderCanvasView)bindableObject).BorderPropertyChanged();
                }
            );

        private void BorderPropertyChanged()
        {
            InvalidateSurface();
        }

        public Thickness Border
        {
            get => (Thickness)GetValue(BorderProperty);
            set => SetValue(BorderProperty, value);
        }


        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(
                nameof(BorderColor),
                typeof(Color),
                typeof(BorderCanvasView),
                Color.Black,
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((BorderCanvasView)bindableObject).ColorPropertyChanged();
                }
            );

        private void ColorPropertyChanged()
        {
            InvalidateSurface();
        }

        public Color BorderColor
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        #endregion
    }
}
