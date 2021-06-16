using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AillieoUtils.Pathfinding;

[CreateAssetMenu(fileName = "TestGridData", menuName = "TestGridData")]
public class TestGridData : ScriptableObject, IGridData
{
    // public SquareGridData data;

    [SerializeField]
    private bool[] data;
    [SerializeField]
    private int rangeX;
    [SerializeField]
    private int rangeY;

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
        for (int i = 0, xMin = Mathf.Min(newRangeX, rangeX); i < xMin; ++ i)
        {
            for (int j = 0, yMin = Mathf.Min(newRangeY, rangeY); j < yMin; ++j)
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

    public IEnumerable<Vector2Int> Obstacles()
    {
        if(data == null)
        {
            yield break;
        }

        for(int i = 0; i < rangeX * rangeY; ++ i)
        {
            if(!data[i])
            {
                yield return new Vector2Int(i % rangeX, i / rangeX);
            }
        }
    }

    public IEnumerable<Point> CollectNeighbor(Point current)
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

                yield return new Point(x, y);
            }
        }
    }
}
