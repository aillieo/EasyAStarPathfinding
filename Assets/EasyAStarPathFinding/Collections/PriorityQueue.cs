using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils
{
    public class PriorityQueue<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
        private readonly IComparer<T> comparer;
        private T[] data;
        private const int defaultCapacity = 16;

        public int Count { get; private set; }

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public PriorityQueue()
            : this(null) { }

        public PriorityQueue(int capacity)
            : this(capacity, null) { }

        public PriorityQueue(IComparer<T> comparer)
            : this(defaultCapacity, comparer) { }

        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
            this.data = new T[capacity];
        }

        public void Enqueue(T item)
        {
            if (Count >= data.Length)
            {
                Array.Resize(ref data, Count * 2);
            }

            data[Count] = item;
            SiftUp(Count++);
        }

        public T Dequeue()
        {
            var v = Peek();
            data[0] = data[--Count];
            if (Count > 0)
            {
                SiftDown(0);
            }
            return v;
        }

        public void Clear()
        {
            Count = 0;
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
            var v = data[n];
            for (var n2 = n / 2; n > 0 && comparer.Compare(v, data[n2]) > 0; n = n2, n2 /= 2)
            {
                data[n] = data[n2];
            }

            data[n] = v;
        }

        private void SiftDown(int n)
        {
            var v = data[n];
            for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2)
            {
                if (n2 + 1 < Count && comparer.Compare(data[n2 + 1], data[n2]) > 0)
                {
                    n2++;
                }

                if (comparer.Compare(v, data[n2]) >= 0)
                {
                    break;
                }

                data[n] = data[n2];
            }
            data[n] = v;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return data[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return data[i];
            }
        }
    }
}
