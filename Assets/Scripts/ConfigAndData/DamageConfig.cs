using System;
using System.Collections.Generic;
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
        [SerializeField] private AttackDamageConfig[] damageConfig;

        private Dictionary<AttackType, Dictionary<ArmorType, float>> config;

        public float GetDamage(float damage, AttackType attackType, ArmorType armorType)
        {
            return damage * GetDamageRate(attackType, armorType);
        }

        public float GetDamageRate(AttackType attackType, ArmorType armorType)
        {
            if (config == null)
            {
                config = GetDamageConfig();
            }

            return config[attackType][armorType];
        }

        private Dictionary<AttackType, Dictionary<ArmorType, float>> GetDamageConfig()
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
    }
}
