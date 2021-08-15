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

            UpdateView();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void UpdateView()
        {
            SquareTileMapData sData = cachedInstance.graphData as SquareTileMapData;
            float cost = sData.GetCost(x, y);
            SetColorByCost(cost);

            float g = 0;
            float h = 0;
            if (cachedInstance.openSet.TryGetValue(sData.GetTile(x, y), out NodePointer<Tile> node))
            {
                g = node.g;
                h = node.h;
            }
            else if(cachedInstance.openSet.TryGetValue(sData.GetTile(x, y), out NodePointer<Tile> node1))
            {
                g = node1.g;
                h = node1.h;
            }

            this.label.text = $"{g:f2}\n{h:f2}";
        }
    }
}
