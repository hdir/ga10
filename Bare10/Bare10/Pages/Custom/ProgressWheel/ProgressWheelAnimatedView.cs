using System;
using System.Threading.Tasks;
using Bare10.Resources;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.ProgressWheel
{
    public class ProgressWheelAnimatedView : SKCanvasView
    {
        public event Action OnAnimationFinished;

        public float CurrentProgress { get; private set; } = 0;
        public float Radius { get; private set; } = 0;

        public uint MillisecondsToCompleteWheel { get; set; } = 4000;

        public bool IsComplete => CurrentProgress > 0 && CurrentProgress >= TotalProgress;

        #region Paints 

        private SKPaint _backgroundPaint;
        private SKPaint _foregroundPaint;
        private SKPaint _pointPaint;

        private void MakePaints(float strokeSize)
        {
            if (_backgroundPaint == null)
            {
                _backgroundPaint = new SKPaint()
                {
                    Style = SKPaintStyle.Stroke,
                    Color = Colors.ProgressWheelBackground.ToSKColor(),
                    IsAntialias = true,
                    StrokeWidth = strokeSize * 0.5f,
                    StrokeCap = SKStrokeCap.Round,
                };

                _foregroundPaint = new SKPaint()
                {
                    Style = SKPaintStyle.Stroke,
                    Color = Colors.ProgressWheelForeground.ToSKColor(),
                    IsAntialias = true,
                    StrokeWidth = strokeSize,
                    StrokeCap = SKStrokeCap.Round,
                };

                _pointPaint = new SKPaint()
                {
                    Style = SKPaintStyle.Fill,
                    Color = Colors.ProgressWheelForeground.ToSKColor().WithAlpha(100),
                    StrokeWidth = strokeSize,
                    IsAntialias = true,
                };
            }
        }
        #endregion

        public ProgressWheelAnimatedView()
        {
            Init();
            var blinking = new Task(StartContinuousBlinkingAnimation);
            blinking.Start();
        }

        public void Init()
        {
            InvalidateSurface();
            AnimateWheel();
        }

        public void SetInitialProgress(float initialProgress)
        {
            if (initialProgress < CurrentProgress)
            {
                // it's a new day, we reset first
                CurrentProgress = 0;
                InvalidateSurface();
            }
            CurrentProgress = initialProgress;
            Init();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            var strokeSize = Math.Min(info.Width, info.Height) * 0.075f;
            var size = Math.Min(info.Width, info.Height) - strokeSize * 2;

            if (strokeSize > 0)
                MakePaints(strokeSize);

            canvas.Clear();

            var x = (info.Width - size) / 2f;
            var y = (info.Height - size) / 2f;
            var drawArea = new SKRect(x, y, x + size, y + size);

            DrawBackground(drawArea, canvas);
            DrawLastPoint(drawArea, canvas);
            DrawForeground(drawArea, canvas);
        }

        #region Drawing

        private static SKPath Segment(SKRect rect, int index, uint segmentCount, float progress = 1f)
        {
            var spacing = segmentCount > 1 ? 16 : 0;
            var path = new SKPath();
            var distance = (360f / segmentCount) - spacing;
            var start = spacing / 2f + index * (spacing + distance);

            path.AddArc(rect, -90 + start, distance * progress);
            return path;
        }

        private void DrawBackground(SKRect rect, SKCanvas canvas)
        {
            for (var i = 0; i < SegmentCount; i++)
            {
                using (var path = Segment(rect, i, SegmentCount))
                {
                    canvas.DrawPath(path, _backgroundPaint);
                }
            }
        }

        private void DrawForeground(SKRect rect, SKCanvas canvas)
        {
            for (var i = 0; i < SegmentCount; i++)
            {
                var segmentWidth = TotalProgress / SegmentCount;
                var progress = Math.Max(0, Math.Min((CurrentProgress - (i * segmentWidth)) / segmentWidth, 1f));

                using (var path = Segment(rect, i, SegmentCount, progress))
                {
                    if (!path.IsEmpty)
                        canvas.DrawPath(path, _foregroundPaint);
                }
            }
        }

        private void DrawLastPoint(SKRect rect, SKCanvas canvas)
        {
            var segmentWidth = TotalProgress / SegmentCount;
            var i = (int) (CurrentProgress / segmentWidth);
            var progress = Math.Max(0.001f, Math.Min((CurrentProgress - (i * segmentWidth)) / segmentWidth, 1f));
            var currentSegment = Segment(rect, i, SegmentCount, progress);

            if (currentSegment.IsEmpty)
                return;

            var stroke = _pointPaint.StrokeWidth / 2f;
            var radius = stroke + Radius * stroke;
            _pointPaint.Color = _pointPaint.Color.WithAlpha((byte)(255 * (1 - Radius)));

            canvas.DrawCircle(currentSegment.LastPoint, radius, _pointPaint);
        }

        #endregion

        #region Animation

        private bool _isAnimating = false;

        private async void AnimateWheel()
        {
            // cancel previous animation
            if (_isAnimating)
                CancelProgressToAnimation();

            // when wheel resets
            if (TargetProgress < CurrentProgress)
                CurrentProgress = TargetProgress;

            // find starting segment
            var segmentWidth = (TotalProgress / SegmentCount);
            var startSegment = (uint)(CurrentProgress / segmentWidth);
            var deltaTime = (MillisecondsToCompleteWheel / SegmentCount);

            for (var i = startSegment; i < SegmentCount; i++)
            {
                _isAnimating = true;
                var segmentStart = i * segmentWidth;
                var segmentEnd = segmentStart + segmentWidth;
                var segmentTarget = Math.Min(TargetProgress, segmentEnd);

                await ProgressTo(
                    CurrentProgress,
                    segmentTarget,
                    value =>
                    {
                        CurrentProgress = float.IsNaN(value) ? CurrentProgress : value;
                        InvalidateSurface();
                    },
                    (uint) (deltaTime * ((segmentStart + segmentTarget) / segmentEnd)),
                    Easing.SinInOut
                );
                _isAnimating = false;
            }
            OnAnimationFinished?.Invoke();
        }

        private async void StartContinuousBlinkingAnimation()
        {
            // wait for initialized
            await Task.Delay(500);

            // only run if complete
            while (!IsComplete)
            {
                await RadiusTo(0, 1, value =>
                {
                    Radius = value;
                    InvalidateSurface();
                }, 1000, Easing.CubicOut);
            }
        }

        public Task<bool> ProgressTo(float fromValue, float toValue, Action<float> callback, uint length = 250, Easing easing = null)
        {
            float Transform(double t) => (float) (fromValue + t * (toValue-fromValue));
            return ValueAnimation(this, nameof(ProgressTo), Transform, callback, length, easing);
        }

        public void CancelProgressToAnimation()
        {
            this.AbortAnimation(nameof(ProgressTo));
        }

        public Task<bool> RadiusTo(float fromValue, float toValue, Action<float> callback, uint length = 250, Easing easing = null)
        {
            float Transform(double t) => (float) (fromValue + t * (toValue - fromValue));
            return ValueAnimation(this, nameof(RadiusTo), Transform, callback, length, easing);
        }

        public void CancelRadiusToAnimation()
        {
            this.AbortAnimation(nameof(RadiusTo));
        }

        private Task<bool> ValueAnimation(IAnimatable element, string name, Func<double, float> transform, Action<float> callback, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty TotalProgressProperty =
            BindableProperty.Create(
                nameof(TotalProgress),
                typeof(float),
                typeof(ProgressWheelAnimatedView),
                default(float),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressWheelAnimatedView)bindableObject).Init();
                }
            );

        public float TotalProgress
        {
            get => (float)GetValue(TotalProgressProperty);
            set => SetValue(TotalProgressProperty, value);
        }

        public static readonly BindableProperty TargetProgressProperty =
            BindableProperty.Create(
                nameof(TargetProgress),
                typeof(float),
                typeof(ProgressWheelAnimatedView),
                default(float),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressWheelAnimatedView)bindableObject).Init();
                }
            );

        public float TargetProgress
        {
            get => (float)GetValue(TargetProgressProperty);
            set => SetValue(TargetProgressProperty, value);
        }

        public static readonly BindableProperty SegmentCountProperty =
            BindableProperty.Create(
                nameof(SegmentCount),
                typeof(uint),
                typeof(ProgressWheelAnimatedView),
                1u,
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressWheelAnimatedView)bindableObject).Init();
                }
            );

        public uint SegmentCount
        {
            get => (uint)GetValue(SegmentCountProperty);
            set => SetValue(SegmentCountProperty, value);
        }

        #endregion
    }
}
