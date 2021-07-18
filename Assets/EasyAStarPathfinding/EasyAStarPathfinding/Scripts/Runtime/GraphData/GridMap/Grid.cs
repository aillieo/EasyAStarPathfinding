using System;

namespace AillieoUtils.Pathfinding
{
    public struct Grid : IEquatable<Grid>, IComparable<Grid>
    {
        public int x { get; set; }
        public int y { get; set; }

        public Grid(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Grid lhs, Grid rhs)
        {
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
