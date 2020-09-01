using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.ViewControllers
{

    public class MasterDetailView : RelativeLayout
    {
        private readonly ContentView _master = new ContentView();
        private readonly ContentView _detail = new ContentView();

        #region Properties

        public View Master
        {
            get => _master.Content;
            set => _master.Content = value;
        }

        public View Detail
        {
            get => _detail.Content;
            set => _detail.Content = value;
        }

        private double _x;

        #endregion

        public MasterDetailView()
        {
            Children.Add(_master,
                Constraint.Constant(0),
                Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width),
                Constraint.RelativeToParent(p => p.Height)
            );
            Children.Add(_detail,
                Constraint.RelativeToParent(p => p.Width),
                Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width),
                Constraint.RelativeToParent(p => p.Height)
            );

            PanGestureRecognizer panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += PanUpdated;
            GestureRecognizers.Add(panGesture);
        }

        private void PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (!IsSwipeable)
                return;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _x = _master.TranslationX;
                    break;
                case GestureStatus.Running:
                    var x = _x + e.TotalX;
                    if (x <= 0 && x >= -Width)
                    {
                        _master.TranslationX = x;
                        _detail.TranslationX = x;
                    }
                    break;
                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    var d = (_master.TranslationX < -Width * 0.5f) ? -Width : 0;
                    _master.TranslateTo(d, 0, (uint)((-_master.TranslationX / Width) * AnimationTime * 0.5f), Easing.CubicOut);
                    _detail.TranslateTo(d, 0, (uint)((-_master.TranslationX / Width) * AnimationTime * 0.5f), Easing.CubicOut);
                    ShowDetail = d != 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual async Task AnimateDetailIn()
        {
            await Task.WhenAll(
                _master.TranslateTo(-Width, 0, AnimationTime, Easing.CubicInOut),
                _detail.TranslateTo(-Width, 0, AnimationTime, Easing.CubicInOut)
            );
        }

        protected virtual async Task AnimateDetailOut()
        {
            await Task.WhenAll(
                _master.TranslateTo(0, 0, AnimationTime, Easing.CubicInOut),
                _detail.TranslateTo(0, 0, AnimationTime, Easing.CubicInOut)
            );
        }

        #region Bindable Properties

        public static readonly BindableProperty DetailViewProperty =
            BindableProperty.Create(
                nameof(DetailView),
                typeof(object),
                typeof(MasterDetailView),
                default(object),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((MasterDetailView) bindableObject).DetailViewPropertyChanged(oldValue, newValue);
                }
            );

        private void DetailViewPropertyChanged(object oldValue, object item)
        {
            var template = (ItemTemplate != null && ItemTemplate is DataTemplateSelector selector)
                ? selector.SelectTemplate(item, null)
                : ItemTemplate;

            try
            {
                var templateInstance = template.CreateContent();

                var templateView = templateInstance as View;
                var templateCell = templateInstance as ViewCell;

                if (templateView == null && templateCell == null)
                    throw new InvalidOperationException("DataTemplate must be either a Cell or a View");

                var view = templateView ?? templateCell.View;

                if (view != null)
                {
                    view.BindingContext = item;
                    Detail = view;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public object DetailView
        {
            get => GetValue(DetailViewProperty);
            set => SetValue(DetailViewProperty, value);
        }


        public static readonly BindableProperty ShowDetailProperty =
            BindableProperty.Create(
                nameof(ShowDetail),
                typeof(bool),
                typeof(MasterDetailView),
                default(bool),
                BindingMode.TwoWay,
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((MasterDetailView)bindableObject).ShowDetailPropertyChanged((bool)oldValue, (bool)newValue);
                }
            );

        private async void ShowDetailPropertyChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                await AnimateDetailIn();
            }
            else if (oldValue && !newValue)
            {
                await AnimateDetailOut();
            }
        }

        public bool ShowDetail
        {
            get => (bool)GetValue(ShowDetailProperty);
            set => SetValue(ShowDetailProperty, value);
        }

        public uint AnimationTime { get; set; } = 200u;

        public static readonly BindableProperty IsSwipeableProperty =
            BindableProperty.Create(
                nameof(IsSwipeable),
                typeof(bool),
                typeof(MasterDetailView),
                default(bool)
            );

        public bool IsSwipeable
        {
            get => (bool)GetValue(IsSwipeableProperty);
            set => SetValue(IsSwipeableProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(MasterDetailView),
                default(DataTemplate),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((MasterDetailView)bindableObject).InvalidateLayout();
                }
            );

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        #endregion
    }

    //private void UpdateChildren(IEnumerable items)
    //    {
    //        // reset details
    //            foreach (var child in _detailViews.Values)
    //                _containerView.Children.Remove(child);
    //        _detailViews?.Clear();

    //        // repopulate master with details
    //        foreach (var item in items)
    //        {
    //            View view = new Grid();

    //            var template = (ItemTemplate != null && ItemTemplate is DataTemplateSelector selector)
    //                ? selector.SelectTemplate(item, null)
    //                : ItemTemplate;

    //            try
    //            {
    //                var templateInstance = template.CreateContent();

    //                var templateView = templateInstance as View;
    //                var templateCell = templateInstance as ViewCell;

    //                if (templateView == null && templateCell == null)
    //                    throw new InvalidOperationException("DataTemplate must be either a Cell or a View");

    //                view = templateView ?? templateCell.View;

    //                view.BindingContext = item;
    //            }
    //            catch (Exception e)
    //            {
    //                Crashes.TrackError(e);
    //            }

    //            // set initially out of bounds
    //            view.TranslationX = _containerView.Width;
    //            view.IsVisible = false;
    //            // keep reference of created view for animating it later
    //            _detailViews.Add(item, view);
    //            // add to content container
    //            _containerView.Children.Add(view);
    //        }
    //    }
}
