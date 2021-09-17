namespace AillieoUtils
{
    public static class UniquePriorityQueueExt
    {
        public static bool Update<T>(this UniquePriorityQueue<T> upq, T item)
        {
            if (upq.Remove(item))
            {
                upq.Enqueue(item);
                return true;
            }

            return false;
        }

        public static T TryDequeue<T>(this UniquePriorityQueue<T> upq)
        {
            if (upq.Count > 0)
            {
                return upq.Dequeue();
            }

            return default;
        }
    }
}
