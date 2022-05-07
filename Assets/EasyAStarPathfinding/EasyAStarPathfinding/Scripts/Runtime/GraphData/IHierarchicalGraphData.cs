namespace AillieoUtils.Pathfinding
{
    public interface IHierarchicalGraphData<T> : IGraphData where T : IGraphNode
    {
        int GetLevelCount();

        T GetNodeForLevel(int inputLevel, T inputNode, int targetLevel);

        void ConfigCurrentLevel(int curLevel);
    }
}
