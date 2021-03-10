using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class EnemyManager : SubStageManager
    {
        private float spawnInterval = 3f;
        private float createTimer = 0f;
        private List<Vector3>[] paths;
        private List<Enemy> enemys = new List<Enemy>();

        public EnemyManager(StageManager stageManager) : base(stageManager)
        {
            TypeEventSystem.Register<OnChangePaths>(OnChangePaths);
        }

        public override void OnUpdate()
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

        private void CreateEnemy()
        {
            EnemyData enemyData = ConfigManager.Instance.EnemyConfig.RandomData(); // 随机获取敌人数据
            GameObject enemyObj = ObjectPool.Instance.Spawn(enemyData.name); // 生成对象
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            int random = Random.Range(0, paths.Length); // 随机设置路径
            enemy.SetData(enemyData).SetPath(paths[random]);
            enemys.Add(enemy);
        }

        private void UpdateEnemys()
        {
            if (enemys.Count == 0) return;

            for (int i = 0; i < enemys.Count; i++)
            {
                if (!enemys[i].OnUpdate())
                {
                    ObjectPool.Instance.Unspawn(enemys[i].Name, enemys[i].gameObject);
                    enemys.QuickRemove(i--);
                }
            }
        }

        private void OnChangePaths(OnChangePaths context)
        {
            paths = context.paths;
        }
    }
}
