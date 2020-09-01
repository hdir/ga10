using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bare10.Resources;
using Bare10.Utils.Converters;
using Bare10.ViewModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.Graph
{
    public class HourlyAreaGraph : ContentView
    {
        private const int HourSegmentWidth = 200;

        private void ReCreate()
        {
            var hours = new List<WalkingDataPointsViewModel>();

            foreach (var item in ItemSource)
            {
                if (item is WalkingDataPointsViewModel vm)
                    hours.Add(vm);
            }

            if (ItemSource.Count > 0)
            {
                var view = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 2f,
                };

                for (var i = 0; i < hours.Count; i++)
                {
                    view.Children.Add(CreateHourView(hours[i]));

                    if (i < hours.Count - 1)
                    {
                        // amount of hours in difference, if greater than 1 hour difference show 3 stripes instead of 1
                        var diff = hours[i + 1].Hour - hours[i].Hour;
                        view.Children.Add(Separator(diff > 1 ? 3 : 1));
                    }
                }

                Content = view;
            }
        }

        private View CreateHourView(object bindingContext)
        {
            var view = new HourSegmentCanvasView()
            {
                WidthRequest = HourSegmentWidth,
            };
            view.SetBinding(HourSegmentCanvasView.WalkingDataPointsProperty, nameof(WalkingDataPointsViewModel.WalkingDataPoints));

            
            var lbl = new Label()
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                FontSize = Sizes.TextSmall,
            };
            lbl.SetBinding(Label.TextProperty, nameof(WalkingDataPointsViewModel.HourText));

            var layout = new Grid
            {
                RowSpacing = 2f,
                Margin = new Thickness(3f, 0),
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){Height = GridLength.Star},
                    new RowDefinition(){Height = GridLength.Auto},
                },
                Children =
                {
                    {view, 0, 0}, 
                    {lbl, 0, 1}, 
                },
                BindingContext = bindingContext,
            };
            layout.SetBinding(IsVisibleProperty, nameof(WalkingDataPointsViewModel.WalkingDataPoints),
                converter: new ListIsEmptyToBooleanConverter(true));

            return layout;
        }

        private static View Separator(int amount = 1)
        {
            return new Grid
            {
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){Height = GridLength.Star},
                    new RowDefinition(){Height = GridLength.Auto},
                },
                Children =
                {
                    {new Label()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 10),
                        FontSize = Sizes.TextMicro,
                        Text = string.Concat(Enumerable.Repeat(" - ", amount)), //" - ";
                        TextColor = Colors.GraphFadedLine,
                    }, 0, 0},
                    {new Label()
                    {
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.Start,
                        FontSize = Sizes.TextSmall,
                        Margin = 0,
                        Opacity = 0f,
                    }, 0, 1},
                },
            };
        }

        #region Bindable Properties

        public static readonly BindableProperty ItemSourceProperty =
            BindableProperty.Create(
                nameof(ItemSource),
                typeof(ICollection),
                typeof(HourlyAreaGraph),
                default(ICollection),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((HourlyAreaGraph) bindableObject).ItemSourcePropertyChanged((ICollection) oldValue, (ICollection) newValue);
                }
            );

        private void ItemSourcePropertyChanged(ICollection oldValue, ICollection newValue)
        {
            if (oldValue is INotifyPropertyChanged old)
                old.PropertyChanged -= CollectionChanged;
            if (newValue is INotifyPropertyChanged @new)
                @new.PropertyChanged += CollectionChanged;
            CollectionChanged();
        }

        private void CollectionChanged(object sender = null, PropertyChangedEventArgs e = null)
        {
            try
            {
                ReCreate();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public ICollection ItemSource
        {
            get => (ICollection) GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }

        #endregion

    }
}
