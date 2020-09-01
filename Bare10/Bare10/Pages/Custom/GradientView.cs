using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom
{
    public class GradientView : SKCanvasView
    {
        public StackOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (value == _orientation)
                    return;
                _orientation = value;
                InvalidateSurface();
            }
        }

        private readonly SKColor _fromColor;
        private readonly SKColor _toColor;
        private StackOrientation _orientation = StackOrientation.Horizontal;

        public GradientView(Color fromColor)
        {
            _fromColor = fromColor.ToSKColor();
            _toColor = fromColor.MultiplyAlpha(0).ToSKColor();
        }

        public GradientView(Color fromColor, Color toColor)
        {
            _fromColor = fromColor.ToSKColor();
            _toColor = toColor.ToSKColor();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            using (var paint = new SKPaint())
            {
                // Fill view
                var rect = new SKRect(0, 0, info.Width, info.Height);

                var start = StartPointFromOrientation(rect, Orientation);
                var end = EndPointFromOrientation(rect, Orientation);

                // Create linear gradient from up to down
                paint.Shader = SKShader.CreateLinearGradient(
                    start,
                    end,
                    new[] { _fromColor, _toColor },
                    new[] { 0, 1.0f },
                    SKShaderTileMode.Clamp);

                // Draw the gradient on the rectangle
                canvas.DrawPaint(paint);
            }
        }

        private SKPoint StartPointFromOrientation(SKRect rect, StackOrientation orientation)
        {
            switch (orientation)
            {
                case StackOrientation.Vertical:
                    return new SKPoint(rect.MidX, rect.Top);
                case StackOrientation.Horizontal:
                    return new SKPoint(rect.Left, rect.MidY);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        private SKPoint EndPointFromOrientation(SKRect rect, StackOrientation orientation)
        {
            switch (orientation)
            {
                case StackOrientation.Vertical:
                    return new SKPoint(rect.MidX, rect.Bottom);
                case StackOrientation.Horizontal:
                    return new SKPoint(rect.Right, rect.MidY);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
    }
}
