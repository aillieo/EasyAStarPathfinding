using System;
using System.Collections.Generic;

namespace AillieoUtils
{
    public delegate IEnumerable<Point> NeighborCollectingFunc(Point current, IGridDataProvider gridDataProvider);

    public static class NeighborCollectingFuncPreset
    {
        public static IEnumerable<Point> DefaultNeighborCollectingFunc(Point current, IGridDataProvider gridDataProvider)
        {
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    if (i != 0 && j != 0)
                    {
                        // ½ûÖ¹Ð±×Å×ß
                        continue;
                    }

                    int x = current.x + i;
                    int y = current.y + j;

                    // Debug.LogError($" TESTING  x={x} y= {y}");

                    if (!gridDataProvider.Passable(x, y))
                    {
                        continue;
                    }

                    yield return new Point(x, y);
                }
            }
        }
    }
}
