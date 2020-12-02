using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AillieoUtils;

public class PathFindingTest : MonoBehaviour
{
    public TestGridData mapData;
    public Vector2Int start = new Vector2Int(1,1);
    public Vector2Int end = new Vector2Int(8, 8);

    private IEnumerable<Point> path;
    private PathFinder pathFinder;

    private void Start()
    {
        FindPath();
    }

    [ContextMenu("Find")]
    void FindPath()
    {
        if(mapData == null)
        {
            return;
        }

        if(pathFinder == null)
        {
            pathFinder = new PathFinder(mapData);
        }

        path = pathFinder.FindPath(new Point(start.x, start.y), new Point(end.x,end.y));
    }

    private void OnDrawGizmos()
    {
        if (mapData != null)
        {
            for (int i = 0; i < mapData.rangeX; ++i)
            {
                for (int j = 0; j < mapData.rangeY; ++j)
                {
                    Gizmos.DrawSphere(new Vector3(i, j, 0), 0.25f);
                }
            }
        }

        Color backup = Gizmos.color;
        if (path != null)
        {
            int count = path.Count();
            int index = 0;

            foreach (var pp in path)
            {
                Gizmos.color = Color.Lerp(Color.red, Color.blue, ((float)(index++)) / count);
                Gizmos.DrawSphere(new Vector3(pp.x, pp.y, 0), 0.4f);
            }
        }

        Gizmos.color = Color.black;
        foreach (var o in mapData.Obstacles())
        {
            Gizmos.DrawSphere(new Vector3(o.x, o.y, 0), 0.5f);
        }

        Gizmos.color = backup;
    }
}
