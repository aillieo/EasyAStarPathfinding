using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AillieoUtils;
using AillieoUtils.Pathfinding;
using System.Diagnostics;
using System;
using AillieoUtils.Pathfinding.Visualizers;

namespace Samples
{
    public class SquareTileSample : MonoBehaviour
    {
        [HideInInspector] public string assetFilePath;
        public Vector2Int start = new Vector2Int(1, 1);
        public Vector2Int end = new Vector2Int(255, 255);
        public Algorithms algorithm;
        public float timeStepForCoroutine = 0.1f;

        private SquareTileMapData tileData;
        private IEnumerable<AillieoUtils.Pathfinding.Tile> path;

        public void LoadData()
        {
            tileData = SerializeHelper.Load<SquareTileMapData>(assetFilePath);
        }

        private Pathfinder pathfinder;

        public bool autoPathfinding = false;
        public bool drawPassable = true;
        public bool drawBlock = true;
        public bool drawPath = true;
        public bool drawOpenList = true;
        public bool drawClosedList = true;

        private SquareGidGizmosDrawer drawer = new SquareGidGizmosDrawer();

        private void EnsureFindingContext()
        {
            if (tileData == null)
            {
                LoadData();
            }

            if (tileData == null)
            {
                pathfinder = null;
                throw new Exception($"invalid {nameof(tileData)}");
            }

            if (pathfinder != null && pathfinder.algorithm != this.algorithm)
            {
                pathfinder = null;
            }

            if (pathfinder == null)
            {
                pathfinder = new Pathfinder(tileData, this.algorithm);
            }
        }

        public void FindPath()
        {
            EnsureFindingContext();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            path = pathfinder.FindPath(tileData.GetTile(start.x, start.y), tileData.GetTile(end.x, end.y));
            long costTime = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log($"costTime {costTime}ms");
        }

        public async void FindPathAsync()
        {
            EnsureFindingContext();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            path = await pathfinder.FindPathAsync(tileData.GetTile(start.x, start.y), tileData.GetTile(end.x, end.y));
            long costTime = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log($"costTime {costTime}ms");
        }

        public void FindPathInCoroutine()
        {
            StopAllCoroutines();

            EnsureFindingContext();

            if (pathfinder.state == PathfindingState.Found || pathfinder.state == PathfindingState.Failed)
            {
                pathfinder.CleanUp();
            }

            pathfinder.Init(tileData.GetTile(start.x, start.y), tileData.GetTile(end.x, end.y));
            StartCoroutine(InternalFindPathInCoroutine());
        }

        private IEnumerator InternalFindPathInCoroutine()
        {
            path = null;
            WaitForSeconds waitForSeconds = new WaitForSeconds(timeStepForCoroutine);
            while (pathfinder.state != PathfindingState.Found && pathfinder.state != PathfindingState.Failed)
            {
                pathfinder.FindPath();
                yield return waitForSeconds;
            }

            path = pathfinder.GetResult();
        }

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

            if (tileData != null)
            {
                if (drawPassable || drawBlock)
                {
                    int rangeX = tileData.RangeX;
                    int rangeY = tileData.RangeY;
                    for (int i = 0; i < rangeX; ++i)
                    {
                        for (int j = 0; j < rangeY; ++j)
                        {
                            bool passable = tileData.GetCost(i, j) < 0.5f;
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
                Vector3? last = null;

                foreach (var pp in path)
                {
                    Gizmos.color = Color.Lerp(Color.red, Color.blue, ((float) (index++)) / count);
                    Vector3 pos = new Vector3(pp.x, pp.y, 0);
                    Gizmos.DrawCube(pos, Vector3.one * 0.4f);
                    if (last.HasValue)
                    {
                        Gizmos.DrawLine(pos, last.Value);
                    }

                    last = pos;
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
}
