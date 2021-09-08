using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils.Pathfinding
{
    public enum NodeConsistency
    {
        LocallyConsistent = 0,
        LocallyOverconsistent = 1,
        LocallyUnderconsistent = -1
    }
}
