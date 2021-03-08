using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class TowerManager : MonoSingleton<TowerManager>
    {
        private List<Tower> towers = new List<Tower>();

        public void OnUpdate()
        {
            UpdateTower();
        }

        public bool CreateTower(Vector3 position)
        {
            if (MapManager.Instance.CanPlaceTower(position, out int x, out int y))
            {
                if (MapManager.Instance.PlaceTower(x, y))
                {
                    Vector3 pos = MapManager.Instance.GetCenterPosition(x, y);
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
            if (MapManager.Instance.GetGridPosition(position, out int x, out int y))
            {
                MapManager.Instance.RemoveTower(x, y);
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
