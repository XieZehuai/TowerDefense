using UnityEngine.SceneManagement;
using UnityEngine;

namespace TowerDefense
{
    // TODO:    失败结算界面、章节选择界面、音效管理、设置界面、加速模式
    // TODO:    多个关卡、所有炮塔数据、点击炮塔类型后显示炮塔模型
    // BUG:     
    // DOING:   成功结算界面，玩家数据管理

    /// <summary>
    /// 控制游戏主流程
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        public PathFindingStrategy pathFindingStrategy;

        #region 游戏数值配置
        [Header("攻击类型以及护甲类型的伤害配置表")]
        [SerializeField] private DamageConfig damageConfig = default;

        [Header("敌人配置表")]
        [SerializeField] private EnemyConfig enemyConfig = default;

        [Header("关卡配置表，0为默认关卡数据")]
        [SerializeField] private StageConfig[] stageConfigs = default;

        [Header("炮塔配置表")]
        [SerializeField] private TowerConfig towerConfig = default;
        #endregion

        public DamageConfig DamageConfig => damageConfig;

        public EnemyConfig EnemyConfig => enemyConfig;

        public StageConfig DefaultStageConfig => stageConfigs[0];

        public TowerConfig TowerConfig => towerConfig;

        protected override void OnInit()
        {
            UIManager.Instance.Open<UIMainScene>();
        }

        /// <summary>
        /// 加载游戏场景
        /// </summary>
        public void LoadGameScene()
        {
            UIManager.Instance.CloseAll();
            SceneManager.LoadScene("GameScene");
        }

        /// <summary>
        /// 加载主场景
        /// </summary>
        public void LoadMainScene()
        {
            UIManager.Instance.CloseAll();
            SceneManager.LoadScene("MainScene");
            UIManager.Instance.Open<UIMainScene>();
        }

        /// <summary>
        /// 获取关卡数据
        /// </summary>
        /// <param name="stage">目标关卡数</param>
        /// <returns>成功返回对应的数据，失败返回默认关卡数据</returns>
        public StageConfig GetStageConfig(int stage)
        {
            if (stage > 0 && stage < stageConfigs.Length)
            {
                return stageConfigs[stage];
            }

            Debug.LogWarning("获取不到目标关卡的数据，返回默认关卡的数据，关卡" + stage);
            return DefaultStageConfig;
        }
    }
}