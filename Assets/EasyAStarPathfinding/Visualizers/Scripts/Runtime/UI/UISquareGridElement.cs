using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareGridElement : MaskableGraphic, IPointerClickHandler
    {
        public int x { get; private set; }
        public int y { get; private set; }

        private PathfindingContext<Grid> cachedInstance;

        public void Init(int x, int y, PathfindingContext<Grid> context)
        {
            this.x = x;
            this.y = y;
            this.cachedInstance = context;

            (this.transform as RectTransform).anchoredPosition = new Vector2(x * 22f, y * 22f);
            SetColorByCost((cachedInstance.graphData as SquareGridMapData).GetCost(x, y));
        }

        private void SetColorByCost(float cost)
        {
            this.color = new Color(cost, cost, cost, 1f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SquareGridMapData sData = cachedInstance.graphData as SquareGridMapData;
            float cost = sData.GetCost(x, y);
            cost += 0.25f;
            while(cost > 1)
            {
                cost -= 1;
            }

            sData.SetCost(x, y, cost);
            SetColorByCost(cost);
        }
    }
}
