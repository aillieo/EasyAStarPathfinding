using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils
{
    public class PointNode
    {
        public Point point;
        public PointNode previous;

        private PointNode()
        {
        }

        private static Stack<PointNode> nodePool = new Stack<PointNode>();

        public static PointNode GetPointNode(Point point = default, PointNode parent = default)
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

        public static void Recycle(PointNode node)
        {
            nodePool.Push(node);
        }
    }
}
