using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 管理关卡场景内所有炮塔相关的操作
    /// </summary>
    public class TowerManager : SubStageManager
    {
        private readonly List<Tower> towers = new List<Tower>();

        public TowerManager(StageManager stageManager) : base(stageManager)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            UpdateTower(deltaTime);
        }

        /// <summary>
        /// 摆放炮塔
        /// </summary>
        /// <param name="position">摆放位置</param>
        /// <param name="towerId">炮塔ID</param>
        /// <returns>摆放成功或失败</returns>
        public bool PlaceTower(Vector3 position, int towerId)
        {
            TowerData data = GameManager.Instance.TowerConfig.GetTowerData(towerId);
            data.Init();

            if (manager.Coins >= data.LevelData.cost)
            {
                if (manager.MapManager.TryPlaceTower(position, out int x, out int y, out Vector3 towerPos))
                {
                    Tower tower = CreateTower(data.name, towerPos);
                    tower.Data = data;
                    tower.SetCoordinate(x, y);
                    towers.Add(tower);
                    manager.Coins -= data.LevelData.cost;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 加载炮塔
        /// </summary>
        /// <param name="towerName">炮塔的名称</param>
        /// <param name="towerPos">炮塔要实例化的位置</param>
        /// <returns>加载的炮塔</returns>
        private Tower CreateTower(string towerName, Vector3 towerPos)
        {
            Tower tower = ObjectPool.Spawn<Tower>(towerName + "Prefab", towerPos);
            tower.Manager = manager;

            return tower;
        }

        /// <summary>
        /// 移除炮塔
        /// </summary>
        /// <param name="tower">要移除的炮塔</param>
        public void RemoveTower(Tower tower)
        {
            manager.MapManager.RemoveTower(tower.X, tower.Y);
            ObjectPool.Unspawn(tower);
            towers.Remove(tower);
        }

        public void UpgradeTower(Tower tower)
        {
            int cost = tower.Data.GetNextLevelCost();

            if (cost != -1 && manager.Coins >= cost)
            {
                manager.Coins -= cost;
                tower.LevelUp();
            }
        }

        public void SellTower(Tower tower)
        {
            manager.Coins += tower.GetSellPrice();
            RemoveTower(tower);
        }

        public void Replay()
        {
            if (towers.Count == 0) return;

            manager.MapManager.RemoveAllTower();

            for (int i = 0; i < towers.Count; i++)
            {
                ObjectPool.Unspawn(towers[i]);
            }
            towers.Clear();
        }

        private void UpdateTower(float deltaTime)
        {
            foreach (var tower in towers)
            {
                tower.OnUpdate(deltaTime);
            }
        }
    }
}