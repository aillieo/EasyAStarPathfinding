namespace AillieoUtils.Pathfinding
{
    public interface IGridData : IGraphData
    {
        bool Passable(int x, int y);
    }
}
