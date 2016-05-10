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
    public class ConcurrentList<T> : IList<T>, ICollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        List<T> _collection = new List<T>();
        object _padlock = new object();

        public ConcurrentList()
        {
        }

        public T Find(Func<T, bool> predicate)
        {
            lock (_padlock)
            {
                return _collection.FirstOrDefault(predicate);
            }
        }


        public int IndexOf(T item)
        {
            lock (_padlock)
            {
                return _collection.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_padlock)
            {
                _collection.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_padlock)
            {
                _collection.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_padlock)
                {
                    return _collection[index];
                }
            }
            set
            {
                lock (_padlock)
                {
                    _collection[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            lock (_padlock)
            {
                _collection.Add(item);
            }
        }

        public void Clear()
        {
            lock (_padlock)
            {
                foreach (var item in _collection.ToList())
                    _collection.Remove(item);
            }
        }

        public bool Contains(T item)
        {
            lock (_padlock)
            {
                return _collection.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_padlock)
            {
                _collection.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (_padlock)
                {
                    return _collection.Count;
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
            lock (_padlock)
            {
                return _collection.Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_padlock)
            {
                return _collection.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_padlock)
            {
                return _collection.GetEnumerator();
            }
        }
        
        public void CopyTo(Array array, int index)
        {
            lock (_padlock)
            {
                this.ToArray().CopyTo(array, index);
            }
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

        object IList.this[int index]
        {
            get
            {
                lock (_padlock)
                {
                    return _collection[index];
                }
            }

            set
            {
                lock (_padlock)
                {
                    _collection[index] = (T)value;
                }
            }
        }

        public void Remove(object value)
        {
            lock (_padlock)
            {
                if (value is T)
                    Remove((T)value);
            }
        }        
    }
}
