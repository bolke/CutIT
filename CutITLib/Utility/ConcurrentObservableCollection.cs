using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace CutIT.Utility
{
    public class ConcurrentObservableCollection<T> : IList<T>, ICollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableCollection<T> collection;
        private object padlock = new object();

        public ConcurrentObservableCollection()
        {
            collection = new ObservableCollection<T>();
            collection.CollectionChanged += collectionChanged;
        }

        void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(sender, e);
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public T Find(Func<T, bool> predicate)
        {
            lock (padlock)
            {
                return collection.FirstOrDefault(predicate);
            }
        }


        public int IndexOf(T item)
        {
            lock (padlock)
            {
                return collection.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (padlock)
            {
                collection.Insert(index, item);
            }
        }
        public void Move(int index, int newindex)
        {
            try
            {
                lock (padlock)
                {
                    if (newindex >= (collection.Count - 1))
                        collection.Move(index, (collection.Count - 1));
                    else
                        collection.Move(index, newindex);

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("!!!!!!" + e.Message);
            }
        }

        public void RemoveAt(int index)
        {
            lock (padlock)
            {
                collection.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (padlock)
                {
                    return collection[index];
                }
            }
            set
            {
                lock (padlock)
                {
                    collection[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            lock (padlock)
            {
                collection.Add(item);
            }
        }

        public void Clear()
        {
            lock (padlock)
            {
                foreach (var item in collection.ToList())
                    collection.Remove(item);
            }
        }

        public bool Contains(T item)
        {
            lock (padlock)
            {
                return collection.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (padlock)
            {
                collection.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (padlock)
                {
                    return collection.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(T item)
        {
            lock (padlock)
            {
                return collection.Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (padlock)
            {
                return collection.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (padlock)
            {
                return collection.GetEnumerator();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return false;
        }

        public int IndexOf(object value)
        {
            if (value is T)
                return IndexOf((T)value);
            return -1;
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            if (value is T)
                Remove((T)value);
        }

        object IList.this[int index]
        {
            get
            {
                lock (padlock)
                {
                    return (collection as IList)[index];
                }
            }
            set
            {
                lock (padlock)
                {
                    (collection as IList)[index] = value;
                }
            }
        }

        public T Pop()
        {
            T result = default(T);
            lock (padlock)
            {
                if (collection.Count > 0)
                {
                    result = collection[0];
                    collection.RemoveAt(0);
                }
            }
            return result;
        }

        public bool TryDequeue(out T element)
        {
            bool result = false;
            lock (padlock)
            {
                if (collection.Count > 0)
                {
                    element = collection[0];
                    collection.RemoveAt(0);
                    result = true;
                }
                else
                {
                    element = default(T);
                }
            }
            return result;
        }
    }
}
