using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class SquareGridMapData : IGridMapData
    {
        private Grid[] grids = null;
        private float[] data = Array.Empty<float>();
        private int rangeX = 0;
        private int rangeY = 0;

        public int RangeX => rangeX;
        public int RangeY => rangeY;

        public float GetCost(int x, int y)
        {
            return this[x, y];
        }

        public void SetCost(int x, int y, float cst)
        {
            this[x, y] = cst;
        }

        [Conditional("UNITY_EDITOR")]
        public void Resize(int newRangeX, int newRangeY)
        {
            if (newRangeX == rangeX && newRangeY == rangeY)
            {
                return;
            }

            float[] oldData = data;
            data = new float[newRangeX * newRangeY];
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

        public Grid GetGrid(int x, int y)
        {
            if (grids == null || grids.Length == 0)
            {
                grids = new Grid[data.Length];
            }

            int index = x + y * rangeX;
            Grid grid = grids[index];
            if (grid == null)
            {
                grid = new Grid(x, y);
                grid.cost = this[x, y];
                grids[index] = grid;
            }

            return grid;
        }

        private float this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= rangeX)
                {
                    return float.MaxValue;
                }
                if (y < 0 || y >= rangeY)
                {
                    return float.MaxValue;
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

                    yield return GetGrid(x, y);
                }
            }
        }
    }
}
