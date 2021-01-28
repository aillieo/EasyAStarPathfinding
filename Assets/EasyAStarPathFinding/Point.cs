using System;

namespace AillieoUtils
{
    public partial struct Point : IEquatable<Point>, IComparable<Point>
    {
        public int x { get; set; }
        public int y { get; set; }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Point lhs, Point rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Point lhs, Point rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            if (!(other is Point))
                return false;

            return Equals((Point)other);
        }

        public bool Equals(Point other)
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

        public int CompareTo(Point other)
        {
            if(x != other.x)
            {
                return x - other.x;
            }
            return y - other.y;
        }
    }

}
