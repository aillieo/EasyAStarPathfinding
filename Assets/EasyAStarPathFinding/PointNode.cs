using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils
{
    public class PointNode : IEquatable<PointNode>
    {
        public Point point;
        public PointNode previous;

        public bool Equals(PointNode other)
        {
            if (other == null)
            {
                return false;
            }

            return this.point.Equals(other.point);
        }

        public static bool operator ==(PointNode lhs, PointNode rhs)
        {
            object lho = (object)lhs;
            object rho = (object)rhs;

            if(lho == null || rho == null)
            {
                return lho == rho;
            }

            return lhs.point == rhs.point;
        }

        public static bool operator !=(PointNode lhs, PointNode rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            return Equals(other as PointNode);
        }

        public override int GetHashCode()
        {
            return point.GetHashCode();
        }
    }
}
