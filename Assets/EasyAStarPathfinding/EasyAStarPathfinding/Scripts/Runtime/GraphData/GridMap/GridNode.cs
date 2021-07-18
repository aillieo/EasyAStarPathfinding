using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public class GridNode : IComparable<GridNode>
    {
        public Grid grid;
        public GridNode previous;
        public float g;
        public float h;
        private GridNode()
        {
        }

        internal static PointNodePool Pool()
        {
            return new PointNodePool();
        }

        public int CompareTo(GridNode other)
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

            return other.grid.CompareTo(grid);
        }

        internal class PointNodePool
        {
            internal PointNodePool()
            {
            }

            private Stack<GridNode> nodePool = new Stack<GridNode>();

            internal GridNode GetPointNode(Grid grid = default, GridNode parent = default)
            {
                GridNode newNode;
                if (nodePool.Count > 0)
                {
                    newNode = nodePool.Pop();
                    newNode.grid = grid;
                    newNode.previous = parent;
                }
                else
                {
                    newNode = new GridNode { grid = grid, previous = parent };
                }
                return newNode;
            }

            public void Recycle(GridNode node)
            {
                node.g = 0f;
                node.h = 0f;
                node.previous = null;
                nodePool.Push(node);
            }
        }
    }
}
