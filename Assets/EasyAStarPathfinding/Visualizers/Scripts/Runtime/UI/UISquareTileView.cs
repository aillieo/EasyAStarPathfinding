using AillieoUtils.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareTileView : MonoBehaviour, IVisualizer<Tile>
    {
        [SerializeField]
        private UISquareTileElement template;

        [SerializeField]
        private RectTransform tilesRoot;

        [SerializeField]
        private SimpleUILine uiPath;

        [SerializeField]
        private RectTransform uiAgent;

        private UISquareTileElement[,] tiles;
        private IPathfindingContext<Tile, INodeWrapper<Tile>> cachedContext;
        private Pathfinder cachedPathfinder;

        private List<Tile> pathFound = new List<Tile>();

        private bool dirty = true;
        private HashSet<Vector2Int> dirtyElements = new HashSet<Vector2Int>();

        private void OnEnable()
        {
            UISquareTileCtrl.Instance.modifyCostDelegate += this.UpdateTileCost;
        }

        private void OnDisable()
        {
            UISquareTileCtrl.Instance.modifyCostDelegate -= this.UpdateTileCost;
        }

        public void Visualize(Pathfinder pathfinder)
        {
            if (cachedPathfinder != pathfinder || cachedContext == null)
            {
                cachedPathfinder = pathfinder;
                if (pathfinder.solver is AStar<Tile> astar)
                {
                    cachedContext = astar.context;
                }
                else if (pathfinder.solver is LPAStar<Tile> lpa)
                {
                    cachedContext = lpa.context;
                }
                else if (pathfinder.solver is DStarLite<Tile> dsl)
                {
                    cachedContext = dsl.context;
                }
                else if (pathfinder.solver is ThetaStar<Tile> theta)
                {
                    cachedContext = theta.context;
                }
            }

            dirty = true;
        }

        public void SetDirty()
        {
            if (cachedContext == null)
            {
                return;
            }

            IGraphData<Tile> data = cachedContext.GetGraphData();
            SquareTileMapData sData = data as SquareTileMapData;
            for (int i = 0; i < sData.RangeX; ++i)
            {
                for (int j = 0; j < sData.RangeY; ++j)
                {
                    dirtyElements.Add(new Vector2Int(i, j));
                }
            }

            dirty = true;
        }

        public void SetTileDirty(int x, int y)
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
                UpdateAgent();
                dirty = false;
            }
        }

        private void UpdateElements()
        {
            IGraphData<Tile> data = cachedContext.GetGraphData();
            SquareTileMapData sData = data as SquareTileMapData;
            if (tiles == null || tiles.Length == 0)
            {
                tiles = new UISquareTileElement[sData.RangeX, sData.RangeY];
                for (int i = 0; i < sData.RangeX; ++i)
                {
                    for (int j = 0; j < sData.RangeY; ++j)
                    {
                        UISquareTileElement tile = Instantiate<UISquareTileElement>(template, this.tilesRoot);
                        tile.Init(i, j, cachedContext);
                        (tile.transform as RectTransform).anchoredPosition = new Vector2((i + 0.5f) * 60f, (j + 0.5f) * 60f);
                        tiles[i, j] = tile;
                        tile.UpdateView();
                    }
                }
            }
            else
            {
                foreach (var cord in dirtyElements)
                {
                    UISquareTileElement tile = this.tiles[cord.x, cord.y];
                    tile.UpdateView();
                }
                dirtyElements.Clear();
            }
        }

        private void UpdatePath()
        {
            //pathFound.Clear();
            if (cachedPathfinder != null)
            {
                uiPath.RemoveAllPoints();

                if (cachedPathfinder.state == PathfindingState.Found)
                {
                    cachedPathfinder.GetResult(pathFound);

                    Vector2 rectSize = (this.transform as RectTransform).rect.size;
                    foreach (var g in pathFound)
                    {
                        UISquareTileElement ele = this.tiles[g.x, g.y];
                        RectTransform eleRect = ele.transform as RectTransform;
                        Vector2 elePos = eleRect.anchoredPosition;
                        Vector2 eleSize = eleRect.rect.size;
                        Vector2 pos = new Vector2((elePos.x - eleSize.x / 2) / rectSize.x, (elePos.y - eleSize.y / 2) / rectSize.y);
                        uiPath.AddPoint(pos);
                    }

                    uiPath.rectTransform.anchoredPosition = (tiles[0, 0].transform as RectTransform).anchoredPosition;
                    //uiPath.rectTransform.Strentch();
                }
            }
        }

        private void UpdateAgent()
        {
            if (cachedPathfinder.solver is DStarLite<Tile> dStarLite)
            {
                Tile curPos = dStarLite.currentNode;
                if (curPos != null)
                {
                    float x = curPos.x;
                    float y = curPos.y;
                    var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this.transform, tiles[curPos.x, curPos.y].transform);
                    (this.uiAgent.transform as RectTransform).anchoredPosition = new Vector2(bounds.center.x, bounds.center.y);
                }
            }
        }

        private void UpdateTileCost(int x, int y, float cost)
        {
            IGraphData<Tile> data = cachedContext.GetGraphData();
            SquareTileMapData sData = data as SquareTileMapData;
            sData.SetCost(x, y, cost);
            SetTileDirty(x, y);
        }

        public RectTransform GetUIAgent()
        {
            return uiAgent;
        }
    }
}
