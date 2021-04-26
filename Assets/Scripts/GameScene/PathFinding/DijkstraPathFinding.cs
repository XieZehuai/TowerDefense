using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 单独计算每个起始点到目标点的最短路径，起始点较少时效率较高
    /// </summary>
    public class DijkstraPathFinding : FlowFieldPathFinding
    {
        public override string Name => "迪杰斯特拉算法";

        public override bool FindPaths(Vector2Int[] startPositions, Vector2Int endPosition, ref List<Vector2Int>[] paths)
        {
            for (int i = 0; i < startPositions.Length; i++)
            {
                if (!FindPath(startPositions[i], endPosition, ref paths[i]))
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual bool FindPath(Vector2Int startPos, Vector2Int endPos, ref List<Vector2Int> path)
        {
            PriorityQueue<PathNode> openList = new PriorityQueue<PathNode>(new DijkstraPrioirtyComparer()); // 保存所有待寻路的节点（可用优先队列保存）
            HashSet<PathNode> closeList = new HashSet<PathNode>(); // 保存所有已经寻路过的节点

            int endIndex = GetIndex(endPos);
            PathNode start = nodes[GetIndex(startPos)];
            start.costG = 0;
            nodes[GetIndex(startPos)] = start;

            openList.Add(start);
            while (openList.Count > 0)
            {
                PathNode currentNode = openList.DeleteMax();
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
