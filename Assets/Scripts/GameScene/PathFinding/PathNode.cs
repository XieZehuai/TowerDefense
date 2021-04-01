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

        public int CostF => costG + costH;

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                PathNode p = (PathNode)obj;
                return (x == p.x) && (y == p.y);
            }
        }

        public override int GetHashCode()
        {
            return x << 16 + y;
        }
    }
}
