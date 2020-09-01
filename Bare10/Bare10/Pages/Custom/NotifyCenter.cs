using System;
using System.Collections.Generic;
using System.Windows.Input;
using Bare10.Utils;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Bare10.Pages.Custom
{
    public class NotifyCenter : MvxNotifyPropertyChanged
    {
        private static uint _key;

        public enum InfoType
        {
            Loading, Info, Error, Warning
        }

        #region Properties

        private readonly MessageContainer _messages = new MessageContainer();

        public ExplicitObservableCollection<MessageDetails> Messages => _messages.Messages;

        #endregion

        #region Singleton

        private static readonly Lazy<NotifyCenter> lazy =
            new Lazy<NotifyCenter>(() => new NotifyCenter());

        public static NotifyCenter Instance => lazy.Value;

        #endregion

        private NotifyCenter(){ }

        public uint AddMessage(MessageDetails message)
        {
            return _messages.Add(message);
        }

        public void Clear(uint id)
        {
            _messages.Remove(id);
        }

        public void ClearAll()
        {
            _messages.Clear();
        }

        public class MessageDetails 
        {
            public string Text { get; set; }
            public InfoType Type { get; set; }
            public Action Action { get; set; }

            public ICommand ClickedCommand => new Command(() =>
            {
                Action?.Invoke();
            });
        }

        private class MessageContainer
        {
            public ExplicitObservableCollection<MessageDetails> Messages { get; } =
                new ExplicitObservableCollection<MessageDetails>();

            private Dictionary<uint, MessageDetails> _indices { get; } =
                new Dictionary<uint, MessageDetails>();

            public uint Add(MessageDetails message)
            {
                _key++;
                _indices.Add(_key, message);
                Messages.Add(message);

                Messages.RaisePropertyChanged();

                return _key;
            }

            public void Remove(uint id)
            {
                if (_indices.ContainsKey(id))
                {
                    Messages.Remove(_indices[id]);
                    _indices.Remove(id);
                    Messages.RaisePropertyChanged();
                }
            }

            public void Clear()
            {
                _indices.Clear();
                Messages.Clear();
                Messages.RaisePropertyChanged();
            }
        }
    }
}