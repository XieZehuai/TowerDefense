using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class TowerManager : SubStageManager
    {
        private readonly List<Tower> towers = new List<Tower>();

        public TowerManager(StageManager stageManager) : base(stageManager)
        {
        }

        public override void OnUpdate()
        {
            UpdateTower();
        }

        public bool CreateTower(Vector3 position, int towerId)
        {
            if (manager.MapManager.TryPlaceTower(position, out int x, out int y, out Vector3 towerPos))
            {
                Tower tower = CreateTower(towerId, towerPos);
                tower.SetCoordinate(x, y);
                towers.Add(tower);
                return true;
            }

            return false;
        }

        private Tower CreateTower(int towerId, Vector3 towerPos)
        {
            switch (towerId)
            {
                case 0: return ObjectPool.Spawn<MachineGunTower>("MachineGunTower", towerPos);
                case 1: return ObjectPool.Spawn<LaserTower>("LaserTower", towerPos);
                case 2: return ObjectPool.Spawn<CannonTower>("CannonTower", towerPos).SetWarEntityManager(manager.WarEntityManager);
                case 3: return ObjectPool.Spawn<DecelerationTower>("DecelerationTower", towerPos);

                default: Debug.LogError("没有ID为" + towerId + "的炮塔"); break;
            }

            return null;
        }

        public void RemoveTower(Vector3 position)
        {
            if (manager.MapManager.GetGridPosition(position, out int x, out int y))
            {
                for (int i = 0; i < towers.Count; i++)
                {
                    if (towers[i].X == x && towers[i].Y == y)
                    {
                        manager.MapManager.RemoveTower(x, y);
                        ObjectPool.Unspawn(towers[i]);
                        towers.QuickRemove(i);
                        break;
                    }
                }
            }
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

        private void UpdateTower()
        {
            foreach (var tower in towers)
            {
                tower.OnUpdate();
            }
        }
    }
}
