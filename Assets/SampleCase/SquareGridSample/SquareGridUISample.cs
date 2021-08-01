using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AillieoUtils;
using AillieoUtils.Pathfinding;
using System.Diagnostics;
using System;
using AillieoUtils.Pathfinding.Visualizers;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Grid = AillieoUtils.Pathfinding.Grid;

namespace Samples
{
    public class SquareGridUISample : MonoBehaviour
    {
        [SerializeField]
        private UISquareGridView view;
        [SerializeField]
        public string assetFilePath;

        private Vector2Int start = new Vector2Int(0, 0);
        private Vector2Int end = new Vector2Int(19, 19);
        private Algorithms algorithm;
        private float timeStepForCoroutine = 0.1f;

        [SerializeField]
        private Dropdown algorithmSelector;


        private SquareGridMapData gridData;
        private IEnumerable<AillieoUtils.Pathfinding.Grid> path;

        private void Awake()
        {
            var enums = typeof(Algorithms).GetEnumNames();
            var options = enums.Select(s => new Dropdown.OptionData { text = s }).ToList();
            algorithmSelector.options = options;
            algorithmSelector.onValueChanged.AddListener(i =>
            {
                string enumName = enums[i];
                Enum.TryParse<Algorithms>(enumName, out this.algorithm);
                Debug.Log(this.algorithm);
            });
        }

        public void LoadData()
        {
            gridData = SerializeHelper.Load<SquareGridMapData>(assetFilePath);
        }

        private Pathfinder pathfinder;

        private bool autoPathfinding = false;
        private bool drawPassable = true;
        private bool drawBlock = true;
        private bool drawPath = true;
        private bool drawOpenList = true;
        private bool drawClosedList = true;

        private void EnsureFindingContext()
        {
            if (gridData == null)
            {
                LoadData();
            }

            if (gridData == null)
            {
                pathfinder = null;
                throw new Exception($"invalid {nameof(gridData)}");
            }

            if (pathfinder != null && pathfinder.algorithm != this.algorithm)
            {
                pathfinder = null;
            }

            if (pathfinder == null)
            {
                pathfinder = new Pathfinder(gridData, this.algorithm);
            }

            this.view.Visualize(pathfinder);
        }

        public void FindPath()
        {
            EnsureFindingContext();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            path = pathfinder.FindPath(gridData.GetGrid(start.x, start.y), gridData.GetGrid(end.x, end.y));
            this.view.SetDirty();
            long costTime = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log($"costTime {costTime}ms");
        }

        public async void FindPathAsync()
        {
            EnsureFindingContext();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            path = await pathfinder.FindPathAsync(gridData.GetGrid(start.x, start.y), gridData.GetGrid(end.x, end.y));
            this.view.SetDirty();
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

            pathfinder.Init(gridData.GetGrid(start.x, start.y), gridData.GetGrid(end.x, end.y));
            StartCoroutine(InternalFindPathInCoroutine());
        }

        private IEnumerator InternalFindPathInCoroutine()
        {
            path = null;
            WaitForSeconds waitForSeconds = new WaitForSeconds(timeStepForCoroutine);
            while (pathfinder.state != PathfindingState.Found && pathfinder.state != PathfindingState.Failed)
            {
                pathfinder.FindPath();
                this.view.SetDirty();
                yield return waitForSeconds;
            }

            path = pathfinder.GetResult();
            this.view.SetDirty();
        }

        private void Update()
        {
            if (autoPathfinding)
            {
                FindPath();
            }
        }
    }
}
