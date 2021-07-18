using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class SquareGridMapData : IGridMapData
    {
        private bool[] data = Array.Empty<bool>();
        private int rangeX = 2;
        private int rangeY = 2;

        public int RangeX => rangeX;
        public int RangeY => rangeY;

        public bool Passable(int x, int y)
        {
            return this[x, y];
        }

        public void SetPassable(int x, int y, bool passable)
        {
            this[x, y] = passable;
        }

        public void Resize(int newRangeX, int newRangeY)
        {
            if (newRangeX == rangeX && newRangeY == rangeY)
            {
                return;
            }

            bool[] oldData = data;
            data = new bool[newRangeX * newRangeY];
            for (int i = 0, xMin = Math.Min(newRangeX, rangeX); i < xMin; ++i)
            {
                for (int j = 0, yMin = Math.Min(newRangeY, rangeY); j < yMin; ++j)
                {
                    data[i + newRangeX * j] = oldData[i + rangeX * j];
                }
            }

            rangeX = newRangeX;
            rangeY = newRangeY;
        }

        private bool this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= rangeX)
                {
                    return false;
                }
                if (y < 0 || y >= rangeY)
                {
                    return false;
                }

                if (data.Length < rangeX * rangeY)
                {
                    Array.Resize(ref data, rangeX * rangeY);
                }

                return data[x + y * rangeX];
            }

            set
            {
                if (x < 0 || x >= rangeX)
                {
                    return;
                }
                if (y < 0 || y >= rangeY)
                {
                    return;
                }

                if (data.Length < rangeX * rangeY)
                {
                    Array.Resize(ref data, rangeX * rangeY);
                }

                data[x + y * rangeX] = value;
            }
        }

        public IEnumerable<Grid> CollectNeighbor(Grid current)
        {
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    if (i != 0 && j != 0)
                    {
                        // 禁止斜着走
                        continue;
                    }

                    int x = current.x + i;
                    int y = current.y + j;

                    if (!Passable(x, y))
                    {
                        continue;
                    }

                    yield return new Grid(x, y);
                }
            }
        }
    }
}
