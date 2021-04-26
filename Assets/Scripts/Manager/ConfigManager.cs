using UnityEngine;

namespace TowerDefense
{
    /*
     * 没有用到，所有数值配置放在GameManager里
     */
    public class ConfigManager : MonoSingleton<ConfigManager>
    {
        [Header("攻击类型以及护甲类型的伤害配置表")]
        [SerializeField] private DamageConfig damageConfig = default;

        [Header("敌人配置表")]
        [SerializeField] private EnemyConfig enemyConfig = default;

        [Header("关卡配置表，0为默认关卡数据")]
        [SerializeField] private StageConfig[] stageConfigs = default;

        public DamageConfig DamageConfig => damageConfig;

        public EnemyConfig EnemyConfig => enemyConfig;

        public StageConfig GetStageConfig(int stage) => stageConfigs[stage];

        public StageConfig DefaultStageConfig => stageConfigs[0];
    }
}
