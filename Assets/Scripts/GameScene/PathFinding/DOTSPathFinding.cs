using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TowerDefense
{
    public class DOTSPathFinding : IPathFindingStrategy
    {
        private int width;
        private int height;
        private NativeArray<PathNode> mapData;

        public void SetMapData(MapObject[,] datas)
        {
            width = datas.GetLength(0);
            height = datas.GetLength(1);
            mapData = new NativeArray<PathNode>(width * height, Allocator.TempJob);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PathNode node = new PathNode
                    {
                        x = x,
                        y = y,
                        index = x * height + y,
                        costG = int.MaxValue,
                        isWalkable = datas[x, y].IsWalkable(),
                        parentIndex = -1,
                    };

                    mapData[node.index] = node;
                }
            }
        }

        public bool FindPaths(Vector2Int[] startPositions, Vector2Int endPosition, ref List<Vector2Int>[] paths)
        {
            int startPosCount = startPositions.Length;

            NativeArray<bool>[] results = new NativeArray<bool>[startPosCount];
            NativeList<int2>[] tempPaths = new NativeList<int2>[startPosCount];
            NativeArray<PathNode>[] mapDatas = new NativeArray<PathNode>[startPosCount];
            for (int i = 0; i < startPosCount; i++)
            {
                results[i] = new NativeArray<bool>(1, Allocator.TempJob);
                tempPaths[i] = new NativeList<int2>(Allocator.TempJob);
                mapDatas[i] = new NativeArray<PathNode>(mapData, Allocator.TempJob);
            }

            NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(startPosCount, Allocator.TempJob);

            int2 endPos = new int2(endPosition.x, endPosition.y);
            for (int i = 0; i < startPosCount; i++)
            {
                FindPathJob findPathJob = new FindPathJob
                {
                    width = width,
                    height = height,
                    startPos = new int2(startPositions[i].x, startPositions[i].y),
                    endPos = endPos,
                    nodes = mapDatas[i],
                    result = results[i],
                    path = tempPaths[i],
                };

                jobHandles[i] = findPathJob.Schedule();
            }

            JobHandle.CompleteAll(jobHandles);

            bool success = true;
            for (int i = 0; i < startPosCount; i++)
            {
                if (success && !results[i][0])
                {
                    success = false;
                }

                GetPath(ref paths[i], tempPaths[i]);

                results[i].Dispose();
                tempPaths[i].Dispose();
                mapDatas[i].Dispose();
            }

            jobHandles.Dispose();
            mapData.Dispose();

            return success;
        }

        private void GetPath(ref List<Vector2Int> path, NativeList<int2> tempPath)
        {
            int len = tempPath.Length;
            for (int i = 0; i < len; i++)
            {
                int2 pos = tempPath[len - i - 1];
                path.Add(new Vector2Int(pos.x, pos.y));
            }
        }


        [BurstCompile]
        private struct FindPathJob : IJob
        {
            public int width;
            public int height;
            public int2 startPos;
            public int2 endPos;
            public NativeArray<PathNode> nodes;

            // 用于保存寻路结果
            public NativeArray<bool> result;
            public NativeList<int2> path;

            public void Execute()
            {
                PathNode start = nodes[GetIndex(startPos.x, startPos.y)];
                start.costG = 0;
                nodes[start.index] = start;
                int endIndex = GetIndex(endPos.x, endPos.y);

                NativeList<int> openList = new NativeList<int>(Allocator.Temp);
                NativeList<int> closeList = new NativeList<int>(Allocator.Temp);

                openList.Add(start.index);
                while (openList.Length > 0)
                {
                    int currIndex = openList[0];
                    for (int i = 1; i < openList.Length; i++)
                    {
                        if (nodes[openList[i]].CostF < nodes[currIndex].CostF)
                        {
                            currIndex = openList[i];
                        }
                    }

                    // 抵达终点
                    if (currIndex == endIndex) break;

                    PathNode currNode = nodes[currIndex];

                    // 把当前结点从open list移除
                    for (int i = 0; i < openList.Length; i++)
                    {
                        if (openList[i] == currIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }

                    closeList.Add(currIndex);

                    NativeList<PathNode> neighbours = GetNeighbours(nodes, currNode);
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        PathNode neighbourNode = neighbours[i];
                        if (closeList.Contains(neighbourNode.index)) continue; // 已经加入关闭列表

                        int g = currNode.costG + GetDistance(new int2(currNode.x, currNode.y), new int2(neighbourNode.x, neighbourNode.y));

                        bool hasSearch = openList.Contains(neighbourNode.index);
                        if (g < neighbourNode.costG || !hasSearch)
                        {
                            neighbourNode.parentIndex = currIndex;
                            neighbourNode.costG = g;
                            neighbourNode.costH = GetDistance(new int2(neighbourNode.x, neighbourNode.y), endPos);
                            nodes[neighbourNode.index] = neighbourNode;

                            if (!hasSearch)
                            {
                                openList.Add(neighbourNode.index);
                            }
                        }
                    }

                    neighbours.Dispose();
                }

                PathNode endNode = nodes[endIndex];
                if (endNode.parentIndex == -1)
                {
                    result[0] = false;
                }
                else
                {
                    result[0] = true;

                    PathNode temp = endNode;
                    path.Add(new int2(temp.x, temp.y));

                    while (temp.parentIndex != -1)
                    {
                        temp = nodes[temp.parentIndex];
                        path.Add(new int2(temp.x, temp.y));
                    }
                }
            }

            private NativeList<PathNode> GetNeighbours(NativeArray<PathNode> nodes, PathNode node)
            {
                NativeList<PathNode> neighbours = new NativeList<PathNode>(Allocator.Temp);

                int x = node.x;
                int y = node.y;

                bool left = IsNeighbourWalkable(nodes, x - 1, y);
                bool right = IsNeighbourWalkable(nodes, x + 1, y);
                bool up = IsNeighbourWalkable(nodes, x, y + 1);
                bool down = IsNeighbourWalkable(nodes, x, y - 1);

                bool leftUp = IsNeighbourWalkable(nodes, x - 1, y + 1);
                bool rightUp = IsNeighbourWalkable(nodes, x + 1, y + 1);
                bool leftDown = IsNeighbourWalkable(nodes, x - 1, y - 1);
                bool rightDown = IsNeighbourWalkable(nodes, x + 1, y - 1);

                if (left) neighbours.Add(nodes[GetIndex(x - 1, y)]);
                if (right) neighbours.Add(nodes[GetIndex(x + 1, y)]);
                if (up) neighbours.Add(nodes[GetIndex(x, y + 1)]);
                if (down) neighbours.Add(nodes[GetIndex(x, y - 1)]);

                if (left && up && leftUp) neighbours.Add(nodes[GetIndex(x - 1, y + 1)]);
                if (right && up && rightUp) neighbours.Add(nodes[GetIndex(x + 1, y + 1)]);
                if (left && down && leftDown) neighbours.Add(nodes[GetIndex(x - 1, y - 1)]);
                if (right && down && rightDown) neighbours.Add(nodes[GetIndex(x + 1, y - 1)]);

                return neighbours;
            }

            private bool IsNeighbourWalkable(NativeArray<PathNode> nodes, int x, int y)
            {
                if (!IsValidPos(new int2(x, y))) return false;

                return nodes[GetIndex(x, y)].isWalkable;
            }

            private int GetIndex(int x, int y)
            {
                return x * height + y;
            }

            private int GetDistance(int2 a, int2 b)
            {
                int h = math.abs(a.x - b.x);
                int v = math.abs(a.y - b.y);
                int remain = math.abs(h - v);
                return 14 * math.min(h, v) + 10 * remain;
            }

            private bool IsValidPos(int2 pos)
            {
                return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
            }
        }
    }
}
