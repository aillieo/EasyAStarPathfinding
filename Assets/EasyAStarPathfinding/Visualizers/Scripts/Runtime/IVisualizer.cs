namespace AillieoUtils.Pathfinding.Visualizers
{
    public interface IVisualizer<T> where T : IGraphNode
    {
        void Visualize(Pathfinder pathfinder);
    }
}
