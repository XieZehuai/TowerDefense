using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace TowerDefense.Test
{
    public class PathFinding : MonoBehaviour
    {
        private float timer = 0f;

        private void Start()
        {
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 0f;
                float startTime = Time.realtimeSinceStartup;

                int findPathJobCount = 50;
                NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.TempJob);

                for (int i = 0; i < 5; i++)
                {
                    FindPathJob findPathJob = new FindPathJob
                    {
                        startPosition = new int2(0, 0),
                        endPosition = new int2(99, 99)
                    };

                    jobHandleArray[i] = findPathJob.Schedule();
                }

                JobHandle.CompleteAll(jobHandleArray);
                jobHandleArray.Dispose();

                Debug.Log("Time: " + (Time.realtimeSinceStartup - startTime) * 1000f);
            }
        }

        private struct PathNode
        {
            public int x;
            public int y;
            public int index;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;

            public int cameFromNodeIndex;

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }
        }

        [BurstCompile]
        private struct FindPathJob : IJob
        {
            public int2 startPosition;
            public int2 endPosition;

            public void Execute()
            {
                int2 gridSize = new int2(100, 100);
                NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp); ;

                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        PathNode pathNode = new PathNode();
                        pathNode.x = x;
                        pathNode.y = y;
                        pathNode.index = CalculateIndex(x, y, gridSize.x);

                        pathNode.gCost = int.MaxValue;
                        pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                        pathNode.CalculateFCost();

                        pathNode.isWalkable = true;
                        pathNode.cameFromNodeIndex = -1;

                        pathNodeArray[pathNode.index] = pathNode;
                    }
                }

                NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
                neighbourOffsetArray[0] = new int2(-1, 0);
                neighbourOffsetArray[1] = new int2(1, 0);
                neighbourOffsetArray[2] = new int2(0, -1);
                neighbourOffsetArray[3] = new int2(0, 1);
                neighbourOffsetArray[4] = new int2(-1, -1);
                neighbourOffsetArray[5] = new int2(-1, 1);
                neighbourOffsetArray[6] = new int2(1, -1);
                neighbourOffsetArray[7] = new int2(1, 1);

                int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

                PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
                startNode.gCost = 0;
                startNode.CalculateFCost();
                pathNodeArray[startNode.index] = startNode;

                NativeList<int> openList = new NativeList<int>(Allocator.Temp);
                NativeList<int> closeList = new NativeList<int>(Allocator.Temp);

                openList.Add(startNode.index);
                while (openList.Length > 0)
                {
                    int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray
                        );
                    PathNode currentNode = pathNodeArray[currentNodeIndex];

                    if (currentNodeIndex == endNodeIndex)
                    {
                        break;
                    }

                    for (int i = 0; i < openList.Length; i++)
                    {
                        if (openList[i] == currentNodeIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }

                    closeList.Add(currentNodeIndex);

                    for (int i = 0; i < neighbourOffsetArray.Length; i++)
                    {
                        int2 neighbourOffset = neighbourOffsetArray[i];
                        int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                        if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                        {
                            continue;
                        }

                        int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                        PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                        if (!neighbourNode.isWalkable)
                        {
                            continue;
                        }

                        int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                        int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                        if (tentativeGCost < neighbourNode.gCost)
                        {
                            neighbourNode.cameFromNodeIndex = currentNodeIndex;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.CalculateFCost();
                            pathNodeArray[neighbourNodeIndex] = neighbourNode;

                            if (!openList.Contains(neighbourNode.index))
                            {
                                openList.Add(neighbourNode.index);
                            }
                        }
                    }
                }

                PathNode endNode = pathNodeArray[endNodeIndex];
                if (endNode.cameFromNodeIndex == -1)
                {
                    Debug.Log("can't find a path");
                }
                else
                {
                    NativeList<int2> path = CalculatePath(pathNodeArray, endNode);
                    //foreach (var item in path)
                    //{
                    //    Debug.Log(item);
                    //}
                    path.Dispose();
                }

                pathNodeArray.Dispose();
                openList.Dispose();
                closeList.Dispose();
                neighbourOffsetArray.Dispose();
            }

            private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
            {
                if (endNode.cameFromNodeIndex == -1)
                {
                    return new NativeList<int2>(Allocator.Temp);
                }
                else
                {
                    NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                    path.Add(new int2(endNode.x, endNode.y));

                    PathNode currentNode = endNode;
                    while (currentNode.cameFromNodeIndex != -1)
                    {
                        PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                        path.Add(new int2(cameFromNode.x, cameFromNode.y));
                        currentNode = cameFromNode;
                    }

                    return path;
                }
            }

            private int CalculateIndex(int x, int y, int gridWidth)
            {
                return x + y * gridWidth;
            }

            private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
            {
                return gridPosition.x >= 0 && gridPosition.x < gridSize.x
                    && gridPosition.y >= 0 && gridPosition.y < gridSize.y;
            }

            private int CalculateDistanceCost(int2 a, int2 b)
            {
                int xDistance = math.abs(a.x - b.x);
                int yDistance = math.abs(a.y - b.y);
                int remaining = math.abs(xDistance - yDistance);
                return 14 * math.min(xDistance, yDistance) + 10 * remaining;
            }

            private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
            {
                PathNode lowestCostPathNode = pathNodeArray[openList[0]];
                for (int i = 1; i < openList.Length; i++)
                {
                    PathNode testPathNode = pathNodeArray[openList[i]];
                    if (testPathNode.fCost < lowestCostPathNode.fCost)
                    {
                        lowestCostPathNode = testPathNode;
                    }
                }

                return lowestCostPathNode.index;
            }
        }
    }
}
