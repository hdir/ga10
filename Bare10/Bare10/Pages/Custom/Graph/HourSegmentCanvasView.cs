using System.Collections.Generic;
using Bare10.Models.Walking;
using Bare10.Resources;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.Graph
{
    public class HourSegmentCanvasView : SKCanvasView
    {
        private const int HOUR = 60;
        private const int DIVISIONS = 30;

        private float CornerRadius = 10;

        private Thickness minMargin = new Thickness(1f, 0);

        private SKPaint briskPaint => new SKPaint { Color = Colors.GraphBrisk.ToSKColor() };
        private SKPaint regularPaint => new SKPaint { Color = Colors.GraphNormal.ToSKColor() };
        private SKPaint thinLinePaint => new SKPaint { Color = Colors.GraphFadedLine.ToSKColor() };
        private SKPaint segmentPaint => new SKPaint { Color = Colors.GraphWhiteLine.ToSKColor() };

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            DrawChart(info.Width, info.Height - 30f, canvas);
            DrawLabel(new SKRect(0, info.Height - 20, info.Width-1, info.Height), canvas, segmentPaint);
        }

        private static void DrawLabel(SKRect rect, SKCanvas canvas, SKPaint paint)
        {
            canvas.DrawLine(
                new SKPoint(rect.Left, rect.Top), 
                new SKPoint(rect.Left, rect.Bottom),
                paint
            );
            canvas.DrawLine(
                new SKPoint(rect.Left, rect.Bottom-1),
                new SKPoint(rect.Right, rect.Bottom-1),
                paint
            );
            canvas.DrawLine(
                new SKPoint(rect.Right, rect.Bottom),
                new SKPoint(rect.Right, rect.Top),
                paint
            );
        }

        private void DrawChart(float width, float height, SKCanvas canvas)
        {
            // draw background lines
            for (var i = 0; i < DIVISIONS; i++)
            {
                var x = i * ((width - 1) / (DIVISIONS - 1));
                var yStart = CornerRadius / 2f;
                var yEnd = height - yStart;

                canvas.DrawLine(new SKPoint(x, yStart),
                    new SKPoint(x, yEnd),
                    thinLinePaint);
            }

            // draw values
            foreach (var dataPoint in WalkingDataPoints)
            {
                var rect = WalkingDataRect(dataPoint, width, height);
                canvas.DrawRoundRect(rect, dataPoint.WasBrisk ? briskPaint : regularPaint);
            }
        }

        private SKRoundRect WalkingDataRect(WalkingDataPointModel value, float totalWidth, float totalHeight)
        {
            var d = totalWidth / (HOUR - 1); // divide into minutes
            var xStart = d * value.Start.Minute + (float)minMargin.Left;
            var xEnd = d * value.Stop.Minute - (float)minMargin.Right;

            var rect = new SKRect(xStart, 0, xEnd, totalHeight);
            var roundedRect = new SKRoundRect(rect, CornerRadius, CornerRadius);
            return roundedRect;
        }

        #region Bindable Properties

        public static readonly BindableProperty WalkingDataPointsProperty =
            BindableProperty.Create(
                nameof(WalkingDataPoints),
                typeof(IEnumerable<WalkingDataPointModel>),
                typeof(HourSegmentCanvasView),
                new List<WalkingDataPointModel>(),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((HourSegmentCanvasView) bindableObject).WalkingDataPointsPropertyChanged((IEnumerable<WalkingDataPointModel>) oldValue, (IEnumerable<WalkingDataPointModel>) newValue);
                }
            );

        private void WalkingDataPointsPropertyChanged(IEnumerable<WalkingDataPointModel> oldValue, IEnumerable<WalkingDataPointModel> newValue)
        {
            InvalidateSurface();
        }

        public IEnumerable<WalkingDataPointModel> WalkingDataPoints
        {
            get => (IEnumerable<WalkingDataPointModel>) GetValue(WalkingDataPointsProperty);
            set => SetValue(WalkingDataPointsProperty, value);
        }

        #endregion
    }
}
