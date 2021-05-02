using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 敌人的护甲类型
    /// </summary>
    public enum ArmorType
    {
        Light, // 轻甲
        Medium, // 中甲
        Heavy, // 重甲
    }


    /// <summary>
    /// 敌人的数据
    /// </summary>
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
    }


    [CreateAssetMenu(menuName = "TowerDefense/EnemyConfig", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField] private EnemyData[] enemyDatas;

        private Dictionary<int, EnemyData> config;

        /// <summary>
        /// 根据ID获取敌人的数据
        /// </summary>
        /// <param name="id">敌人的ID</param>
        /// <returns>获取到的敌人数据，获取失败返回null</returns>
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

        /// <summary>
        /// 根据敌人名称获取敌人数据
        /// </summary>
        /// <param name="name">敌人的名称</param>
        /// <returns>获取到的敌人数据，获取失败返回null</returns>
        public EnemyData GetEnemyData(string name)
        {
            if (config == null)
            {
                config = GetEnemyConfig();
            }

            return config.Values.First(data => data.name == name);
        }

        /// <summary>
        /// 随机获取敌人数据
        /// </summary>
        public EnemyData RandomData()
        {
            if (config == null)
            {
                config = GetEnemyConfig();
            }

            int id = UnityEngine.Random.Range(0, config.Count);
            return config[id];
        }

        // 获取敌人数据配置表
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
