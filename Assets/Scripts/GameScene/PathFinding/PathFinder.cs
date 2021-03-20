using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public enum PathFindingStrategy
    {
        Dijkstra,
        ReverseDijkstra,
        DOTS,
    }


    public class PathFinder
    {
        private IPathFindingStrategy pathFindingStrategy;

        public PathFinder(IPathFindingStrategy pathFindingStrategy)
        {
            SetPathFindingStrategy(pathFindingStrategy);
        }

        public void SetPathFindingStrategy(IPathFindingStrategy pathFindingStrategy)
        {
            this.pathFindingStrategy = pathFindingStrategy;
        }

        public void SetMapData(MapObject[,] datas)
        {
            if (pathFindingStrategy == null)
            {
                Debug.LogError("未选择寻路算法");
            }

            pathFindingStrategy.SetMapData(datas);
        }

        public bool FindPaths(Vector2Int[] startPositions, Vector2Int endPosition, ref List<Vector2Int>[] paths, bool showResult = false)
        {
            if (pathFindingStrategy == null)
            {
                Debug.LogError("未选择寻路算法");
                return false;
            }

            float startTime = Time.realtimeSinceStartup;
            bool success = pathFindingStrategy.FindPaths(startPositions, endPosition, ref paths);

            if (showResult)
            {
                Debug.Log("寻路" + (success ? "成功" : "失败"));
                Debug.Log("寻路时间：" + (Time.realtimeSinceStartup - startTime) * 1000f + "毫秒");
            }

            return success;
        }
    }
}
