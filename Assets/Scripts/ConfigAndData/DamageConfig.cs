using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    [Serializable]
    public class ArmorDamageConfig
    {
        public ArmorType armorType;
        public int damageRate;
    }


    [Serializable]
    public class AttackDamageConfig
    {
        public AttackType attackType;
        public ArmorDamageConfig[] armorDamageConfigs;
    }


    [CreateAssetMenu(menuName = "TowerDefense/DamageConfig", fileName = "DamageConfig")]
    public class DamageConfig : ScriptableObject
    {
        [SerializeField] private AttackDamageConfig[] damageConfig; // 伤害配置，直接在编辑器里配置

        private Dictionary<AttackType, Dictionary<ArmorType, float>> config; // 每种攻击类型对每种护甲造成的伤害百分比
        private Dictionary<AttackType, List<ArmorType>> restraintTable; // 每种攻击类型对每种护甲的优先级

        /// <summary>
        /// 计算实际造成的伤害
        /// </summary>
        /// <param name="damage">原始伤害值</param>
        /// <param name="attackType">攻击类型</param>
        /// <param name="armorType">护甲类型</param>
        /// <returns>实际伤害值</returns>
        public float GetDamage(float damage, AttackType attackType, ArmorType armorType)
        {
            return damage * GetDamageRate(attackType, armorType);
        }

        /// <summary>
        /// 计算攻击类型对护甲造成的伤害比率
        /// </summary>
        /// <param name="attackType">攻击类型</param>
        /// <param name="armorType">护甲类型</param>
        /// <returns>伤害比率</returns>
        public float GetDamageRate(AttackType attackType, ArmorType armorType)
        {
            if (config == null)
            {
                config = CreateDamageConfig();
            }

            return config[attackType][armorType];
        }

        /// <summary>
        /// 获取攻击类型对护甲的优先级（优先级越大，造成的伤害比率越高）
        /// </summary>
        /// <param name="attackType">攻击类型</param>
        /// <param name="armorType">护甲类型</param>
        /// <returns>优先级</returns>
        public int GetArmorPriority(AttackType attackType, ArmorType armorType)
        {
            if (restraintTable == null)
            {
                restraintTable = CreateRestraintTable();
            }

            for (int i = 0; i < restraintTable[attackType].Count; i++)
            {
                if (restraintTable[attackType][i] == armorType)
                {
                    return i;
                }
            }

            throw new ArgumentOutOfRangeException("找不到对应的克制关系" + attackType + " " + armorType);
        }

        // 获取伤害配置表
        private Dictionary<AttackType, Dictionary<ArmorType, float>> CreateDamageConfig()
        {
            Dictionary<AttackType, Dictionary<ArmorType, float>> dic = new Dictionary<AttackType, Dictionary<ArmorType, float>>();

            for (int i = 0; i < damageConfig.Length; i++)
            {
                dic.Add(damageConfig[i].attackType, new Dictionary<ArmorType, float>());

                for (int j = 0; j < damageConfig[i].armorDamageConfigs.Length; j++)
                {
                    dic[damageConfig[i].attackType].Add(damageConfig[i].armorDamageConfigs[j].armorType, damageConfig[i].armorDamageConfigs[j].damageRate / 100f);
                }
            }

            return dic;
        }

        // 获取攻击优先级配置表
        private Dictionary<AttackType, List<ArmorType>> CreateRestraintTable()
        {
            if (config == null)
            {
                config = CreateDamageConfig();
            }

            Dictionary<AttackType, List<ArmorType>> dic = new Dictionary<AttackType, List<ArmorType>>();

            foreach (var item in config)
            {
                dic.Add(item.Key, new List<ArmorType>());
                KeyValuePair<ArmorType, float>[] arr = item.Value.ToArray();
                Array.Sort(arr, (a, b) =>
                {
                    if (a.Value > b.Value) return -1;
                    if (a.Value < b.Value) return 1;
                    return 0;
                });

                for (int i = 0; i < arr.Length; i++)
                {
                    dic[item.Key].Add(arr[i].Key);
                }
            }

            return dic;
        }
    }
}
