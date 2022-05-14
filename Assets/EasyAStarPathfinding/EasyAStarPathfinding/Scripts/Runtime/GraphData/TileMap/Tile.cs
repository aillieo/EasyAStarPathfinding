using System;
using UnityEngine;

namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class Tile : IEquatable<Tile>, IComparable<Tile>
    {
        public readonly int x;
        public readonly int y;

        public int id => throw new NotImplementedException();

        public int flag => throw new NotImplementedException();

        public float cost { get; set; } = 0f;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static explicit operator Vector2(Tile tile)
        {
            return new Vector2(tile.x, tile.y);
        }

        public static explicit operator Vector2Int(Tile tile)
        {
            return new Vector2Int(tile.x, tile.y);
        }

        public static bool operator ==(Tile lhs, Tile rhs)
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

        public static bool operator !=(Tile lhs, Tile rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            if (!(other is Tile))
            {
                return false;
            }

            return Equals((Tile)other);
        }

        public bool Equals(Tile other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }

        public int CompareTo(Tile other)
        {
            if (x != other.x)
            {
                return x - other.x;
            }
            return y - other.y;
        }
    }
}
