using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using Bare10.Pages.Popups;

namespace Bare10.ViewModels.Base
{
    public class Toolbar : MvxNotifyPropertyChanged, IToolbarState
    {
        public Stack<StackItem> Stack { get; } = new Stack<StackItem>();

        #region Events
        public event Action OnShareButtonClicked;
        public event Action OnStackChanged;
        #endregion

        #region Commands
        public IMvxCommand BackButtonClicked { get; private set; }
        public IMvxCommand ShareButtonClicked { get; private set; }
        public IMvxAsyncCommand ShowDebugPopup { get; private set; }
        #endregion

        #region Properties
        public bool ShowBackButton => Stack.Any();

        private FormattedString _title;
        public FormattedString Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _showShareButton;
        public bool ShowShareButton
        {
            get => _showShareButton;
            set => SetProperty(ref _showShareButton, value);
        }

        private bool _infoButtonActive;
        public bool InfoButtonActive
        {
            get => _infoButtonActive;
            set => SetProperty(ref _infoButtonActive, value);
        }

        private bool _showIcon;
        public bool ShowIcon
        {
            get => _title == null && _showIcon;
            set => SetProperty(ref _showIcon, value);
        }

        private string _backButtonText;
        public string BackButtonText
        {
            get => _backButtonText;
            set => SetProperty(ref _backButtonText, value);
        }
        #endregion

        #region Singleton

        private static readonly Lazy<Toolbar> lazy =
            new Lazy<Toolbar>(() => new Toolbar());

        public static Toolbar Instance => lazy.Value;

        #endregion

        private Toolbar()
        {
            ShareButtonClicked = new MvxCommand(() =>
            {
                OnShareButtonClicked?.Invoke();
                Analytics.TrackEvent(TrackingEvents.ItemTapped,
                    new TrackingEvents.ItemTappedArgs(TrackingEvents.ItemsToTap.Share));
            });
            BackButtonClicked = new MvxCommand(Pop);

#if DEBUG
            ShowDebugPopup = new MvxAsyncCommand(async () => await PopupNavigation.Instance.PushAsync(new DebugPopup(testingViewModel)));
#endif
        }

#if DEBUG
        private TestingViewModel testingViewModel = new TestingViewModel();
#endif

        public void Push(IToolbarState newState, Action onPopped)
        {
            Stack.Push(new StackItem(ToState(), onPopped));
            SetState(newState);

            RaisePropertyChanged(() => ShowBackButton);
            OnStackChanged?.Invoke();
        }

        public void Pop()
        {
            if (!Stack.Any()) return;

            var popped = Stack.Pop();
            if (popped != null)
            {
                SetState(popped.PreviousToolbar);
                popped.OnPopped();
            }
            RaisePropertyChanged(() => ShowBackButton);
            OnStackChanged?.Invoke();
        }

        public void PopAll()
        {
            if (!Stack.Any()) return;

            while (Stack.Any())
            {
                var popped = Stack.Pop();
                if (popped != null)
                {
                    SetState(popped.PreviousToolbar);
                    popped.OnPopped();
                }
            }
            RaisePropertyChanged(() => ShowBackButton);
            OnStackChanged?.Invoke();
        }

        public void SetState(IToolbarState toolbar)
        {
            ShowShareButton = toolbar.ShowShareButton;
            InfoButtonActive = toolbar.InfoButtonActive;
            Title = toolbar.Title;
            ShowIcon = toolbar.ShowIcon;
            BackButtonText = toolbar.BackButtonText;
        }

        public IToolbarState ToState() => new ToolbarState()
        {
            Title = Title,
            ShowShareButton = ShowShareButton,
            InfoButtonActive = InfoButtonActive,
            ShowIcon = ShowIcon,
            BackButtonText = BackButtonText,
        };
    }

    public interface IToolbarState
    {
        FormattedString Title { get; set; }
        bool ShowShareButton { get; set; }
        bool InfoButtonActive { get; set; }
        bool ShowIcon { get; set; }
        string BackButtonText { get; set; }
    }

    public class ToolbarState : IToolbarState
    {
        public FormattedString Title { get; set; }
        public bool ShowShareButton { get; set; }
        public bool InfoButtonActive { get; set; }
        public bool ShowIcon { get; set; }
        public string BackButtonText { get; set; }
    }

    public class StackItem
    {
        private event Action CloseEvent;

        public IToolbarState PreviousToolbar { get; }

        public StackItem(IToolbarState previous, Action close)
        {
            PreviousToolbar = previous;
            CloseEvent = close;
        }

        public void OnPopped()
        {
            CloseEvent?.Invoke();
        }
    }
}
