using System;
using System.Globalization;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.ProgressBar
{
    public class FinishLineProgressBarCanvasView : ProgressBarCanvasView
    {
        private readonly SKPath FLAG_PATH = SKPath.ParseSvgPathData(
            "M10.146 7.961c-.573-.74-1.822-1.748-4.15-1.144-1.154.3-2.073.271-2.656-.083a1.304 1.304 0 0 1-.56-.649l1.24-4.324c.519.528 1.61 1.13 3.77.679 2.516-.528 3.432.685 3.641 1.04l-1.285 4.481zm1.922-4.642c-.04-.091-1.002-2.22-4.41-1.506-2.483.521-3.25-.5-3.42-.812l.13-.458-.614-.177-.165.575-1.45 5.057-1.65 5.754.615.176L2.546 6.9c.114.127.257.255.44.368.187.117.395.208.623.273.678.195 1.53.16 2.547-.104 2.91-.755 3.764 1.338 3.798 1.426a.32.32 0 0 0 .606-.025l1.521-5.304a.323.323 0 0 0-.013-.215z"
        );

        #region Paint

        protected SKPaint FinishCirclePaint { get; } = new SKPaint
        {
            Color = Color.Gray.ToSKColor(),
            IsAntialias = true,
            IsStroke = true,
            StrokeCap = SKStrokeCap.Round,
        };

        protected SKPaint FlagPaint { get; } = new SKPaint
        {
            Color = SKColors.White,
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

            var lineSize = info.Height * 0.25f;  //25%
            var lineSizeHalf = lineSize * 0.5f;

            BackgroundPaint.StrokeWidth = lineSize;
            ForegroundPaint.StrokeWidth = lineSize;
            ProgressCirclePaint.StrokeWidth = lineSize;
            FinishCirclePaint.StrokeWidth = lineSize * 2;

            var rect = new SKRect(lineSizeHalf, 0, info.Width - lineSize, info.Height);

            var progress = Math.Min(Math.Max(0, Progress), TotalProgress) / TotalProgress;
            var progressWidth = rect.Left + (rect.Width * progress);

            // Draw Background
            canvas.DrawLine(rect.Left, rect.MidY, rect.Right, rect.MidY, BackgroundPaint);

            // Draw Foreground
            ForegroundPaint.StrokeWidth = lineSize;

            canvas.DrawLine(rect.Left, rect.MidY, progressWidth, rect.MidY, ForegroundPaint);

            // Draw current progress line
            if (progress > 0 && progress < 1)
            {
                // Draw Top Progress Text
                var progressText = Progress.ToString(CultureInfo.InvariantCulture);
                var textRect = new SKRect();
                TextPaint.MeasureText(progressText, ref textRect);
                canvas.DrawText(progressText,
                    progressWidth - 1 - textRect.MidX,
                    rect.Top + textRect.Height,
                    ActiveTextPaint);

                // Draw Progress Line
                ForegroundPaint.StrokeWidth = 1;
                canvas.DrawLine(progressWidth - 1, rect.Top + textRect.Height + lineSize*0.3f, progressWidth - 1, rect.MidY + lineSizeHalf, ForegroundPaint);
            }

            // Draw end and start circle
            if (progress > 0)
                canvas.DrawLine(rect.Left, rect.MidY, rect.Left, rect.MidY, ProgressCirclePaint);
            if (progress >= 1)
                canvas.DrawLine(rect.Right, rect.MidY, rect.Right, rect.MidY, ProgressCirclePaint);

            // Draw Finish Circle
            FinishCirclePaint.Color = progress >= 1 ? ProgressColor.ToSKColor() : ProgressBackgroundColor.ToSKColor();
            var end = rect.Right;
            canvas.DrawLine(end, rect.MidY, end, rect.MidY, FinishCirclePaint);

            // Draw Labels
            if (LabelCount > 1)
            {
                var width = rect.Width;
                var interval = width / (LabelCount - 1);
                for (var i = 0; i < LabelCount; i++)
                {
                    var xPos = rect.Left + interval * i;

                    var text = (i * (TotalProgress/(LabelCount-1))).ToString(CultureInfo.InvariantCulture);
                    var textRect = new SKRect();
                    TextPaint.MeasureText(text, ref textRect);
                    canvas.DrawText(text,
                        XPosInsideBounds(xPos, textRect, info),
                        rect.Bottom - 2,
                        TextPaint);
                }
            }

            // Draw Finish Flag
            canvas.Save();
            FlagPaint.Color = TextPaint.Color.WithAlpha(150);
            FLAG_PATH.GetTightBounds(out var bounds);
            canvas.Translate(end, rect.MidY);
            var size = lineSize * 0.75f;
            var scale = new SKPoint(size / (bounds.Width + FlagPaint.StrokeWidth),
                size / (bounds.Height + FlagPaint.StrokeWidth));
            canvas.Scale(scale);
            canvas.Translate(-bounds.MidX, -bounds.MidY);
            canvas.DrawPath(FLAG_PATH, FlagPaint);
            canvas.Restore();
        }

        public static readonly BindableProperty LabelCountProperty =
            BindableProperty.Create(
                nameof(LabelCount),
                typeof(int),
                typeof(FinishLineProgressBarCanvasView),
                default(int),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((FinishLineProgressBarCanvasView) bindableObject).InvalidateSurface();
                }
            );

        public int LabelCount
        {
            get => (int) GetValue(LabelCountProperty);
            set => SetValue(LabelCountProperty, value);
        }
    }
}