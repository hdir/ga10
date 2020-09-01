using System;
using System.Globalization;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Bare10.Pages.Custom.ProgressBar
{
    public class SegmentProgressBarCanvasView : ProgressBarCanvasView
    {
        private readonly SKPath CHECKMARK_PATH = SKPath.ParseSvgPathData(
            "M10.445 15.069l-4.008-4.23a.586.586 0 1 1 .852-.806l3.064 3.233 4.47-5.812a.586.586 0 1 1 .93.715l-5.308 6.9z"
        );

        #region Paint

        protected SKPaint CheckmarkPaint { get; } = new SKPaint
        {
            Color = SKColors.Black,
            StrokeWidth = 1f,
            IsAntialias = true,
        };

        #endregion

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            base.OnPaintSurface(args);

            var info = args.Info;
            var canvas = args.Surface.Canvas;

            canvas.Clear();

            var lineSize = info.Height * 0.12f; //25%
            var segmentCircleSize = lineSize * 4;

            BackgroundPaint.StrokeWidth = lineSize;
            ForegroundPaint.StrokeWidth = lineSize;
            ProgressCirclePaint.StrokeWidth = lineSize;

            var rect = new SKRect(segmentCircleSize / 2, 0, info.Width - segmentCircleSize / 2, info.Height);

            var progress = Math.Min(Math.Max(0, Progress-1), (TotalProgress - 1)) / (TotalProgress - 1);
            var progressWidth = rect.Left + (rect.Width * progress);

            // Draw Background
            canvas.DrawLine(rect.Left, rect.MidY, rect.Right, rect.MidY, BackgroundPaint);

            // Draw Foreground
            ForegroundPaint.StrokeWidth = lineSize;
            canvas.DrawLine(rect.Left, rect.MidY, progressWidth, rect.MidY, ForegroundPaint);

            // Draw end and start circle
            if (progress > 0)
                canvas.DrawLine(rect.Left, rect.MidY, rect.Left, rect.MidY, ProgressCirclePaint);
            if (progress >= 1)
                canvas.DrawLine(rect.Right, rect.MidY, rect.Right, rect.MidY, ProgressCirclePaint);

            var width = rect.Width;
            var interval = width / (TotalProgress-1);
            for (var i = 0; i < TotalProgress; i++)
            {
                var xPos = rect.Left + interval * i;

                // Draw Circles
                ProgressCirclePaint.StrokeWidth = segmentCircleSize;
                BackgroundPaint.StrokeWidth = segmentCircleSize * 0.9f;
                canvas.DrawLine(xPos, rect.MidY, xPos, rect.MidY, Progress > i ? ProgressCirclePaint : BackgroundPaint);

                // Draw Checkmark
                if (Progress > i)
                {
                    canvas.Save();
                    CHECKMARK_PATH.GetTightBounds(out var bounds);
                    canvas.Translate(xPos, rect.MidY);
                    var size = lineSize * 1.5f;
                    var scale = new SKPoint(size / (bounds.Width + CheckmarkPaint.StrokeWidth),
                        size / (bounds.Height + CheckmarkPaint.StrokeWidth));
                    canvas.Scale(scale);
                    canvas.Translate(-bounds.MidX, -bounds.MidY);
                    canvas.DrawPath(CHECKMARK_PATH, CheckmarkPaint);
                    canvas.Restore();
                }

                // Draw Labels
                var text = (i+1).ToString(CultureInfo.InvariantCulture);
                var textRect = new SKRect();
                TextPaint.MeasureText(text, ref textRect);
                canvas.DrawText(text,
                    XPosInsideBounds(xPos, textRect, info),
                    rect.Bottom - 2,
                    TextPaint);
            }
        }
    }
}
