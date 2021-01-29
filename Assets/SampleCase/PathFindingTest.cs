using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AillieoUtils;
using AillieoUtils.PathFinding;
using System.Diagnostics;
using System;

public class PathFindingTest : MonoBehaviour
{
    public TestGridData gridData;
    public Vector2Int start = new Vector2Int(1, 1);
    public Vector2Int end = new Vector2Int(255, 255);
    public float timeStepForCoroutine = 0.1f;

    private readonly HashSet<Vector2Int> openList = new HashSet<Vector2Int>();
    private readonly HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

    private IEnumerable<Point> path;
    private PathFinder pathFinder;

    public bool drawPassable = true;
    public bool drawBlock = true;
    public bool drawPath = true;
    public bool drawOpenList = true;
    public bool drawClosedList = true;

    private void EnsureFindingContext()
    {
        if (gridData == null)
        {
            pathFinder = null;
            throw new Exception($"invalid {nameof(gridData)}");
        }

        if (pathFinder == null)
        {
            pathFinder = new PathFinder(gridData);
        }
    }

    public void FindPath()
    {
        EnsureFindingContext();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        path = pathFinder.FindPath(new Point(start.x, start.y), new Point(end.x, end.y));
        long costTime = sw.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"costTime {costTime}ms");
    }

    public async void FindPathAsync()
    {
        EnsureFindingContext();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        path = await pathFinder.FindPathAsync(new Point(start.x, start.y), new Point(end.x, end.y));
        long costTime = sw.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"costTime {costTime}ms");
    }

    public void FindPathInCoroutine()
    {
        EnsureFindingContext();

        openList.Clear();
        closedList.Clear();
        StartCoroutine(pathFinder.FindPathInCoroutine(new Point(start.x, start.y), new Point(end.x, end.y), new WaitForSeconds(timeStepForCoroutine), info =>
        {
            HashSet<Vector2Int> list = default;

            if ((info.changeFlag & PointChangeFlag.OpenList) != PointChangeFlag.None)
            {
                list = openList;
            }
            else
            {
                list = closedList;
            }

            if ((info.changeFlag & PointChangeFlag.Add) != PointChangeFlag.None)
            {
                list.Add(new Vector2Int(info.point.x, info.point.y));
            }
            else
            {
                list.Remove(new Vector2Int(info.point.x, info.point.y));
            }
        }));
    }

    //private IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(3);
    //    FindPath();
    //    yield return null;
    //    yield return null;
    //    UnityEditor.EditorApplication.isPaused = true;
    //}

    private void OnDrawGizmos()
    {
        Color backup = Gizmos.color;

        if (gridData != null)
        {
            if (drawPassable || drawBlock)
            {
                int rangeX = gridData.RangeX;
                int rangeY = gridData.RangeY;
                for (int i = 0; i < rangeX; ++i)
                {
                    for (int j = 0; j < rangeY; ++j)
                    {
                        bool passable = gridData.Passable(i, j);
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

        if (drawOpenList)
        {
            Gizmos.color = Color.black;
            foreach (var p in openList)
            {
                Gizmos.DrawCube(new Vector3(p.x, p.y, 0), Vector3.one * 0.4f);
            }
        }

        if (drawClosedList)
        {
            Gizmos.color = Color.white;
            foreach (var p in closedList)
            {
                Gizmos.DrawCube(new Vector3(p.x, p.y, 0), Vector3.one * 0.4f);
            }
        }

        Gizmos.color = backup;
    }
}
