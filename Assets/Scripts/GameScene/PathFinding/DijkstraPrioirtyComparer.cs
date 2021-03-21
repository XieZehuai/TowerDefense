namespace TowerDefense
{
    public class DijkstraPrioirtyComparer : IPriorityComparer<PathNode>
    {
        public int Compare(PathNode a, PathNode b)
        {
            return b.costG - a.costG;
        }
    }
}
