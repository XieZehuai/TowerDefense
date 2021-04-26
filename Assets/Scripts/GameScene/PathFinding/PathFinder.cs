using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    #region 寻路算法
    /// <summary>
    /// 寻路算法
    /// </summary>
    public enum PathFindingStrategy
    {
        /// <summary>
        /// Dijkstra算法
        /// </summary>
        Dijkstra,
        /// <summary>
        /// 流场
        /// </summary>
        FlowField,
        /// <summary>
        /// 用DOTS实现的A星
        /// </summary>
        DOTS,
        /// <summary>
        /// A星
        /// </summary>
        AStar,
    }
    #endregion


    public class PathFinder
    {
        private IPathFindingStrategy pathFindingStrategy;

        public PathFinder()
        {
        }

        public PathFinder(PathFindingStrategy strategy)
        {
            SetPathFindingStrategy(strategy);
        }

        /// <summary>
        /// 设置寻路算法
        /// </summary>
        /// <param name="strategy">要使用的寻路算法</param>
        public void SetPathFindingStrategy(PathFindingStrategy strategy)
        {
            pathFindingStrategy = GetPathFindingStrategy(strategy);
        }

        /// <summary>
        /// 设置将应用寻路算法的地图数据
        /// </summary>
        /// <param name="datas">由所有地图元素组成的二维数组</param>
        public void SetMapData(MapObject[,] datas)
        {
            if (pathFindingStrategy == null)
            {
                Debug.LogError("未选择寻路算法");
            }

            pathFindingStrategy.SetMapData(datas);
        }

        /// <summary>
        /// 寻路
        /// </summary>
        /// <param name="startPositions">由所有起点组成的数组</param>
        /// <param name="endPosition">目标点</param>
        /// <param name="paths">用于保存寻找到的路径</param>
        /// <param name="showResult">是否打印结果</param>
        /// <returns>寻路成功返回true，失败返回false</returns>
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
                Debug.Log(pathFindingStrategy.Name + " 寻路" + (success ? "成功" : "失败") +
                    "寻路时间：" + (Time.realtimeSinceStartup - startTime) * 1000f + "毫秒");
            }

            return success;
        }

        private IPathFindingStrategy GetPathFindingStrategy(PathFindingStrategy strategy)
        {
            switch (strategy)
            {
                case PathFindingStrategy.Dijkstra: return new DijkstraPathFinding();
                case PathFindingStrategy.FlowField: return new FlowFieldPathFinding();
                case PathFindingStrategy.DOTS: return new DOTSPathFinding();
                case PathFindingStrategy.AStar: return new AStarPathFinding();

                default:
                    Debug.LogError("不支持的寻路策略" + strategy);
                    break;
            }

            return null;
        }
    }
}
