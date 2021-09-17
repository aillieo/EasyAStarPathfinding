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
    public class UISquareTileSampleBase : MonoBehaviour
    {
        [SerializeField]
        protected UISquareTileView view;
        [SerializeField]
        protected UISquareTileSettingsView settingsView;
        [SerializeField]
        [HideInInspector]
        protected string assetFilePath;

        protected Vector2Int start = new Vector2Int(0, 0);
        protected Vector2Int end = new Vector2Int(19, 19);
        protected Algorithms algorithm;

        [SerializeField]
        protected Dropdown algorithmSelector;
        [SerializeField]
        protected Slider stepIntervalSlider;
        [SerializeField]
        protected Toggle autoPathfinding;
        [SerializeField]
        protected Toggle showSettingsView;

        protected SquareTileMapData tileData;
        protected IEnumerable<AillieoUtils.Pathfinding.Tile> path;

        protected virtual void Awake()
        {
            ConfigDropdowns();
            ConfigToggles();
        }

        protected virtual void OnEnable()
        {
            UISquareTileCtrl.Instance.setStartDelegate += SetStartPoint;
            UISquareTileCtrl.Instance.setTargetDelegate += SetTargetPoint;
        }

        protected virtual void OnDisable()
        {
            UISquareTileCtrl.Instance.setStartDelegate -= SetStartPoint;
            UISquareTileCtrl.Instance.setTargetDelegate -= SetTargetPoint;
        }

        protected void SetStartPoint(int x, int y)
        {
            this.start = new Vector2Int(x, y);
        }

        protected void SetTargetPoint(int x, int y)
        {
            this.end = new Vector2Int(x, y);
        }

        private void ConfigDropdowns()
        {
            string[] algorithms = typeof(Algorithms).GetEnumNames();
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

        private void ConfigToggles()
        {
            showSettingsView.onValueChanged.AddListener(isOn => settingsView.gameObject.SetActive(isOn));
            settingsView.gameObject.SetActive(showSettingsView.isOn);
        }

        public void LoadData()
        {
            tileData = SerializeHelper.Load<SquareTileMapData>(assetFilePath);
        }
    }
}
