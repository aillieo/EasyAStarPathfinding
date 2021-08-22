using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding.Visualizers
{
    public class UISquareTileCtrl
    {
        public enum ColorMode
        {
            TileCost = 0,
            OpenOrClose = 1,
            GValue = 2,
            GHValue = 3,
        }

        public enum TextMode
        {
            TileCost = 0,
            GHValue = 1,
        }

        public enum OperationMode
        {
            ModifyCost = 0,
            ClickToSetStart = 1,
            ClickToSetTarget = 2,
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
        public OperationMode opMode = OperationMode.ModifyCost;
    }
}
