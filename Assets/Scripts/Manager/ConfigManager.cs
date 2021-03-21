using UnityEngine;

namespace TowerDefense
{
    public class ConfigManager : MonoSingleton<ConfigManager>
    {
        [SerializeField] private DamageConfig damageConfig = default;
        [SerializeField] private EnemyConfig enemyConfig = default;

        public DamageConfig DamageConfig => damageConfig;

        public EnemyConfig EnemyConfig => enemyConfig;
    }
}
