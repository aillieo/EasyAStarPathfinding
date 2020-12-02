using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AillieoUtils;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "TestGridData", menuName = "TestGridData")]
public class TestGridData : ScriptableObject, IGridDataProvider
{
    [SerializeField]
    private bool[] data;

    public int rangeX;
    public int rangeY;

    public bool this[int x, int y]
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
}
