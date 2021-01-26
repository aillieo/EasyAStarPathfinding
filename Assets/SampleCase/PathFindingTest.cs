using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AillieoUtils;
using AillieoUtils.PathFinding;
using System.Diagnostics;

public class PathFindingTest : MonoBehaviour
{
    public TestGridData mapData;
    public Vector2Int start = new Vector2Int(1, 1);
    public Vector2Int end = new Vector2Int(8, 8);

    private IEnumerable<Point> path;
    private PathFinder pathFinder;

    public bool drawPassable = true;
    public bool drawBlock = true;
    public bool drawPath = true;

    public void FindPath()
    {
        if (mapData == null)
        {
            pathFinder = null;
            return;
        }

        if (pathFinder == null)
        {
            pathFinder = new PathFinder(mapData);
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();
        path = pathFinder.FindPath(new Point(start.x, start.y), new Point(end.x, end.y));
        long costTime = sw.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"costTime {costTime}ms");
    }

    private void OnDrawGizmos()
    {
        Color backup = Gizmos.color;

        if (mapData != null)
        {
            for (int i = 0; i < mapData.RangeX; ++i)
            {
                for (int j = 0; j < mapData.RangeY; ++j)
                {
                    bool passable = mapData.Passable(i, j);
                    if (passable)
                    {
                        if (drawPassable)
                        {
                            Gizmos.color = Color.white;
                            Gizmos.DrawWireCube(new Vector3(i, j, 0), Vector3.one * 0.25f);
                        }
                    }
                    else
                    {
                        if (drawBlock)
                        {
                            Gizmos.color = Color.black;
                            Gizmos.DrawWireCube(new Vector3(i, j, 0), Vector3.one * 0.5f);
                        }
                    }
                }
            }
        }

        if (drawPath && path != null)
        {
            int count = path.Count();
            int index = 0;

            foreach (var pp in path)
            {
                Gizmos.color = Color.Lerp(Color.red, Color.blue, ((float)(index++)) / count);
                Gizmos.DrawCube(new Vector3(pp.x, pp.y, 0), Vector3.one * 0.4f);
            }
        }

        Gizmos.color = backup;
    }
}
