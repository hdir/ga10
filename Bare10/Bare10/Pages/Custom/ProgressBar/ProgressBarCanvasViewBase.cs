using System;
using Bare10.Resources;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.ProgressBar
{
    public abstract class ProgressBarCanvasViewBase : SKCanvasView
    {
        #region Paints
        protected SKPaint ForegroundPaint { get; } = new SKPaint
        {
            Color = Color.CadetBlue.ToSKColor(),
            IsAntialias = true,
        };

        protected SKPaint BackgroundPaint { get; } = new SKPaint
        {
            Color = Color.Gray.ToSKColor(),
            IsAntialias = true,
            IsStroke = true,
            StrokeCap = SKStrokeCap.Round,
        };

        protected readonly SKPaint ActiveTextPaint = new SKPaint
        {
            Color = Colors.Text.ToSKColor(),
            IsAntialias = true,
            TextSize = (float)Sizes.TextLarge,
        };

        protected readonly SKPaint TextPaint = new SKPaint
        {
            Color = Colors.Text.ToSKColor().WithAlpha(150),
            IsAntialias = true,
            TextSize = (float)Sizes.TextLarge,
        };
        #endregion

        protected ProgressBarCanvasViewBase()
        {
            IgnorePixelScaling = false;
        }

        protected static float XPosInsideBounds(float xPos, SKRect textRect, SKImageInfo info)
        {
            return Math.Max(0, Math.Min(xPos - textRect.MidX, info.Width - textRect.Width - 10));
        }

        #region Bindable properties

        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(
                nameof(Progress),
                typeof(float),
                typeof(ProgressBarCanvasViewBase),
                default(float),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressBarCanvasViewBase)bindableObject).InvalidateSurface();
                }
            );

        public float Progress
        {
            get => (float)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly BindableProperty TotalProgressProperty =
            BindableProperty.Create(
                nameof(TotalProgress),
                typeof(int),
                typeof(ProgressBarCanvasViewBase),
                1,
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressBarCanvasViewBase)bindableObject).InvalidateSurface();
                }
            );

        public int TotalProgress
        {
            get => (int) GetValue(TotalProgressProperty);
            set => SetValue(TotalProgressProperty, value);
        }

        public static readonly BindableProperty ProgressColorProperty =
            BindableProperty.Create(
                nameof(ProgressColor),
                typeof(Color),
                typeof(ProgressBarCanvasViewBase),
                default(Color),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressBarCanvasViewBase) bindableObject).ColorPropertyChanged((Color) newValue);
                }
            );

        protected virtual void ColorPropertyChanged(Color newValue)
        {
            ForegroundPaint.Color = newValue.ToSKColor();
            InvalidateSurface();
        }

        public Color ProgressColor
        {
            get => (Color) GetValue(ProgressColorProperty);
            set => SetValue(ProgressColorProperty, value);
        }

        public static readonly BindableProperty ProgressBackgroundColorProperty =
            BindableProperty.Create(
                nameof(ProgressBackgroundColor),
                typeof(Color),
                typeof(ProgressBarCanvasViewBase),
                default(Color),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressBarCanvasViewBase) bindableObject).ProgressBackgroundColorPropertyChanged((Color) newValue);
                }
            );

        protected virtual void ProgressBackgroundColorPropertyChanged(Color newValue)
        {
            BackgroundPaint.Color = newValue.ToSKColor();
            InvalidateSurface();
        }

        public Color ProgressBackgroundColor
        {
            get => (Color) GetValue(ProgressBackgroundColorProperty);
            set => SetValue(ProgressBackgroundColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(
                nameof(TextColor),
                typeof(Color),
                typeof(ProgressBarCanvasViewBase),
                default(Color),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressBarCanvasViewBase) bindableObject).TextColorPropertyChanged((Color) newValue);
                }
            );

        private void TextColorPropertyChanged(Color newValue)
        {
            ActiveTextPaint.Color = newValue.ToSKColor();
            InvalidateSurface();
        }

        public Color TextColor
        {
            get => (Color) GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty LabelColorProperty =
            BindableProperty.Create(
                nameof(LabelColor),
                typeof(Color),
                typeof(ProgressBarCanvasViewBase),
                default(Color),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressBarCanvasViewBase) bindableObject).LabelColorPropertyChanged((Color) newValue);
                }
            );

        private void LabelColorPropertyChanged(Color newValue)
        {
            TextPaint.Color = newValue.ToSKColor();
            InvalidateSurface();
        }

        public Color LabelColor
        {
            get => (Color) GetValue(LabelColorProperty);
            set => SetValue(LabelColorProperty, value);
        }


        public static readonly BindableProperty TextSizeProperty =
            BindableProperty.Create(
                nameof(TextSize),
                typeof(double),
                typeof(ProgressBarCanvasViewBase),
                default(double),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((ProgressBarCanvasViewBase) bindableObject).TextSizeChanged((double) newValue);
                }
            );

        protected virtual void TextSizeChanged(double newValue)
        {
            TextPaint.TextSize = (float) newValue;
            ActiveTextPaint.TextSize = (float) newValue;
            InvalidateSurface();
        }

        public double TextSize
        {
            get => (double) GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        #endregion
    }
}