using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class NodeWrapper<T> : IComparable<NodeWrapper<T>> where T : IGraphNode
    {
        public T node;
        public NodeWrapper<T> previous;
        public float g;
        public float h;
        public NodeState state;

        public NodeWrapper(T node, NodeWrapper<T> previous = default)
        {
            this.node = node;
            this.previous = previous;
        }

        public int CompareTo(NodeWrapper<T> other)
        {
            float f1 = h + g;
            float f2 = other.h + other.g;

            if (f1 != f2)
            {
                return f2.CompareTo(f1);
            }

            if (h != other.h)
            {
                return other.h.CompareTo(h);
            }

            return Comparer<T>.Default.Compare(node, other.node);
        }
    }
}
