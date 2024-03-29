using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class NodeWrapperEx<T> : INodeWrapper<T>, IComparable<NodeWrapperEx<T>>
    {
        public T node { get; set; }
        public NodeState state { get; set; } // remove
        public float g = float.PositiveInfinity;
        public float rhs = float.PositiveInfinity;
        public float h;
        public Vector2 key;

        public NodeWrapperEx(T node)
        {
            this.node = node;
        }

        public int CompareTo(NodeWrapperEx<T> other)
        {
            Vector2 ko = other.key;
            if (key.x != ko.x)
            {
                return key.x.CompareTo(ko.x);
            }

            if (key.y != ko.y)
            {
                return key.y.CompareTo(ko.y);
            }

            return Comparer<T>.Default.Compare(node, other.node);
        }

        public NodeConsistency GetConsistency()
        {
            if (g > rhs)
            {
                return NodeConsistency.Overconsistent;
            }

            if (g < rhs)
            {
                return NodeConsistency.Underconsistent;
            }

            return NodeConsistency.Consistent;
        }
    }
}
