using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 关卡数值配置
    /// </summary>
    [CreateAssetMenu(menuName = "TowerDefense/StageData", fileName = "StageConfig_")]
    public class StageConfig : ScriptableObject
    {
        /// <summary>
        /// 对应的关卡数
        /// </summary>
        public int stage;
        /// <summary>
        /// 初始血量
        /// </summary>
        public int hp;
        /// <summary>
        /// 初始金币
        /// </summary>
        public int coins;
        /// <summary>
        /// 一星评分需要剩余血量
        /// </summary>
        public int oneStarHp;
        /// <summary>
        /// 二星评分需要剩余血量
        /// </summary>
        public int twoStarHp;
        /// <summary>
        /// 三星评分需要剩余血量
        /// </summary>
        public int threeStarHp;
        /// <summary>
        /// 每一波的生成间隔
        /// </summary>
        public float waveInterval;
        /// <summary>
        /// 每一波敌人的配置
        /// </summary>
        public WaveConfig[] waveDatas = default;

        /// <summary>
        /// 计算关卡的评分
        /// </summary>
        /// <param name="hp">剩余血量</param>
        /// <returns>关卡评分</returns>
        public int GetScore(int hp)
        {
            if (hp >= threeStarHp) return 3;
            if (hp >= twoStarHp) return 2;
            if (hp >= oneStarHp) return 1;
            return 0;
        }
    }
}
