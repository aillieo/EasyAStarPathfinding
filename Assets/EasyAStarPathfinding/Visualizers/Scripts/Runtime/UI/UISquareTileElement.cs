using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareTileElement : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int x { get; private set; }
        public int y { get; private set; }

        [SerializeField]
        private Image image;
        [SerializeField]
        private Text label;

        private PathfindingContext<Tile> cachedInstance;

        public void Init(int x, int y, PathfindingContext<Tile> context)
        {
            this.x = x;
            this.y = y;
            this.cachedInstance = context;

            (this.transform as RectTransform).anchoredPosition = new Vector2((x + 0.5f) * 60f, (y + 0.5f) * 60f);
            float cost = (cachedInstance.graphData as SquareTileMapData).GetCost(x, y);
            SetColorByCost(cost);
        }

        private void SetColorByCost(float cost)
        {
            this.image.color = new Color(cost, cost, cost, 1f);
            this.label.color = new Color(1 - cost, 1 - cost, 1 - cost, 1f);
        }

        private float dragBeginValue;

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            SquareTileMapData sData = cachedInstance.graphData as SquareTileMapData;
            float cost = sData.GetCost(x, y);

            dragBeginValue = cost;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.position - eventData.pressPosition;
            float dy = delta.y;
            float cost = dragBeginValue + dy * 0.01f;

            cost = Mathf.Clamp01(cost);

            SquareTileMapData sData = cachedInstance.graphData as SquareTileMapData;
            sData.SetCost(x, y, cost);
            SetColorByCost(cost);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}
