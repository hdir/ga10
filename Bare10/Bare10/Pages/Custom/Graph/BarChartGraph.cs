using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bare10.Models.Walking;
using Bare10.Resources;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.Graph
{
    public class BarChartGraph : ContentView
    {
        private readonly BarChartCanvasView _chartCanvasView = new BarChartCanvasView()
        {
            Margin = new Thickness(0, Sizes.TextMicro, 0, 0),
        };
        private readonly StackLayout _yLabels = new StackLayout();
        private readonly StackLayout _xLabels = new StackLayout()
        {
            Orientation = StackOrientation.Horizontal,
            Padding = new Thickness(2, 0),
        };

        public int yMin { get; set; } = 0;
        public int yMax { get; set; } = 1;

        public BarChartGraph()
        {
            Content = new Grid()
            {
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() {Height = GridLength.Star},
                    new RowDefinition() {Height = GridLength.Auto},
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Star},
                },

                Children =
                {
                    {_yLabels, 0, 0},
                    {_chartCanvasView, 1, 0},
                    {_xLabels, 1, 1},
                }
            };
        }

        #region Bindable Properties

        public static readonly BindableProperty ItemSourceProperty =
            BindableProperty.Create(
                nameof(ItemSource),
                typeof(ICollection<WalkingDayModel>),
                typeof(BarChartGraph),
                default(ICollection),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((BarChartGraph)bindableObject).ItemSourcePropertyChanged((ICollection)oldValue, (ICollection)newValue);
                }
            );

        private void ItemSourcePropertyChanged(ICollection oldValue, ICollection newValue)
        {
            if (oldValue is INotifyPropertyChanged old)
                old.PropertyChanged -= ItemSourceCollectionChanged;
            if (newValue is INotifyPropertyChanged @new)
                @new.PropertyChanged += ItemSourceCollectionChanged;
            ItemSourceCollectionChanged();
        }

        private void ItemSourceCollectionChanged(object sender = null, PropertyChangedEventArgs e = null)
        {
            try
            {
                _chartCanvasView.TotalY = yMax - yMin;
                _chartCanvasView.BarValues = ItemSource;
                _chartCanvasView.InvalidateSurface();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
#if DEBUG
                throw;
#endif
            }
        }

        public ICollection<WalkingDayModel> ItemSource
        {
            get => (ICollection<WalkingDayModel>)GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }

        public static readonly BindableProperty VerticalLabelsProperty =
            BindableProperty.Create(
                nameof(VerticalLabels),
                typeof(IEnumerable<View>),
                typeof(BarChartGraph),
                default(IEnumerable<View>),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((BarChartGraph)bindableObject).VerticalLabelsPropertyChanged((IEnumerable<View>)oldValue, (IEnumerable<View>)newValue);
                }
            );

        private void VerticalLabelsPropertyChanged(IEnumerable<View> oldValue, IEnumerable<View> newValue)
        {
            _yLabels.Children.Clear();

            foreach (var view in newValue)
                _yLabels.Children.Add(view);

            _chartCanvasView.HorizontalStripes = (int)newValue?.Count();

            InvalidateLayout();
        }

        public IEnumerable<View> VerticalLabels
        {
            get => (IEnumerable<View>)GetValue(VerticalLabelsProperty);
            set => SetValue(VerticalLabelsProperty, value);
        }

        public static readonly BindableProperty HorizontalLabelsProperty =
            BindableProperty.Create(
                nameof(HorizontalLabels),
                typeof(IEnumerable<View>),
                typeof(BarChartGraph),
                default(IEnumerable<View>),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((BarChartGraph)bindableObject).HorizontalLabelsPropertyChanged((IEnumerable<View>)oldValue, (IEnumerable<View>)newValue);
                }
            );

        private void HorizontalLabelsPropertyChanged(IEnumerable<View> oldValue, IEnumerable<View> newValue)
        {
            _xLabels.Children.Clear();

            foreach (var view in newValue)
                _xLabels.Children.Add(view);

            InvalidateLayout();
        }

        public IEnumerable<View> HorizontalLabels
        {
            get => (IEnumerable<View>)GetValue(HorizontalLabelsProperty);
            set => SetValue(HorizontalLabelsProperty, value);
        }

        public bool ShowFirstAndLastLines
        {
            get => _chartCanvasView.ShowFirstAndLastLines;
            set
            {
                _chartCanvasView.ShowFirstAndLastLines = value;
                _xLabels.Margin = new Thickness(0, ShowFirstAndLastLines ? 0 : -15, 0, 0);
            }
        }

        #endregion
    }
}
