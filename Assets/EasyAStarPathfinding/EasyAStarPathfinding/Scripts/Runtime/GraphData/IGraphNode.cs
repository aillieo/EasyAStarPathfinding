namespace AillieoUtils.Pathfinding
{
    public interface IGraphNode
    {
        int id { get; }

        int flag { get; }

        float cost { get; }
    }
}
