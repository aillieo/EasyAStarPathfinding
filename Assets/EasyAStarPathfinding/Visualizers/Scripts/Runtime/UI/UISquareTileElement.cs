using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareTileElement : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public int x { get; private set; }
        public int y { get; private set; }

        [SerializeField]
        private Image image;
        [SerializeField]
        private Text label;

        private IPathfindingContext<Tile, INodeWrapper<Tile>> cachedContext;

        public void Init(int x, int y, IPathfindingContext<Tile, INodeWrapper<Tile>> context)
        {
            this.x = x;
            this.y = y;
            this.cachedContext = context;

            (this.transform as RectTransform).anchoredPosition = new Vector2((x + 0.5f) * 60f, (y + 0.5f) * 60f);
        }

        private void SetColor(Color clr)
        {
            this.image.color = clr;
            this.label.color = new Color(1 - clr.r, 1 - clr.g, 1 - clr.b, 1f);
        }

        private float dragBeginValue;

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            SquareTileMapData sData = cachedContext.GetGraphData() as SquareTileMapData;
            dragBeginValue = sData.GetCost(x, y);
        }

        public void OnDrag(PointerEventData eventData)
        {
            switch (UISquareTileCtrl.Instance.opMode)
            {
                case UISquareTileCtrl.OperationMode.ModifyCost:
                    Vector2 delta = eventData.position - eventData.pressPosition;
                    float dy = delta.y;
                    float cost = dragBeginValue + dy * 0.01f;

                    cost = Mathf.Clamp01(cost);

                    UISquareTileCtrl.Instance.ModifyTileCost(x, y, cost);
                    break;
                case UISquareTileCtrl.OperationMode.ClickToSetStart:
                case UISquareTileCtrl.OperationMode.ClickToSetTarget:
                    break;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (UISquareTileCtrl.Instance.opMode)
            {
                case UISquareTileCtrl.OperationMode.ClickToSetStart:
                    UISquareTileCtrl.Instance.setStartDelegate?.Invoke(x, y);
                    break;
                case UISquareTileCtrl.OperationMode.ClickToSetTarget:
                    UISquareTileCtrl.Instance.setTargetDelegate?.Invoke(x, y);
                    break;
                case UISquareTileCtrl.OperationMode.ModifyCost:
                    SquareTileMapData sData = cachedContext.GetGraphData() as SquareTileMapData;
                    float cost = sData.GetCost(x, y);
                    cost = 1 - cost;
                    UISquareTileCtrl.Instance.ModifyTileCost(x, y, cost);
                    break;
            }
        }

        public void UpdateView()
        {
            SquareTileMapData sData = cachedContext.GetGraphData() as SquareTileMapData;

            switch (UISquareTileCtrl.Instance.colorMode)
            {
                case UISquareTileCtrl.ColorMode.TileCost:
                    float cost = sData.GetCost(x, y);
                    SetColor(new Color(cost, cost, cost));
                    break;
                case UISquareTileCtrl.ColorMode.GHValue:
                    {
                        GetGH(x, y, out float g, out float h);
                        float c = Mathf.Atan(g + h);
                        SetColor(new Color(c, c, c));
                    }
                    break;
                case UISquareTileCtrl.ColorMode.OpenOrClose:
                    {
                        float c = 0f;
                        Tile tile = sData.GetTile(x, y);
                        NodeWrapper<Tile> nodeWrapper = cachedContext.TryGetNode(tile) as NodeWrapper<Tile>;
                        if (nodeWrapper != null)
                        {
                            if (nodeWrapper.state == NodeState.Open)
                            {
                                c = 0.75f;
                            }
                            else
                            {
                                c = 1.0f;
                            }
                        }

                        SetColor(new Color(c, c, c));
                    }
                    break;
                case UISquareTileCtrl.ColorMode.GValue:
                    {
                        GetGH(x, y, out float g, out float h);
                        float c = Mathf.Atan(g);
                        SetColor(new Color(c, c, c));
                    }
                    break;
                case UISquareTileCtrl.ColorMode.NodeConsistency:
                    {
                        float c = 0f;
                        Tile tile = sData.GetTile(x, y);
                        NodeWrapperEx<Tile> nodeWrapper = cachedContext.TryGetNode(tile) as NodeWrapperEx<Tile>;
                        if (nodeWrapper != null)
                        {
                            if (nodeWrapper.GetConsistency() == NodeConsistency.Consistent)
                            {
                                c = 0.75f;
                            }
                            else if (nodeWrapper.GetConsistency() == NodeConsistency.Overconsistent)
                            {
                                c = 1.0f;
                            }
                        }

                        SetColor(new Color(c, c, c));
                    }
                    break;
            }

            switch (UISquareTileCtrl.Instance.textMode)
            {
                case UISquareTileCtrl.TextMode.TileCost:
                    float cost = sData.GetCost(x, y);
                    this.label.text = $"{cost:f2}";
                    break;
                case UISquareTileCtrl.TextMode.GHValue:
                    GetGH(x, y, out float g, out float h);
                    this.label.text = $"{g:f2}\n{h:f2}";
                    break;
            }
        }

        private void GetGH(int x, int y, out float g, out float h)
        {
            SquareTileMapData sData = cachedContext.GetGraphData() as SquareTileMapData;
            g = 0;
            h = 0;
            Tile tile = sData.GetTile(x, y);

            switch (cachedContext.TryGetNode(tile))
            {
                case NodeWrapper<Tile> nodeWrapper:
                    g = nodeWrapper.g;
                    h = nodeWrapper.h;
                    break;
                case NodeWrapperEx<Tile> nodeWrapperEx:
                    g = nodeWrapperEx.g;
                    h = nodeWrapperEx.h;
                    break;
            }
        }
    }
}
