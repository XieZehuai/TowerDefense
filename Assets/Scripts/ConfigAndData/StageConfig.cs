using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 关卡数据，包括初始血量、金币以及敌人配置，敌人配置由多波敌人组成
    /// </summary>
    [CreateAssetMenu(menuName = "TowerDefense/StageData", fileName = "StageConfig_")]
    public class StageConfig : ScriptableObject
    {
        public int stage;
        public int playerHp;
        public int coins;
        public float waveInterval;
        public WaveConfig[] waveDatas = default;
    }
}
