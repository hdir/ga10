using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Bare10.Pages.Card;
using Bare10.Resources;
using Bare10.Utils.Effects;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.ViewControllers
{
    public class BulletinStackView : Grid
    {
        public BulletinStackView()
        {
            BackgroundColor = Colors.BulletinPopupBackground;
            
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += PanGestureOnPanUpdated;
            GestureRecognizers.Add(panGesture);

            Effects.Add(new ViewLifeCycleEffect(loaded:(sender, args) =>
            {
                if (ItemSource?.Count > 0)
                    ShowNextBulletin();
            }));
            Setup();
        }

        protected override async void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsVisible))
            {
                if (IsVisible)
                {
                    await this.FadeTo(1f, 150u, Easing.CubicOut);
                }
            }
        }

        /// <summary>
        /// Setup stack with views from ItemSource
        /// </summary>
        private void Setup()
        {
            Children.Clear();

            if (ItemSource?.Count > 0)
            {
                InputTransparent = false;

                foreach (var item in ItemSource)
                {
                    var content = ItemTemplate?.CreateContent() as View;
                    //if (content != null)
                    //    content.InputTransparent = true; // prevent from accepting input because parent controls sliding

                    var view = new CardView(content)
                    {
                        TranslationY = 20,
                        Scale = 0.90f,
                        BindingContext = item,
                        AnchorY = 1, // anchor to bottom
                        CloseButtonClicked = CloseTop,
                    };
                    Children.Add(view);
                }
            }

            if (Children.Count > 0)
                ShowNextBulletin();
        }

        #region Animation and Control 

        /// <summary>
        /// Shows the next bulletin in the stack
        /// </summary>
        private async void ShowNextBulletin()
        {
            var first = Children.LastOrDefault();

            if (first == null)
                return;

            // show front
            AnimateBulletin(first, 1f, 0);

            if (Children.Count > 1)
            {
                // animate stacks up
                await Task.Delay(120);
                AnimateBulletin(Children[Children.Count - 2], 0.95f, 10);
            }
        }

        private async void AnimateBulletin(View card, float targetScale, float targetY)
        {
            await Task.WhenAll(
                card.ScaleTo(targetScale, 500u, easing: Easing.SpringOut),
                card.TranslateTo(0, targetY, 500u, easing: Easing.CubicInOut)
            );
        }

        /// <summary>
        /// Closes the current view (top view in stack)
        /// </summary>
        public async void CloseTop()
        {
            var view = Children.LastOrDefault();

            if (view != null)
            {
                await view.FadeTo(0, easing: Easing.CubicOut);
                Children.Remove(view);
            }

            if (Children.Count > 0)
                ShowNextBulletin();
            else 
                CloseStack();
        }

        /// <summary>
        /// When stack is empty we invoke event
        /// </summary>
        private void CloseStack()
        {
            InputTransparent = true;
            EmptyStackCommand?.Execute(this);
        }

        #endregion

        #region Handling Top Bulletin (Touches)

        private async void PanGestureOnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    break;
                case GestureStatus.Running:
                    HandleTouchRunning(Children.LastOrDefault(), (float)e.TotalX);
                    break;
                case GestureStatus.Completed:
                    await HandleTouchCompleted(Children.LastOrDefault());
                    break;
                case GestureStatus.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleTouchRunning(VisualElement view, float xDiff)
        {
            if (view == null)
                return;

            // Move the card
            view.TranslationX = (xDiff);

            // Calculate a angle for the card
            var rotationAngel = (float)(0.3f * Math.Min(xDiff / Width, 1.0f));
            view.Rotation = rotationAngel * 57.2957795f;
        }

        private async Task HandleTouchCompleted(View view)
        {
            if (view == null)
                return;

            // if moved more than half if itself it's counted as read
            if (Math.Abs(view.TranslationX) > view.Width / 2f)
            {
                // animate out
                var dir = view.TranslationX / Math.Abs(view.TranslationX);
                await Task.WhenAll(
                    view.TranslateTo(dir * App.ScreenWidth, 0, easing: Easing.Linear),
                    view.FadeTo(0, 250, Easing.CubicOut)
                );
                // Close current card
                CloseTop();
            }
            else
            {
                // snap back to original position
                await Task.WhenAll(
                    view.TranslateTo(0, 0, easing:Easing.CubicOut),
                    view.RotateTo(0, easing:Easing.CubicOut)
                );
            }
        }

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty ItemSourceProperty =
            BindableProperty.Create(
                nameof(ItemSource),
                typeof(ICollection),
                typeof(BulletinStackView),
                default(ICollection),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((BulletinStackView) bindableObject).ItemSourcePropertyChanged((ICollection) oldValue, (ICollection) newValue);
                }
            );

        private void ItemSourcePropertyChanged(ICollection oldValue, ICollection newValue)
        {
            if (oldValue is INotifyPropertyChanged old)
                old.PropertyChanged -= CollectionChanged;
            if (newValue is INotifyPropertyChanged @new)
                @new.PropertyChanged += CollectionChanged;
            Setup();
        }

        private void CollectionChanged(object sender, PropertyChangedEventArgs e)
        {
            Setup();
        }

        public ICollection ItemSource
        {
            get => (ICollection) GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(BulletinStackView),
                default(DataTemplate),
                propertyChanged:
                (bindableObject, oldValue, newValue) =>
                {
                    ((BulletinStackView) bindableObject).ItemTemplatePropertyChanged((DataTemplate) oldValue, (DataTemplate) newValue);
                }
            );

        private void ItemTemplatePropertyChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            Setup();
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty EmptyStackCommandProperty =
            BindableProperty.Create(
                nameof(EmptyStackCommand),
                typeof(ICommand),
                typeof(BulletinStackView),
                default(ICommand)
            );

        public ICommand EmptyStackCommand
        {
            get => (ICommand) GetValue(EmptyStackCommandProperty);
            set => SetValue(EmptyStackCommandProperty, value);
        }


        #endregion
    }
}
