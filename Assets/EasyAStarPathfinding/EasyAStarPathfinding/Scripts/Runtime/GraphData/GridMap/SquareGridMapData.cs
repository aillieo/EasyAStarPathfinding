using System;
using System.Collections.Generic;
namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class SquareGridMapData : IGridMapData
    {
        private Grid[] data = Array.Empty<Grid>();
        private int rangeX = 2;
        private int rangeY = 2;

        public int RangeX => rangeX;
        public int RangeY => rangeY;

        public float GetCost(int x, int y)
        {
            Grid grid = this[x, y];

            if (grid == null)
            {
                return float.MaxValue;
            }
            else
            {
                //return true;
                return grid.cost;
            }
        }

        public void SetCost(int x, int y, float cst)
        {
            Grid grid = this[x, y];
            if (grid != null)
            {
                grid.cost = cst;
            }
        }

        public void Resize(int newRangeX, int newRangeY)
        {
            if (newRangeX == rangeX && newRangeY == rangeY)
            {
                return;
            }

            Grid[] oldData = data;
            data = new Grid[newRangeX * newRangeY];
            for (int i = 0, xMin = Math.Min(newRangeX, rangeX); i < xMin; ++i)
            {
                for (int j = 0, yMin = Math.Min(newRangeY, rangeY); j < yMin; ++j)
                {
                    data[i + newRangeX * j] = oldData[i + rangeX * j];
                }
            }

            rangeX = newRangeX;
            rangeY = newRangeY;

            FillWithDefault();
        }

        public Grid GetGrid(int x, int y)
        {
            return this[x, y];
        }

        private Grid this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= rangeX)
                {
                    return null;
                }
                if (y < 0 || y >= rangeY)
                {
                    return null;
                }

                if (data.Length < rangeX * rangeY)
                {
                    Array.Resize(ref data, rangeX * rangeY);
                    FillWithDefault();
                }

                Grid grid = data[x + y * rangeX];
                if (grid == null)
                {
                    throw new Exception("invalid grid map data");
                }
                return grid;
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
                    FillWithDefault();
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

                    if (GetCost(x, y) > 0.5f)
                    {
                        continue;
                    }

                    yield return this[x, y];
                }
            }
        }

        private void FillWithDefault()
        {
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == null)
                {
                    data[i] = new Grid(i % rangeX, i / rangeX);
                }
            }
        }
    }
}
