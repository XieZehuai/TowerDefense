using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 炮塔的攻击类型
    /// </summary>
    public enum AttackType
    {
        MachineGun, // 普通机枪
        Laser, // 激光
        Cannon, // 加农炮
        Special, // 特殊攻击类型，造成特殊效果，不造成伤害
    }


    /// <summary>
    /// 炮塔的等级数据
    /// </summary>
    [Serializable]
    public struct TowerLevelData
    {
        public int cost;
        public float attackRange;
        public float attackDuration;
        public float damage;
        public float shellBlastRadius;
        public float decelerateDuration;
        public float decelerateRate;
    }


    /// <summary>
    /// 炮塔数据
    /// </summary>
    [Serializable]
    public struct TowerData
    {
        public int id; // 炮塔的ID
        public string name; // 炮塔的名称
        public AttackType attackType; // 炮塔的攻击类型
        public TowerLevelData[] levelDatas; // 炮塔每个等级的数据

        /// <summary>
        /// 炮塔当前的等级
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 炮塔当前等级的数据
        /// </summary>
        public TowerLevelData LevelData { get; private set; }

        public void Init()
        {
            Level = 1;
            LevelData = levelDatas[0];
        }

        /// <summary>
        /// 获取下一级的花费
        /// </summary>
        public int GetNextLevelCost()
        {
            return !HasNextLevel() ? -1 : levelDatas[Level].cost;
        }

        /// <summary>
        /// 是否能继续升级
        /// </summary>
        public bool HasNextLevel()
        {
            return Level < levelDatas.Length;
        }

        /// <summary>
        /// 升级
        /// </summary>
        public void LevelUp()
        {
            LevelData = levelDatas[Level++];
        }

        /// <summary>
        /// 获取炮塔升级到当前等级的总花费
        /// </summary>
        public int GetTotalCost()
        {
            int cost = 0;

            for (int i = 0; i < Level; i++)
            {
                cost += levelDatas[i].cost;
            }

            return cost;
        }
    }


    [CreateAssetMenu(menuName = "TowerDefense/TowerConfig", fileName = "TowerConfig")]
    public class TowerConfig : ScriptableObject
    {
        [SerializeField] private TowerData[] towerDatas;

        private Dictionary<int, TowerData> config;

        /// <summary>
        /// 根据ID获取炮塔数据
        /// </summary>
        /// <param name="id">炮塔的ID</param>
        /// <returns>炮塔的数据</returns>
        public TowerData GetTowerData(int id)
        {
            if (config == null)
            {
                config = GetTowerConfig();
            }

            if (!config.ContainsKey(id))
            {
                Debug.LogError("找不到炮塔数据" + id);
            }

            return config[id];
        }

        /// <summary>
        /// 根据炮塔名称获取炮塔数据
        /// </summary>
        /// <param name="name">炮塔的名称</param>
        /// <returns>炮塔的数据</returns>
        public TowerData GetTowerData(string name)
        {
            if (config == null)
            {
                config = GetTowerConfig();
            }

            return config.Values.First(data => data.name == name);
        }

        private Dictionary<int, TowerData> GetTowerConfig()
        {
            Dictionary<int, TowerData> dic = new Dictionary<int, TowerData>();

            for (int i = 0; i < towerDatas.Length; i++)
            {
                dic.Add(towerDatas[i].id, towerDatas[i]);
            }

            return dic;
        }
    }
}