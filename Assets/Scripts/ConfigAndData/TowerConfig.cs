using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public enum AttackType
    {
        MachineGun, // 普通机枪
        Laser, // 激光
        Cannon, // 加农炮
        Special, // 特殊攻击类型，造成特殊效果，不造成伤害
    }


    [Serializable]
    public class TowerLevelData
    {
        public int cost;
        public float attackRange;
        public float attackDuration;
        public float damage;
        public float shellBlastRadius;
        public float decelerationDuration;
        public float decelerationRate;
    }


    [Serializable]
    public class TowerData
    {
        public int id;
        public string name;
        public AttackType attackType;
        public TowerLevelData[] levelDatas;

        [NonSerialized] public int level;
        [NonSerialized] public TowerLevelData levelData;

        public void Init()
        {
            level = 1;
            levelData = levelDatas[level - 1];
        }
    }


    [CreateAssetMenu(menuName = "TowerDefense/TowerConfig", fileName = "TowerConfig")]
    public class TowerConfig : ScriptableObject
    {
        [SerializeField] private TowerData[] towerDatas;

        private Dictionary<int, TowerData> config;

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

            config[id].Init();
            return config[id];
        }

        public TowerData GetTowerData(string name)
        {
            if (config == null)
            {
                config = GetTowerConfig();
            }

            return config.Values.First(data =>
            {
                if (data.name == name)
                {
                    data.Init();
                    return true;
                }

                return false;
            });
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
