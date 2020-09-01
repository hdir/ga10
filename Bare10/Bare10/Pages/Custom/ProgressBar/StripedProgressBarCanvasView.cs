using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Bare10.Pages.Custom.ProgressBar
{
    public class StripedProgressBarCanvasView : ProgressBarCanvasViewBase
    {
        public StripedProgressBarCanvasView()
        {
            IgnorePixelScaling = false;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var canvas = args.Surface.Canvas;

            canvas.Clear();

            var lineSize = (info.Width/TotalProgress) * 0.75f;
            var lineSizeHalf = lineSize * 0.5f;

            BackgroundPaint.StrokeWidth = lineSize;
            ForegroundPaint.StrokeWidth = lineSize;
            ForegroundPaint.StrokeCap = SKStrokeCap.Round;

            var rect = new SKRect(lineSize, 0, info.Width - lineSize, info.Height);

            
            var textRect = new SKRect();
            TextPaint.MeasureText("0", ref textRect);

            var verticalSpace = rect.Height * 0.1f;

            var bottom = rect.Bottom - lineSizeHalf - (textRect.Height + verticalSpace);
            var top = rect.Top + lineSizeHalf + (textRect.Height + verticalSpace);

            var regularHeight = (bottom - top) * 0.65f;

            var xPosInterval = rect.Width / (TotalProgress - 1);

            for (var i = 0; i < TotalProgress; i++)
            {
                var xPos = rect.Left + xPosInterval * i;

                // Draw Labels
                if (i == 0)
                {
                    // Start Label
                    const string progressText = "0";
                    canvas.DrawText(progressText,
                        XPosInsideBounds(xPos, textRect, info),
                        rect.Bottom - 2,
                        TextPaint);
                }
                else if (i == TotalProgress - 1)
                {
                    // End Label
                    var progressText = TotalProgress.ToString();
                    TextPaint.MeasureText(progressText, ref textRect);
                    canvas.DrawText(progressText,
                        XPosInsideBounds(xPos, textRect, info),
                        rect.Bottom - 2,
                        TextPaint);
                }

                if (i + 1 < Progress)
                {
                    // Draw Foreground
                    canvas.DrawLine(xPos, bottom - regularHeight, xPos, bottom , ForegroundPaint);
                }
                else if (i + 1 == Progress)
                {
                    // Draw Current Progress
                    canvas.DrawLine(xPos, top, xPos, bottom , ForegroundPaint);

                    TextPaint.Color = TextPaint.Color.WithAlpha(255);
                    var progressText = Progress.ToString();
                    TextPaint.MeasureText(progressText, ref textRect);
                    canvas.DrawText(progressText, 
                        XPosInsideBounds(xPos, textRect, info),
                        rect.Top + textRect.Height,
                        ActiveTextPaint);
                }
                else
                {
                    // Draw Background
                    canvas.DrawLine(xPos, bottom - regularHeight, xPos, bottom, BackgroundPaint);
                }
            }
        }
    }
}
