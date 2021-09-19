using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AillieoUtils.Pathfinding;
using System.Diagnostics;
using System;
using UnityEngine.UI;
using AillieoUtils.Pathfinding.Visualizers;

namespace Samples
{
    public class UISquareTileSampleLPA : UISquareTileSampleBase
    {
        protected override void Awake()
        {
            UISquareTileCtrl.Instance.colorMode = UISquareTileCtrl.ColorMode.NodeConsistency;
            base.Awake();
            ConfigDropdowns();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UISquareTileCtrl.Instance.modifyCostDelegate += OnNodeCostModified;
        }

        protected override void OnDisable()
        {
            UISquareTileCtrl.Instance.modifyCostDelegate -= OnNodeCostModified;
            base.OnDisable();
        }

        private void ConfigDropdowns()
        {
            string[] algorithms = new string[] { typeof(Algorithms).GetEnumName(Algorithms.LPAStar) };
            var algorithmOptions = algorithms.Select(s => new Dropdown.OptionData { text = s }).ToList();
            algorithmSelector.options = algorithmOptions;
            algorithmSelector.onValueChanged.AddListener(i =>
            {
                string enumName = algorithms[i];
                Enum.TryParse<Algorithms>(enumName, out this.algorithm);
                //Debug.Log(this.algorithm);
            });
            Enum.TryParse<Algorithms>(algorithms[0], out this.algorithm);
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
            autoPathfinding.isOn = false;

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
            while (pathfinder.state != PathfindingState.Found && pathfinder.state != PathfindingState.Failed)
            {
                pathfinder.FindPath();
                this.view.SetDirty();
                float timeStepForCoroutine = stepIntervalSlider.value;
                yield return new WaitForSeconds(timeStepForCoroutine);
            }

            path = pathfinder.GetResult();
            this.view.SetDirty();
        }

        public void FindPathByStep()
        {
            autoPathfinding.isOn = false;

            StopAllCoroutines();

            EnsureFindingContext();

            switch (pathfinder.state)
            {
                case PathfindingState.Uninitialized:
                    path = null;
                    pathfinder.Init(tileData.GetTile(start.x, start.y), tileData.GetTile(end.x, end.y));
                    break;

                case PathfindingState.Found:
                case PathfindingState.Failed:
                    pathfinder.CleanUp();
                    break;
                case PathfindingState.Finding:
                case PathfindingState.Initialized:
                    pathfinder.FindPath();
                    break;
            }

            this.view.SetDirty();
        }

        private void Update()
        {
            if (autoPathfinding.isOn)
            {
                FindPath();
            }
        }

        private void OnNodeCostModified(int x, int y, float cost)
        {
            Tile tile = tileData.GetTile(x, y);
            pathfinder.NotifyNodeDataModified(tile);
        }
    }
}
