using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils
{
    public class PointNode
    {
        public Point point;
        public PointNode previous;
        public float g;
        private PointNode()
        {
        }

        internal static PointNodePool Pool()
        {
            return new PointNodePool();
        }

        [ThreadStatic]
        private static PointNode dummy;

        public static PointNode Dummy(Point point)
        {
            if (dummy == null)
            {
                dummy = new PointNode();
            }

            dummy.point = point;
            return dummy;
        }

        internal class PointNodePool
        {
            internal PointNodePool()
            {
            }

            private Stack<PointNode> nodePool = new Stack<PointNode>();

            internal PointNode GetPointNode(Point point = default, PointNode parent = default)
            {
                PointNode newNode;
                if (nodePool.Count > 0)
                {
                    newNode = nodePool.Pop();
                    newNode.point = point;
                    newNode.previous = parent;
                }
                else
                {
                    newNode = new PointNode { point = point, previous = parent };
                }
                return newNode;
            }

            public void Recycle(PointNode node)
            {
                node.previous = null;
                nodePool.Push(node);
            }
        }
    }
}
