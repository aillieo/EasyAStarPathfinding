using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    internal class PointNodeEqualityComparer : IEqualityComparer<PointNode>
    {
        public static readonly PointNodeEqualityComparer instance = new PointNodeEqualityComparer();

        public bool Equals(PointNode x, PointNode y)
        {
            object ox = x;
            object oy = y;
            if (ox == null && oy == null)
            {
                return true;
            }

            if (ox == null || oy == null)
            {
                return false;
            }

            return x.point == y.point;
        }

        public int GetHashCode(PointNode obj)
        {
            if (obj != null)
            {
                return obj.point.GetHashCode();
            }

            return 0;
        }
    }

}
