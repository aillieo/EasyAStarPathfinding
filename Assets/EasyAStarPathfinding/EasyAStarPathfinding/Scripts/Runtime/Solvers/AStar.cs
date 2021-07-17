namespace AillieoUtils.Pathfinding
{
    public class AStar : ISolver
    {
        public void CleanUp()
        {
        }

        public void Init()
        {
        }

        public PathfindingState Step()
        {
            return PathfindingState.Finding;
        }
    }
}
