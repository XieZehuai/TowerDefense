using UnityEngine.SceneManagement;
using UnityEngine;

namespace TowerDefense
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public bool load;
        public string fileName = "MapData";
        public PathFindingStrategy pathFindingStrategy;
        [SerializeField] private int stage;
        public int Stage => stage;

        #region 游戏数值配置
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
        #endregion

        protected override void OnInit()
        {
            UIManager.Instance.Open<UIMainScene>();
        }

        private void Start()
        {
            // Debug.Log(ConfigManager.Instance.DamageConfig.GetArmorPriority(AttackType.Laser, ArmorType.Heavy));
            // Debug.Log(ConfigManager.Instance.DamageConfig.GetArmorPriority(AttackType.Laser, ArmorType.Medium));
            // Debug.Log(ConfigManager.Instance.DamageConfig.GetArmorPriority(AttackType.Laser, ArmorType.Light));
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene("GameScene");
            UIManager.Instance.Close<UIMainScene>();
        }

        public void LoadMainScene()
        {
            SceneManager.LoadScene("MainScene");
            UIManager.Instance.Open<UIMainScene>();
        }
    }
}