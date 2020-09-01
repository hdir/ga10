using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Bare10.Utils
{
    public class ExplicitObservableCollection<T> : List<T>, INotifyPropertyChanged
    {
        public ExplicitObservableCollection()
        {
        }

        public ExplicitObservableCollection(IEnumerable<T> items)
        {
            AddRange(items);
            RaisePropertyChanged();
        }

        public void Replace(IEnumerable<T> items)
        {
            Clear();
            AddRange(items);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            });
        }
    }
}