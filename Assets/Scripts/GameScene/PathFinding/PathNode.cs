namespace TowerDefense
{
    public struct PathNode
    {
        public int x;
        public int y;
        public int index;

        public int costG;
        public int costH;

        public bool isWalkable;
        public int parentIndex;

        public int CostF => costG = costH;

        public override int GetHashCode()
        {
            return x << 16 + y;
        }
    }
}
