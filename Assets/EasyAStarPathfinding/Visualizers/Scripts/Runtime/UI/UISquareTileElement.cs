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

        private IPathfindingContext<Tile> cachedInstance;

        public void Init(int x, int y, IPathfindingContext<Tile> context)
        {
            this.x = x;
            this.y = y;
            this.cachedInstance = context;

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
            SquareTileMapData sData = cachedInstance.GetGraphData() as SquareTileMapData;
            float cost = sData.GetCost(x, y);

            dragBeginValue = cost;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.position - eventData.pressPosition;
            float dy = delta.y;
            float cost = dragBeginValue + dy * 0.01f;

            cost = Mathf.Clamp01(cost);

            SquareTileMapData sData = cachedInstance.GetGraphData() as SquareTileMapData;
            sData.SetCost(x, y, cost);

            UpdateView();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SquareTileMapData sData = cachedInstance.GetGraphData() as SquareTileMapData;
            float cost = sData.GetCost(x, y);
            cost = 1 - cost;
            sData.SetCost(x, y, cost);
            UpdateView();
        }

        public void UpdateView()
        {
            SquareTileMapData sData = cachedInstance.GetGraphData() as SquareTileMapData;

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
                        if (cachedInstance.TryGetOpenNode(tile) != null)
                        {
                            c = 0.75f;
                        }
                        else if (cachedInstance.TryGetClosedNode(tile) != null)
                        {
                            c = 1.0f;
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
            SquareTileMapData sData = cachedInstance.GetGraphData() as SquareTileMapData;
            g = 0;
            h = 0;
            Tile tile = sData.GetTile(x, y);
            NodePointer<Tile> openNode = cachedInstance.TryGetOpenNode(tile);
            if (openNode != null)
            {
                g = openNode.g;
                h = openNode.h;
            }
            else
            {
                NodePointer<Tile> closedNode = cachedInstance.TryGetClosedNode(tile);
                if (closedNode != null)
                {
                    g = closedNode.g;
                    h = closedNode.h;
                }
            }
        }
    }
}
