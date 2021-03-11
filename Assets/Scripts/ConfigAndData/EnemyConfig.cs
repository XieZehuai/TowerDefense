using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    public enum ArmorType
    {
        Light, // 轻甲
        Medium, // 中甲
        Heavy, // 重甲
    }


    [Serializable]
    public class EnemyData
    {
        public int id; // ID
        public string name; // 名字
        public int level; // 等级
        public int reward; // 击杀奖励金币数量
        public float hp; // 生命值
        public float speed; // 移动速度
        public ArmorType armorType; // 护甲类型
        public float spawnInterval; // 生成延迟
    }


    [CreateAssetMenu(menuName = "TowerDefense/EnemyConfig", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField] private EnemyData[] enemyDatas;

        private Dictionary<int, EnemyData> config;

        public EnemyData GetEnemyData(int id)
        {
            if (config == null)
            {
                config = GetEnemyConfig();
            }

            if (!config.ContainsKey(id))
            {
                Debug.LogError("找不到敌人数据" + id);
                return null;
            }

            return config[id];
        }

        public EnemyData GetEnemyData(string name)
        {
            if (config == null)
            {
                config = GetEnemyConfig();
            }

            return config.Values.First(data => data.name == name);
        }

        public EnemyData RandomData()
        {
            if (config == null)
            {
                config = GetEnemyConfig();
            }

            int id = UnityEngine.Random.Range(0, config.Count);
            return config[id];
        }

        private Dictionary<int, EnemyData> GetEnemyConfig()
        {
            Dictionary<int, EnemyData> dic = new Dictionary<int, EnemyData>();

            for (int i = 0; i < enemyDatas.Length; i++)
            {
                dic.Add(enemyDatas[i].id, enemyDatas[i]);
            }

            return dic;
        }
    }
}
