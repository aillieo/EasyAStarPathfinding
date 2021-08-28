using System;

namespace AillieoUtils.Pathfinding
{
    public interface IGraphNode //: IEquatable<T>
    {
        int id { get; }

        int flag { get; }

        float cost { get; }
    }
}
