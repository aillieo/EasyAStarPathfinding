using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils
{
    public class UniquePriorityQueue<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
        private readonly HashSet<T> set;
        private readonly PriorityQueue<T> queue;

        private class EqualityComparer : IEqualityComparer<T>
        {
            private readonly IComparer<T> comparer;
            public EqualityComparer(IComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public bool Equals(T x, T y)
            {
                return comparer.Compare(x, y) == 0;
            }

            public int GetHashCode(T obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                return obj.GetHashCode();
            }
        }

        public UniquePriorityQueue()
            : this(null, null)
        { }

        public UniquePriorityQueue(IComparer<T> comparer)
            : this(null, comparer)
        {
        }

        public UniquePriorityQueue(IEqualityComparer<T> equalityComparer, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                queue = new PriorityQueue<T>();
            }
            else
            {
                queue = new PriorityQueue<T>(comparer);
                if (equalityComparer == null)
                {
                    equalityComparer = new EqualityComparer(comparer);
                }
            }

            if (equalityComparer == null)
            {
                set = new HashSet<T>();
            }
            else
            {
                set = new HashSet<T>(equalityComparer);
            }
        }

        public int Count => queue.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Clear()
        {
            set.Clear();
            queue.Clear();
        }

        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public T Dequeue()
        {
            T item = queue.Dequeue();
            set.Remove(item);
            return item;
        }

        public bool Enqueue(T item)
        {
            if (set.Add(item))
            {
                queue.Enqueue(item);
                return true;
            }
            return false;
        }

        public T Peek()
        {
            return queue.Peek();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }
}
