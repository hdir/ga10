using System.Collections.Generic;
using Bare10.Models.Walking;
using Bare10.Resources;
using CarouselView.FormsPlugin.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Bare10.Pages.Custom.Graph
{
    public class BarChartCanvasView : SKCanvasView
    {
        private float _yMargin;

        public ICollection<WalkingDayModel> BarValues { get; set; }
        public float TotalY = 1;

        public int HorizontalStripes { get; set; } = 1;
        public bool ShowFirstAndLastLines { get; set; } = false;

        #region Paints

        private readonly SKPaint _backgroundPaint = new SKPaint()
        {
            Color = Colors.BarChartBackground.ToSKColor(),
            StrokeWidth = 2,
            IsAntialias = true,
            IsStroke = true,
        };

        private readonly SKPaint _foregroundPaint = new SKPaint()
        {
            StrokeCap = SKStrokeCap.Round,
            Color = Colors.BarChartForeground.ToSKColor(),
            IsAntialias = true,
            IsStroke = true,
        };

        #endregion

        public BarChartCanvasView()
        {
            //IgnorePixelScaling = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            _yMargin = info.Height / 10f;

            var backgroundRect = new SKRect(
                0,
                0,
                info.Width,
                info.Height - _yMargin
            );
            var frontgroundRect = new SKRect(
                backgroundRect.Left,
                backgroundRect.Top,
                backgroundRect.Right,
                backgroundRect.Bottom
            );

            DrawBackground(backgroundRect, canvas);
            DrawValues(frontgroundRect, canvas);
        }

        private void DrawBackground(SKRect rect, SKCanvas canvas)
        {
            var delta = ((rect.Height - 2f) / (HorizontalStripes - 1));
            // draw stripes 
            for (var i = 0; i < HorizontalStripes; i++)
            {
                var yPos = i * delta + 1;
                canvas.DrawLine(new SKPoint(rect.Left, yPos), new SKPoint(rect.Right, yPos), _backgroundPaint);
            }
        }

        private void DrawValues(SKRect rect, SKCanvas canvas)
        {
            if (BarValues != null && BarValues.Count <= 0)
                return;

            var delta = rect.Width / (BarValues.Count);

            _foregroundPaint.StrokeWidth = delta / 1.5f;
            var xStart = delta / 2f;

            // draw start and end line
            if (ShowFirstAndLastLines)
            {
                var yTop = rect.Height + 1;
                var yBottom = rect.Height + _yMargin;

                var LeftTop = new SKPoint(xStart, yTop);
                var LeftBottom = new SKPoint(xStart, yBottom);

                var RightTop = new SKPoint(rect.Right - xStart, yTop);
                var RightBottom = new SKPoint(rect.Right - xStart, yBottom);

                canvas.DrawLine(LeftTop, LeftBottom, _backgroundPaint);
                canvas.DrawLine(RightTop, RightBottom, _backgroundPaint);
            }

            canvas.ClipRect(rect);

            // draw bars
            for (var i = 0; i < BarValues.Count; i++)
            {
                if (BarValues.GetItem(i) is WalkingDayModel data)
                {
                    var xPos = xStart + i * delta;

                    var normalizedValue = data.MinutesBriskWalking / TotalY;

                    var yTop = rect.Height - (rect.Height * normalizedValue) + (_foregroundPaint.StrokeWidth / 2f);
                    var yBottom = rect.Height + _foregroundPaint.StrokeWidth / 2f;

                    var bottom = new SKPoint(xPos, yBottom);
                    var top = new SKPoint(xPos, yTop);

                    canvas.DrawLine(bottom, top, _foregroundPaint);
                }
            }
        }
    }
}
