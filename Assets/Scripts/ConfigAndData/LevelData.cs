using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public enum EnemyType
    {
        Light_1,
        Light_2,
        Light_3,
        Mideum_1,
        Mideum_2,
        Mideum_3,
        Heavy_1,
        Heavy_2,
        Heavy_3,
    }


    [Serializable]
    public struct EnemySequence
    {
        public EnemyType enemyType;
        public int count;
    }


    [Serializable]
    public struct WaveData
    {
        public EnemySequence[] sequences;
    }


    [CreateAssetMenu(menuName = "TowerDefense/StageData")]
    public class StageData : ScriptableObject
    {
        public int index;
        public int playerHp;
        public int coins;
        public float waveInterval;

        [SerializeField] private WaveData[] waveDatas = default;
    }


    [Serializable]
    public class LevelData
    {
        // 第几关
        public int index;

        // 初始生命以及金币
        public int playerHp;
        public int coins;

        // 敌人数据
        public float waveInterval; // 每波的间隔
        public Dictionary<int, int>[] waveData; // 每波敌人的配置

        /// <summary>
        /// 生成默认关卡数据
        /// </summary>
        /// <returns>默认关卡数据</returns>
        public static LevelData CreateDefaultData()
        {
            LevelData data = new LevelData();

            data.playerHp = 10;
            data.coins = 200;

            data.waveInterval = 10f;
            data.waveData = new Dictionary<int, int>[5];

            data.waveData[0] = new Dictionary<int, int> { { 6, 2 } };
            data.waveData[1] = new Dictionary<int, int> { { 0, 4 }, { 1, 4 }, { 3, 5 }, { 6, 2 }, };
            data.waveData[2] = new Dictionary<int, int> { { 0, 4 }, { 1, 4 }, { 3, 2 }, { 4, 2 }, { 6, 1 }, { 7, 1 } };
            data.waveData[3] = new Dictionary<int, int> { { 1, 4 }, { 2, 4 }, { 4, 2 }, { 5, 2 }, { 7, 1 }, { 8, 1 } };
            data.waveData[4] = new Dictionary<int, int> { { 2, 8 }, { 1, 4 }, { 5, 4 }, { 8, 2 } };

            return data;
        }
    }
}
