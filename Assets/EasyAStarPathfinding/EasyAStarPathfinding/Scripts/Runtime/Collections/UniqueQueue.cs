using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class UniqueQueue<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
        private readonly HashSet<T> set;
        private readonly Queue<T> queue;

        public UniqueQueue()
        {
            set = new HashSet<T>();
            queue = new Queue<T>();
        }

        public UniqueQueue(IEqualityComparer<T> comparer)
        {
            set = new HashSet<T>(comparer);
            queue = new Queue<T>();
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

        public IEnumerator GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        public T Peek()
        {
            return queue.Peek();
        }
    }
}
