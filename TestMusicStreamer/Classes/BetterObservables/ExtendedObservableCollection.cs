using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMusicStreamer.Classes.BetterObservables
{
    //TODO: Make sure these don't notify on every removal/addition
    internal class ExtendedObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        private void RemoveAllNoNotify(Func<T, bool> predicate)
        {
            var itemsToRemove = this.Items.Where(predicate);
            foreach (var item in itemsToRemove)
            {
                this.Items.Remove(item);
            }
        }

        private void AddManyNoNotify(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        public void RemoveAll(Func<T, bool> predicate)
        {
            RemoveAllNoNotify(predicate);

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddMany(IEnumerable<T> items)
        {
            AddManyNoNotify(items);

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ReplaceAll(IEnumerable<T> items)
        {
            RemoveAllNoNotify(x => true);
            AddManyNoNotify(items);

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
