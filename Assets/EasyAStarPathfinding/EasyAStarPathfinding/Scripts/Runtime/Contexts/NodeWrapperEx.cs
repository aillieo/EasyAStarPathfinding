using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    public class NodeWrapperEx<T> : NodeWrapper<T>, IComparable<NodeWrapperEx<T>> where T : IGraphNode
    {
        public float rhs;
        public readonly HashSet<NodeWrapperEx<T>> successors = new HashSet<NodeWrapperEx<T>>();

        public NodeWrapperEx(T node, NodeWrapper<T> previous)
            : base(node, previous)
        {
        }

        public int CompareTo(NodeWrapperEx<T> other)
        {
            float min1 = Mathf.Min(this.g, this.rhs);
            float min2 = Mathf.Min(other.g, other.rhs);
            float k11 = min1 + h;
            float k12 = min2 + other.h;
            if (k11 != k12)
            {
                return k12.CompareTo(k11);
            }

            if (min1 != min2)
            {
                return min2.CompareTo(min1);
            }

            return Comparer<T>.Default.Compare(node, other.node);
        }

        public NodeConsistency GetConsistency()
        {
            if (g > rhs)
            {
                return NodeConsistency.LocallyOverconsistent;
            }

            if (g < rhs)
            {
                return NodeConsistency.LocallyUnderconsistent;
            }

            return NodeConsistency.LocallyConsistent;
        }
    }
}
