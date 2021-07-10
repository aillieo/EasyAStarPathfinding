using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AillieoUtils;
using AillieoUtils.Pathfinding;
using System.Diagnostics;
using System;
using AillieoUtils.Pathfinding.Visualizers;

public class PathfindingTest : MonoBehaviour
{
    public SquareGridData gridData;
    public Vector2Int start = new Vector2Int(1, 1);
    public Vector2Int end = new Vector2Int(255, 255);
    public float timeStepForCoroutine = 0.1f;

    private IEnumerable<Point> path;

    public void LoadData(string dataPath)
    {
        gridData = SerializeHelper.Load<SquareGridData>(dataPath);
    }

    private Pathfinder pathfinder;

    public bool autoPathfinding = false;
    public bool drawPassable = true;
    public bool drawBlock = true;
    public bool drawPath = true;
    public bool drawOpenList = true;
    public bool drawClosedList = true;

    private GizmosDrawer drawer = new GizmosDrawer();

    private void EnsureFindingContext()
    {
        if (gridData == null)
        {
            pathfinder = null;
            throw new Exception($"invalid {nameof(gridData)}");
        }

        if (pathfinder == null)
        {
            pathfinder = new Pathfinder(gridData);
        }
    }

    public void FindPath()
    {
        EnsureFindingContext();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        path = pathfinder.FindPath(new Point(start.x, start.y), new Point(end.x, end.y));
        long costTime = sw.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"costTime {costTime}ms");
    }

    public async void FindPathAsync()
    {
        EnsureFindingContext();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        path = await pathfinder.FindPathAsync(new Point(start.x, start.y), new Point(end.x, end.y));
        long costTime = sw.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"costTime {costTime}ms");
    }

    public void FindPathInCoroutine()
    {
        EnsureFindingContext();
        StartCoroutine(pathfinder.FindPathInCoroutine(new Point(start.x, start.y), new Point(end.x, end.y), new WaitForSeconds(timeStepForCoroutine)));
    }

    public void FindPathInCoroutineV2()
    {
        StopAllCoroutines();

        EnsureFindingContext();

        if (pathfinder.state == PathfindingState.Found || pathfinder.state == PathfindingState.Failed)
        {
            pathfinder.CleanUp();
        }

        pathfinder.Init(new Point(start.x, start.y), new Point(end.x, end.y));
        StartCoroutine(InternalFindPathInCoroutineV2());
    }

    private IEnumerator InternalFindPathInCoroutineV2()
    {
        while (pathfinder.state != PathfindingState.Found && pathfinder.state != PathfindingState.Failed)
        {
            pathfinder.FindPath();
            yield return new WaitForSeconds(timeStepForCoroutine);
        }
    }

    //private IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(3);
    //    FindPath();
    //    yield return null;
    //    yield return null;
    //    UnityEditor.EditorApplication.isPaused = true;
    //}

    private void Update()
    {
        if (autoPathfinding)
        {
            FindPath();
        }
    }

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

        if (drawOpenList || drawClosedList)
        {
            drawer.drawClosedList = this.drawClosedList;
            drawer.drawOpenList = this.drawOpenList;
            if (pathfinder != null)
            {
                drawer.Draw(pathfinder);
            }
        }

        Gizmos.color = backup;
    }
}
