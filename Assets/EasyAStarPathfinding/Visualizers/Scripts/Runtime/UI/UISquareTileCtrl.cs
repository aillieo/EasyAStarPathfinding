using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareTileCtrl
    {
        public enum ColorMode
        {
            TileCost = 1,
            OpenOrClose = 2,
            GValue = 3,
            GHValue = 4,
        }

        public enum TextMode
        {
            TileCost = 1,
            GHValue = 3,
        }

        private static UISquareTileCtrl instance;

        public static UISquareTileCtrl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UISquareTileCtrl();
                }
                return instance;
            }
        }

        public TextMode textMode = TextMode.TileCost;
        public ColorMode colorMode = ColorMode.TileCost;
    }
}
