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

        public bool CreateTower(Vector3 position)
        {
            if (manager.MapManager.TryPlaceTower(position, out int x, out int y, out Vector3 towerPos))
            {
                int i = Random.Range(0, 2);
                Tower tower;
                if (i == 0)
                {
                    tower = ObjectPool.Spawn<Tower>("MachineGunTower", towerPos);
                }
                else
                {
                    tower = ObjectPool.Spawn<Tower>("LaserTower", towerPos);
                }

                tower.SetCoordinate(x, y);
                towers.Add(tower);
                return true;
            }

            return false;
        }

        public void RemoveTower(Vector3 position)
        {
            if (manager.MapManager.GetGridPosition(position, out int x, out int y))
            {
                for (int i = 0; i < towers.Count; i++)
                {
                    if (towers[i].x == x && towers[i].y == y)
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
