using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AillieoUtils
{
    public class UniquePriorityQueue<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
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
            : this(defaultCapacity, null)
        {
        }

        public UniquePriorityQueue(int capacity)
            : this(capacity, null)
        {
        }

        public UniquePriorityQueue(IComparer<T> comparer)
            : this(defaultCapacity, comparer)
        {
        }

        public UniquePriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
            this.data = new T[capacity];

            EqualityComparer equalityComparer = new EqualityComparer(this.comparer);
            set = new Dictionary<T, int>(capacity, equalityComparer);
        }

        private readonly IComparer<T> comparer;
        private T[] data;
        private const int defaultCapacity = 16;
        private Dictionary<T, int> set;

        public int Count { get; private set; }

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool Enqueue(T item)
        {
            if (set.ContainsKey(item))
            {
                return false;
            }

            if (Count >= data.Length)
            {
                Array.Resize(ref data, Count * 2);
            }

            data[Count] = item;
            set[item] = Count;
            SiftUp(Count++);

            return true;
        }

        public T Dequeue()
        {
            T v = Peek();
            set.Remove(v);

            Count--;

            T last = data[Count];
            data[0] = last;
            set[last] = 0;

            if (Count > 0)
            {
                SiftDown(0);
            }
            return v;
        }

        public void Clear()
        {
            Count = 0;
            set.Clear();
        }

        public T Peek()
        {
            if (Count > 0)
            {
                return data[0];
            }

            throw new Exception($"attempt to get Top from a empty {nameof(PriorityQueue<T>)}");
        }

        private void SiftUp(int n)
        {
            T v = data[n];
            for (int n2 = n / 2;
                n > 0 && comparer.Compare(v, data[n2]) > 0;
                n = n2, n2 /= 2)
            {
                T v0 = data[n2];
                data[n] = v0;
                set[v0] = n;
            }

            data[n] = v;
            set[v] = n;
        }

        private void SiftDown(int n)
        {
            T v = data[n];
            for (int n2 = n * 2;
                n2 < Count;
                n = n2, n2 *= 2)
            {
                if (n2 + 1 < Count && comparer.Compare(data[n2 + 1], data[n2]) > 0)
                {
                    n2++;
                }

                if (comparer.Compare(v, data[n2]) >= 0)
                {
                    break;
                }

                T v0 = data[n2];
                data[n] = v0;
                set[v0] = n;
            }

            data[n] = v;
            set[v] = n;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return data[i];
            }
        }

        public bool Remove(T item)
        {
            if (set.TryGetValue(item, out int index))
            {
                for (int i = index; i < Count - 1; ++i)
                {
                    T v0 = data[i + 1];
                    data[i] = v0;
                    set[v0] = i;
                }

                this.Count--;
                SiftDown(index);

                return true;
            }

            return false;
        }
    }
}
