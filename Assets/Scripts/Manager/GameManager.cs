using UnityEngine.SceneManagement;
using UnityEngine;

namespace TowerDefense
{
    // TODO: 成功结算界面、失败结算界面、章节选择界面、音效管理、设置界面、加速模式
    // TODO: 多个关卡、炮塔数据、炮塔攻击范围显示、点击炮塔类型后显示炮塔模型
    // BUG: 敌人血量显示在UI上
    
    /// <summary>
    /// 控制游戏主流程
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        public PathFindingStrategy pathFindingStrategy;
        [SerializeField] private int stage;
        public int Stage => stage;

        #region 游戏数值配置

        [Header("攻击类型以及护甲类型的伤害配置表")] [SerializeField]
        private DamageConfig damageConfig = default;

        [Header("敌人配置表")] [SerializeField] private EnemyConfig enemyConfig = default;

        [Header("关卡配置表，0为默认关卡数据")] [SerializeField]
        private StageConfig[] stageConfigs = default;

        [Header("炮塔配置表")] [SerializeField] private TowerConfig towerConfig = default;

        public DamageConfig DamageConfig => damageConfig;

        public EnemyConfig EnemyConfig => enemyConfig;

        public StageConfig DefaultStageConfig => stageConfigs[0];

        public TowerConfig TowerConfig => towerConfig;

        #endregion

        protected override void OnInit()
        {
            UIManager.Instance.Open<UIMainScene>();
        }

        /// <summary>
        /// 加载游戏场景
        /// </summary>
        public void LoadGameScene()
        {
            SceneManager.LoadScene("GameScene");
            UIManager.Instance.Close<UIMainScene>();
        }

        /// <summary>
        /// 加载主场景
        /// </summary>
        public void LoadMainScene()
        {
            SceneManager.LoadScene("MainScene");
            UIManager.Instance.Open<UIMainScene>();
        }

        public StageConfig GetStageConfig(int stage)
        {
            if (stage > 0 && stage < stageConfigs.Length)
            {
                return stageConfigs[stage];
            }

            Debug.LogWarning("获取不到目标关卡的数据，返回默认关卡的数据 " + stage);
            return DefaultStageConfig;
        }
    }
}