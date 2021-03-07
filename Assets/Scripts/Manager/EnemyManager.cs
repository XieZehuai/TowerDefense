using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        private float spawnInterval = 3f;

        private float createTimer = 0f;
        private List<Vector3>[] paths;
        private List<Enemy> enemys = new List<Enemy>();

        public void OnUpdate()
        {
            createTimer += Time.deltaTime;
            if (createTimer >= spawnInterval)
            {
                createTimer = 0f;
                CreateEnemy();
            }

            UpdateEnemys();
        }

        public void SetLevelData(float spawnInterval)
        {
            this.spawnInterval = spawnInterval;
        }

        public void SetPaths(List<Vector3>[] paths)
        {
            this.paths = paths;
            foreach (var item in paths)
            {
                Debug.Log("起点" + item[0]);
            }
        }

        private void CreateEnemy()
        {
            int random = UnityEngine.Random.Range(0, paths.Length);
            GameObject gauss = ObjectPool.Instance.Spawn("EnemyGauss");
            Enemy enemy = gauss.GetComponent<Enemy>();
            enemy.SetPath(paths[random]);
            enemys.Add(enemy);
        }

        private void UpdateEnemys()
        {
            if (enemys.Count == 0) return;

            foreach (var enemy in enemys)
            {
                enemy.OnUpdate();
            }
        }
    }
}
