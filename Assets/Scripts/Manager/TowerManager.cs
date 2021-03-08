using System;
using UnityEngine;

namespace TowerDefense
{
    public class TowerManager : MonoSingleton<TowerManager>
    {
        public bool CreateTower(Vector3 position)
        {
            if (MapManager.Instance.CanPlaceTower(position, out int x, out int y))
            {
                Vector3 pos = MapManager.Instance.GetCenterPosition(x, y);
                ObjectPool.Instance.Spawn("TowerLaser", pos);
                return true;
            }

            return false;
        }

        public void RemoveTower(Vector3 position)
        {

        }
    }
}
