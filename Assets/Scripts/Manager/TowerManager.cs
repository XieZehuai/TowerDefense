using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class TowerManager : SubStageManager
    {
        private List<Tower> towers = new List<Tower>();

        public TowerManager(StageManager stageManager) : base(stageManager)
        {
        }

        public override void OnUpdate()
        {
            UpdateTower();
        }

        public bool CreateTower(Vector3 position)
        {
            if (manager.MapManager.CanPlaceTower(position, out int x, out int y))
            {
                if (manager.MapManager.PlaceTower(x, y))
                {
                    Vector3 pos = manager.MapManager.GetCenterPosition(x, y);
                    GameObject obj = ObjectPool.Instance.Spawn("LaserTower", pos);
                    Tower tower = obj.GetComponent<Tower>();
                    tower.SetCoordinate(x, y);
                    towers.Add(tower);
                    return true;
                }
            }

            return false;
        }

        public void RemoveTower(Vector3 position)
        {
            if (manager.MapManager.GetGridPosition(position, out int x, out int y))
            {
                manager.MapManager.RemoveTower(x, y);
            }
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
