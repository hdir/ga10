using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.ProgressBar
{
    public class ProgressBarCanvasView : ProgressBarCanvasViewBase
    {
        #region Paint

        protected SKPaint ProgressCirclePaint { get; } = new SKPaint
        {
            Color = Color.CadetBlue.ToSKColor(),
            IsAntialias = true,
            IsStroke = true,
            StrokeCap = SKStrokeCap.Round,
        };

        #endregion

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            base.OnPaintSurface(args);

            var info = args.Info;
            var canvas = args.Surface.Canvas;

            canvas.Clear();

            var lineSize = info.Height;
            var lineSizeHalf = lineSize * 0.5f;

            BackgroundPaint.StrokeWidth = lineSize;
            ForegroundPaint.StrokeWidth = lineSize;
            ProgressCirclePaint.StrokeWidth = lineSize;

            var rect = new SKRect(lineSizeHalf, 0, info.Width - lineSizeHalf, info.Height);

            var progress = Math.Min(Math.Max(0, Progress), TotalProgress) / TotalProgress;

            // Draw Background
            canvas.DrawLine(rect.Left, rect.MidY, rect.Right, rect.MidY, BackgroundPaint);

            // Draw Foreground
            canvas.DrawLine(rect.Left, rect.MidY, rect.Left + (rect.Width * progress), rect.MidY, ForegroundPaint);

            // Draw end and start circle
            if (progress > 0)
                canvas.DrawLine(rect.Left, rect.MidY, rect.Left, rect.MidY, ProgressCirclePaint);
            if (progress >= 1)
                canvas.DrawLine(rect.Right, rect.MidY, rect.Right, rect.MidY, ProgressCirclePaint);
        }

        protected override void ColorPropertyChanged(Color newValue)
        {
            base.ColorPropertyChanged(newValue);
            ProgressCirclePaint.Color = newValue.ToSKColor();
            InvalidateSurface();
        }
    }
}
