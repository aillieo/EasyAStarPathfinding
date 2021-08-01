using AillieoUtils.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareGridView : MonoBehaviour, IVisualizer<Grid>
    {
        [SerializeField]
        private UISquareGridElement template;

        [SerializeField]
        private SimpleUILine uiPath;

        private UISquareGridElement[,] grids;
        private PathfindingContext<Grid> cachedContext;
        private Pathfinder cachedPathfinder;

        private List<Grid> pathFound = new List<Grid>();

        private bool dirty = true;
        private HashSet<Vector2Int> dirtyElements = new HashSet<Vector2Int>();

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        public void Visualize(Pathfinder pathfinder)
        {
            cachedPathfinder = pathfinder;
            cachedContext = (pathfinder.solver as AStar<Grid>).context;
            dirty = true;
        }

        public void SetDirty()
        {
            dirty = true;
        }

        public void SetGridDirty(int x, int y)
        {
            dirtyElements.Add(new Vector2Int(x, y));
            dirty = true;
        }

        private void LateUpdate()
        {
            if (!dirty)
            {
                return;
            }

            if (cachedContext != null)
            {
                UpdateElements();
                UpdatePath();
                dirty = false;
            }
        }

        private void UpdateElements()
        {
            IGraphData<Grid> data = cachedContext.graphData;
            SquareGridMapData sData = data as SquareGridMapData;
            if (grids == null || grids.Length == 0)
            {
                grids = new UISquareGridElement[sData.RangeX, sData.RangeY];
                for (int i = 0; i < sData.RangeX; ++i)
                {
                    for (int j = 0; j < sData.RangeY; ++j)
                    {
                        UISquareGridElement grid = Instantiate<UISquareGridElement>(template, this.transform);
                        grid.Init(i, j, cachedContext);
                        grids[i, j] = grid;
                    }
                }
            }
            else
            {
                foreach (var cord in dirtyElements)
                {
                    UISquareGridElement grid = this.grids[cord.x, cord.y];
                    //
                }
                dirtyElements.Clear();
            }
        }

        private void UpdatePath()
        {
            pathFound.Clear();
            if (cachedPathfinder != null)
            {
                if (cachedPathfinder.state == PathfindingState.Found)
                {
                    cachedPathfinder.GetResult(pathFound);

                    uiPath.RemoveAllPoints();
                    foreach (var g in pathFound)
                    {
                        UISquareGridElement ele = this.grids[g.x, g.y];
                        Vector2 pos = ele.rectTransform.anchoredPosition / (this.transform as RectTransform).rect.width;
                        uiPath.AddPoint(pos);
                    }
                }
            }
        }
    }
}
