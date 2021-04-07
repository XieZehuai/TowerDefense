using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace TowerDefense
{
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
                    Tower tower = CreateTower(towerId, towerPos);
                    tower.Data = data;
                    tower.SetCoordinate(x, y);
                    towers.Add(tower);
                    manager.Coins -= data.LevelData.cost;
                    return true;
                }
            }

            return false;
        }

        private Tower CreateTower(int towerId, Vector3 towerPos)
        {
            Tower tower = null;

            switch (towerId)
            {
                case 0:
                    tower = ObjectPool.Spawn<MachineGunTower>("MachineGunTower", towerPos);
                    break;

                case 1:
                    tower = ObjectPool.Spawn<LaserTower>("LaserTower", towerPos);
                    break;

                case 2:
                    tower = ObjectPool.Spawn<CannonTower>("CannonTower", towerPos)
                        .SetWarEntityManager(manager.WarEntityManager);
                    break;

                case 3:
                    tower = ObjectPool.Spawn<DecelerationTower>("DecelerationTower", towerPos);
                    break;

                default:
                    Debug.LogError("没有ID为" + towerId + "的炮塔");
                    break;
            }

            tower.Manager = manager;

            return tower;
        }

        public void RemoveTower(Tower tower)
        {
            manager.MapManager.RemoveTower(tower.X, tower.Y);
            ObjectPool.Unspawn(tower);
            towers.Remove(tower);
        }

        public void Replay()
        {
            if (towers.Count == 0) return;

            manager.MapManager.RemoveAllTower();
            ObjectPool.UnspawnAll("MachineGunTower");
            ObjectPool.UnspawnAll("LaserTower");
            ObjectPool.UnspawnAll("CannonTower");
            ObjectPool.UnspawnAll("DecelerationTower");
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