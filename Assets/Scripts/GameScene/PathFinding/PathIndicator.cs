using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 路径指示器，显示所有从出生点到目标点的移动路径
    /// </summary>
    public class PathIndicator : SubStageManager
    {
        private List<Vector3>[] paths;
        private readonly List<PoolObject> arrows = new List<PoolObject>();
        private readonly Vector3 height = new Vector3(0f, 0.01f, 0f);
        private bool showPath = true;

        public PathIndicator(StageManager stageManager) : base(stageManager)
        {
        }

        /// <summary>
        /// 设置路径数据
        /// </summary>
        /// <param name="paths"></param>
        public void SetPath(List<Vector2Int>[] paths)
        {
            this.paths = new List<Vector3>[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                this.paths[i] = new List<Vector3>();

                for (int j = 0; j < paths[i].Count; j++)
                {
                    this.paths[i].Add(manager.MapManager.GetCenterPosition(paths[i][j]));
                }
            }

            if (showPath)
            {
                HidePath();
                ShowPath();
            }
        }

        /// <summary>
        /// 显示或隐藏路径
        /// </summary>
        public void TogglePathIndicator()
        {
            showPath = !showPath;
            HidePath();

            if (showPath)
            {
                ShowPath();
            }
        }

        /// <summary>
        /// 显示路径
        /// </summary>
        private void ShowPath()
        {
            for (int i = 0; i < paths.Length; i++)
            {
                // 在路径点上加载箭头，跳过出生点和目标点
                for (int j = 1; j < paths[i].Count - 1; j++)
                {
                    Vector3 pos = paths[i][j] + height;
                    float dir = GetDirection(paths[i][j], paths[i][j + 1]);
                    Quaternion rot = Quaternion.Euler(new Vector3(90f, dir, 0f));
                    //PoolObject arrow = ObjectPool.Spawn<PoolObject>("Arrow", pos, rot);
                    PoolObject arrow = ObjectPool.Spawn<PoolObject>(Res.ArrowPrefab, pos, rot);
                    arrows.Add(arrow);
                }
            }
        }

        /// <summary>
        /// 隐藏路径
        /// </summary>
        private void HidePath()
        {
            ObjectPool.UnspawnAll(Res.ArrowPrefab);
            arrows.Clear();
        }

        /// <summary>
        /// 获取箭头的方向
        /// </summary>
        /// <param name="from">当前路径点</param>
        /// <param name="to">下一个路径点</param>
        /// <returns>旋转角度</returns>
        private static float GetDirection(Vector3 from, Vector3 to)
        {
            float x = to.x - from.x;
            float z = to.z - from.z;

            if (x == 0f && z > 0f) return 0f;
            if (x > 0f && z > 0f) return 45f;
            if (x > 0f && z == 0f) return 90f;
            if (x > 0f && z < 0f) return 135f;
            if (x == 0f && z < 0f) return 180f;
            if (x < 0f && z < 0f) return 225f;
            if (x < 0f && z == 0f) return 270f;
            return 315f;
        }
    }
}
