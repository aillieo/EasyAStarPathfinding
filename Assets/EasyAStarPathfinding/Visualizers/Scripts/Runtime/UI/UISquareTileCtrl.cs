using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareTileCtrl
    {
        public delegate void ModifyCostDelegate(int x, int y, float cost);
        public delegate void TileAssignDelegate(int x, int y);

        public ModifyCostDelegate modifyCostDelegate;
        public TileAssignDelegate setStartDelegate;
        public TileAssignDelegate setTargetDelegate;

        public event ModifyCostDelegate onTileCostModified;

        public enum ColorMode
        {
            TileCost = 0,
            OpenOrClose = 1,
            GValue = 2,
            GHValue = 3,
            NodeConsistency = 4,

            SolidWhite = 99,
        }

        public enum TextMode
        {
            TileCost = 0,
            GHValue = 1,
            GHRhsValue = 2,

            NoText = 99,
        }

        public enum OperationMode
        {
            ModifyCost = 0,
            ClickToSetStart = 1,
            ClickToSetTarget = 2,
        }

        public enum CostModificationMode
        {
            RealTime = 0,
            DontAllow = -1,
            Record = 1,
        }

        private static UISquareTileCtrl instance;

        public static UISquareTileCtrl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UISquareTileCtrl();
                }
                return instance;
            }
        }

        public TextMode textMode = TextMode.TileCost;
        public ColorMode colorMode = ColorMode.TileCost;
        public OperationMode opMode = OperationMode.ModifyCost;
        public CostModificationMode costMdfMode = CostModificationMode.RealTime;

        private readonly Dictionary<Vector2Int, float> modificationsCache = new Dictionary<Vector2Int, float>();

        public void ModifyTileCost(int x, int y, float newCost)
        {
            switch (costMdfMode)
            {
                case CostModificationMode.DontAllow:
                    break;
                case CostModificationMode.RealTime:
                    if (modifyCostDelegate != null)
                    {
                        modifyCostDelegate.Invoke(x, y, newCost);
                        onTileCostModified?.Invoke(x, y, newCost);
                    }

                    break;
                case CostModificationMode.Record:
                    modificationsCache[new Vector2Int(x, y)] = newCost;
                    break;
            }
        }

        public void ApplyRecordedTileCostModifications()
        {
            if (modifyCostDelegate != null)
            {
                foreach (var pair in modificationsCache)
                {
                    modifyCostDelegate.Invoke(pair.Key.x, pair.Key.y, pair.Value);
                    onTileCostModified?.Invoke(pair.Key.x, pair.Key.y, pair.Value);
                }
            }

            modificationsCache.Clear();
        }

        public void DiscardRecordedTileCostModifications()
        {
            modificationsCache.Clear();
        }
    }
}
