using System;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom
{
    public class OrganicBubblesCanvasView : SKCanvasView
    {
        public float DeltaMovement { get; set; } = 10f;
        public uint RotationTime { get; set; } = 5000u;

        private readonly SKPaint _backgroundPaint = new SKPaint()
        {
            Color = SKColors.White,
            IsAntialias = true,
        };

        public OrganicBubblesCanvasView()
        {
            IgnorePixelScaling = true;

            Init();
        }

        private float _seed;

        private async void Init()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await ValueTo(0f, (float) (2f * Math.PI), (v) =>
                    {
                        _seed = v;
                        InvalidateSurface();
                    }, RotationTime);
                }
            });
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            // translate to center
            var rect = new SKRect(3 * DeltaMovement, 3 * DeltaMovement, info.Width - 3 * DeltaMovement, info.Height - 3 * DeltaMovement);

            DrawOrganicInside(rect, canvas);
        }

        private SKPoint DeltaPoint(float shift) => new SKPoint((float)Math.Cos(_seed + shift * 2 * Math.PI) * DeltaMovement, (float)Math.Sin(_seed + shift * 2 * Math.PI) * DeltaMovement);

        private void DrawOrganicInside(SKRect rect, SKCanvas canvas)
        {
            var quarterWidth = rect.Width / 4f;

            var cMultiplier = new SKPoint(quarterWidth * 0.5f, 0);

            var p0 = new SKPoint(rect.Left, rect.MidY);
            var p1 = new SKPoint(rect.Left + quarterWidth, rect.Top) + DeltaPoint(0);
            var p2 = new SKPoint(rect.Right - quarterWidth, rect.Top) + DeltaPoint(.5f);
            var p3 = new SKPoint(rect.Right, rect.MidY) + DeltaPoint(.9f);
            var p4 = new SKPoint(rect.Right - quarterWidth, rect.Bottom) + DeltaPoint(.65f);
            var p5 = new SKPoint(rect.Left + quarterWidth, rect.Bottom) + DeltaPoint(.4f);

            var qp1 = new SKPoint(rect.Left, rect.Top);
            var qp2 = new SKPoint(rect.Right, rect.Top);
            var qp3 = new SKPoint(rect.Right, rect.Bottom);
            var qp4 = new SKPoint(rect.Left, rect.Bottom);

            var m1 = new SKPoint(rect.MidX, rect.Top) + Multiply(DeltaPoint(.25f), new SKPoint(-1.5f, 2.4f));
            var m2 = new SKPoint(rect.MidX, rect.Bottom) + Multiply(DeltaPoint(.65f), new SKPoint(0.5f, 1.8f));

            var cp1 = p1 + cMultiplier;
            var cm1 = m1 - cMultiplier;

            var cp2 = p2 - cMultiplier;
            var cm2 = m1 + cMultiplier;

            var cp3 = p4 - cMultiplier;
            var cm3 = m2 + cMultiplier;

            var cp4 = p5 + cMultiplier;
            var cm4 = m2 - cMultiplier;

            // Draw path with quadratic Bezier
            using (var path = new SKPath())
            {
                path.MoveTo(p0);
                path.QuadTo(qp1, p1);
                path.CubicTo(cp1, cm1, m1);
                path.CubicTo(cm2, cp2, p2);
                path.QuadTo(qp2, p3);
                path.QuadTo(qp3, p4);
                path.CubicTo(cp3, cm3, m2);
                path.CubicTo(cm4, cp4, p5);
                path.QuadTo(qp4, p0);

                canvas.DrawPath(path, _backgroundPaint);
            }
        }

        private SKPoint Multiply(SKPoint p1, SKPoint p2) => new SKPoint(p1.X * p2.X, p1.Y * p2.Y);

        public Task<bool> ValueTo(float fromValue, float toValue, Action<float> callback, uint length = 250, Easing easing = null)
        {
            float Transform(double t) => (float)(fromValue + t * (toValue - fromValue));
            return ValueAnimation(this, nameof(ValueTo), Transform, callback, length, easing);
        }

        protected Task<bool> ValueAnimation(IAnimatable element, string name, Func<double, float> transform, Action<float> callback, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(
                nameof(Color),
                typeof(Color),
                typeof(OrganicBubblesCanvasView),
                default(Color),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((OrganicBubblesCanvasView) bindableObject).ColorPropertyChanged((Color) oldValue, (Color) newValue);
                }
            );

        private void ColorPropertyChanged(Color oldValue, Color newValue)
        {
            _backgroundPaint.Color = newValue.ToSKColor();
        }

        public Color Color
        {
            get => (Color) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
    }
}
