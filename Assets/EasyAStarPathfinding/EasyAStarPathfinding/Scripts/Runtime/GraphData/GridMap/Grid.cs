using System;

namespace AillieoUtils.Pathfinding
{
    public class Grid : IGraphNode, IEquatable<Grid>, IComparable<Grid>
    {
        public readonly int x;
        public readonly int y;

        public int id => throw new NotImplementedException();

        public int flag => throw new NotImplementedException();

        public float cost { get; set; } = 0f;

        public Grid(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Grid lhs, Grid rhs)
        {
            bool lNull = (object)lhs == null;
            bool rNull = (object)rhs == null;

            if (lNull && rNull)
            {
                return true;
            }

            if (lNull || rNull)
            {
                return false;
            }

            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Grid lhs, Grid rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            if (!(other is Grid))
            {
                return false;
            }

            return Equals((Grid)other);
        }

        public bool Equals(Grid other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }

        public int CompareTo(Grid other)
        {
            if (x != other.x)
            {
                return x - other.x;
            }
            return y - other.y;
        }
    }

}
