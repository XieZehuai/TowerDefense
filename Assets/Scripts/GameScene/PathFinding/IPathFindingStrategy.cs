using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 寻路算法接口
    /// </summary>
    public interface IPathFindingStrategy
    {
        /// <summary>
        /// 算法的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 设置地图数据
        /// </summary>
        /// <param name="datas">原始地图数据</param>
        void SetMapData(MapObject[,] datas);

        /// <summary>
        /// 寻路
        /// </summary>
        /// <param name="startPositions">由所有起始点组成的数组</param>
        /// <param name="endPosition">终点</param>
        /// <param name="paths">用于保存寻找到的所有路径</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool FindPaths(Vector2Int[] startPositions, Vector2Int endPosition, ref List<Vector2Int>[] paths);
    }
}
