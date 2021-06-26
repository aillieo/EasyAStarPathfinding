using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AillieoUtils.Pathfinding.GraphCreator
{
    public class PolygonComposite : PolygonSimple
    {
        public readonly List<PolygonSimple> holes = new List<PolygonSimple>();

        public override bool Validate()
        {
            if (!base.Validate())
            {
                return false;
            }

            if (holes.Any(h => !Validate()))
            {
                return false;
            }

            // todo 检查各个 hole 是否相交

            // 每个hole 也可能是一个 Composite

            return true;
        }
    }
}
