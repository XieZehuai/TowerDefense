using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 敌人序列，由多个相同敌人组成
    /// </summary>
    [Serializable]
    public class EnemySequence
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

        public EnemyType enemyType; // 敌人ID
        public int count; // 数量
        public float interval; // 敌人的生成间隔
    }


    /// <summary>
    /// 每波敌人的配置，由多个敌人序列组成
    /// </summary>
    [CreateAssetMenu(menuName = "TowerDefense/WaveData", fileName = "WaveConfig_")]
    public class WaveConfig : ScriptableObject, IEnumerable<EnemySequence>
    {
        public EnemySequence[] sequences;

        public IEnumerator<EnemySequence> GetEnumerator()
        {
            for (int i = 0; i < sequences.Length; i++)
            {
                yield return sequences[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}