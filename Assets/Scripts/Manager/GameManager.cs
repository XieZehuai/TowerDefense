using UnityEngine.SceneManagement;
using UnityEngine;

// TODO:    点击炮塔类型后显示炮塔模型
// BUG:    

namespace TowerDefense
{ 
    /// <summary>
    /// 游戏主管理器，管理整个游戏的配置以及流程
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
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

        public int MaxStageCount => stageConfigs.Length - 1;

        public TowerConfig TowerConfig => towerConfig;

        protected override void OnInit()
        {
            UIManager.Instance.Open<UIMainScene>();
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
        /// 加载关卡
        /// </summary>
        /// <param name="stage">关卡数</param>
        public void LoadStage(int stage)
        {
            PlayerManager.Instance.ChangeCurrentStage(stage);
            LoadGameScene();
        }

        /// <summary>
        /// 加载下一关
        /// </summary>
        public void LoadNextStage()
        {
            PlayerManager.Instance.NextStage();
            LoadGameScene();
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 加载游戏场景
        /// </summary>
        private void LoadGameScene()
        {
            UIManager.Instance.CloseAll();
            SceneManager.LoadScene("GameScene");
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
