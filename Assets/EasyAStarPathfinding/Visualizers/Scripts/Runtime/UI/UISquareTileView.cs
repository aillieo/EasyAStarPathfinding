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
        private Transform tilesRoot;

        [SerializeField]
        private SimpleUILine uiPath;

        private UISquareTileElement[,] tiles;
        private PathfindingContext<Tile> cachedContext;
        private Pathfinder cachedPathfinder;

        private List<Tile> pathFound = new List<Tile>();

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
            cachedContext = (pathfinder.solver as AStar<Tile>).context;
            dirty = true;
        }

        public void SetDirty()
        {
            IGraphData<Tile> data = cachedContext.graphData;
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
                dirty = false;
            }
        }

        private void UpdateElements()
        {
            IGraphData<Tile> data = cachedContext.graphData;
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
                        Vector2 elePos = (ele.transform as RectTransform).anchoredPosition;
                        Vector2 pos = new Vector2(elePos.x / rectSize.x, elePos.y / rectSize.y);
                        uiPath.AddPoint(pos);
                    }

                    uiPath.rectTransform.anchoredPosition = (tiles[0, 0].transform as RectTransform).anchoredPosition;
                    //uiPath.rectTransform.Strentch();
                }
            }
        }
    }
}
