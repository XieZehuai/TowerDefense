using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class AStarPathFinding : DijkstraPathFinding
    {
        public override string Name => "A星算法";

        protected override bool FindPath(Vector2Int startPos, Vector2Int endPos, ref List<Vector2Int> path)
        {
            List<PathNode> openList = new List<PathNode>();
            HashSet<PathNode> closeList = new HashSet<PathNode>();

            int endIndex = GetIndex(endPos);
            PathNode start = nodes[GetIndex(startPos)];
            start.costG = 0;
            start.costH = GetDistance(startPos, endPos);
            nodes[GetIndex(startPos)] = start;

            openList.Add(start);
            while (openList.Count > 0)
            {
                PathNode currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].CostF < currentNode.CostF)
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closeList.Add(currentNode);

                if (currentNode.index == endIndex) break;

                List<PathNode> neighbours = GetNeighbour(currentNode);
                for (int i = 0; i < neighbours.Count; i++)
                {
                    PathNode neighbourNode = neighbours[i];

                    if (closeList.Contains(neighbourNode)) continue;

                    int costG = currentNode.costG + GetDistance(currentNode, neighbourNode);

                    if (costG < neighbourNode.costG)
                    {
                        neighbourNode.costG = costG;
                        neighbourNode.costH = GetDistance(neighbourNode, nodes[endIndex]);
                        neighbourNode.parentIndex = currentNode.index;
                        nodes[neighbourNode.index] = neighbourNode;

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }

            PathNode endNode = nodes[endIndex];
            if (endNode.parentIndex == -1)
            {
                ResetMapData();
                return false;
            }

            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            stack.Push(endPos);

            PathNode temp = endNode;
            while (temp.parentIndex != -1)
            {
                temp = nodes[temp.parentIndex];
                stack.Push(new Vector2Int(temp.x, temp.y));
            }

            path = stack.ToList();

            ResetMapData();
            return true;
        }
    }
}
