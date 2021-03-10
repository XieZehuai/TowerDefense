using System;
using UnityEngine;

namespace TowerDefense
{
    public class Config : MonoSingleton<Config>
    {
        [SerializeField] private DamageConfig damageConfig = default;

        public float GetDamage(float damage, AttackType attackType, ArmorType armorType)
        {
            return damageConfig.GetDamage(damage, attackType, armorType);
        }
    }
}
