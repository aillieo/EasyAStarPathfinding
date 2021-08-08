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

namespace Samples
{
    public class SquareTileUISample : MonoBehaviour
    {
        [SerializeField]
        private UISquareTileView view;
        [SerializeField]
        public string assetFilePath;

        private Vector2Int start = new Vector2Int(0, 0);
        private Vector2Int end = new Vector2Int(19, 19);
        private Algorithms algorithm;

        [SerializeField]
        private Dropdown algorithmSelector;
        [SerializeField]
        private Slider stepIntervalSlider;
        [SerializeField]
        private Toggle autoPathfinding;

        private SquareTileMapData tileData;
        private IEnumerable<AillieoUtils.Pathfinding.Tile> path;

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
            tileData = SerializeHelper.Load<SquareTileMapData>(assetFilePath);
        }

        private Pathfinder pathfinder;

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

            this.view.Visualize(pathfinder);
        }

        public void FindPath()
        {
            EnsureFindingContext();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            path = pathfinder.FindPath(tileData.GetTile(start.x, start.y), tileData.GetTile(end.x, end.y));
            this.view.SetDirty();
            long costTime = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log($"costTime {costTime}ms");
        }

        public async void FindPathAsync()
        {
            EnsureFindingContext();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            path = await pathfinder.FindPathAsync(tileData.GetTile(start.x, start.y), tileData.GetTile(end.x, end.y));
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

            pathfinder.Init(tileData.GetTile(start.x, start.y), tileData.GetTile(end.x, end.y));
            StartCoroutine(InternalFindPathInCoroutine());
        }

        private IEnumerator InternalFindPathInCoroutine()
        {
            path = null;
            float timeStepForCoroutine = stepIntervalSlider.value;
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
            if (autoPathfinding.isOn)
            {
                FindPath();
            }
        }
    }
}
