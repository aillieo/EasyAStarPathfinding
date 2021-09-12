using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class NodeWrapper<T> : INodeWrapper<T>, IComparable<NodeWrapper<T>> where T : IGraphNode
    {
        public T node { get; set; }
        public NodeState state { get; set; }
        public NodeWrapper<T> previous;
        public float g;
        public float h;

        public NodeWrapper(T node)
        {
            this.node = node;
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
