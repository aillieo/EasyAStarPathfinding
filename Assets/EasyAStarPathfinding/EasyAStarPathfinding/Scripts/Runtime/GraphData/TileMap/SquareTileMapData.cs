using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AillieoUtils.Pathfinding
{
    [Serializable]
    public class SquareTileMapData : ITileMapData
    {
        private float costScale = 1.0f;
        private bool allowDiagonalMove;
        private Tile[] tiles = Array.Empty<Tile>();
        private int rangeX = 0;
        private int rangeY = 0;

        public float CostScale => costScale;
        public bool AllowDiagonalMove => allowDiagonalMove;
        public int RangeX => rangeX;
        public int RangeY => rangeY;

        public float GetCost(int x, int y)
        {
            Tile t = GetTile(x, y);
            return t.cost;
        }

        public void SetCost(int x, int y, float cst)
        {
            Tile t = GetTile(x, y);
            t.cost = cst;
        }

        [Conditional("UNITY_EDITOR")]
        public void Resize(int newRangeX, int newRangeY)
        {
            if (newRangeX == rangeX && newRangeY == rangeY)
            {
                return;
            }

            Tile[] oldTiles = tiles;
            tiles = new Tile[newRangeX * newRangeY];
            for (int i = 0, xMin = Math.Min(newRangeX, rangeX); i < xMin; ++i)
            {
                for (int j = 0, yMin = Math.Min(newRangeY, rangeY); j < yMin; ++j)
                {
                    Tile oldTile = oldTiles[i + rangeX * j];
                    Tile newTile = new Tile(i, j);
                    newTile.cost = oldTile.cost;
                    tiles[i + newRangeX * j] = newTile;
                }
            }

            rangeX = newRangeX;
            rangeY = newRangeY;
        }

        [Conditional("UNITY_EDITOR")]
        public void SetCostScale(float newCostScale)
        {
            this.costScale = newCostScale;
        }

        [Conditional("UNITY_EDITOR")]
        public void SetAllowDiagonalMove(bool newAllowDiagonalMove)
        {
            this.allowDiagonalMove = newAllowDiagonalMove;
        }

        public Tile GetTile(int x, int y)
        {
            if (tiles == null || tiles.Length == 0)
            {
                tiles = new Tile[rangeX * rangeY];
            }

            int index = x + y * rangeX;
            Tile tile = tiles[index];
            if (tile == null)
            {
                tile = new Tile(x, y);
                tile.cost = 0f;
                tiles[index] = tile;
            }

            return tile;
        }

        public IEnumerable<Tile> CollectNeighbor(Tile current)
        {
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    if (i != 0 && j != 0 && !allowDiagonalMove)
                    {
                        // 禁止斜着走
                        continue;
                    }

                    int x = current.x + i;
                    int y = current.y + j;

                    if (x < 0 || x >= rangeX || y < 0 || y >= rangeY)
                    {
                        continue;
                    }

                    yield return GetTile(x, y);
                }
            }
        }

        public IEnumerable<Tile> GetAllNodes()
        {
            for (int i = 0; i < rangeX; ++i)
            {
                for (int j = 0; j < rangeY; ++j)
                {
                    yield return GetTile(i, j);
                }
            }
        }
    }
}
