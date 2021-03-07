using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    // 敌人护甲类型
    public enum ArmorType
    {
        Light, // 轻甲
        Medium, // 中甲
        Heavy, // 重甲
    }


    [Serializable]
    public class EnemyData
    {
        public int id;
        public string name;
        public int hp;
        public float speed;
        public ArmorType armorType;
        public GameObject model;
    }


    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        private List<Vector3> path;
        private int curr = 0;
        private float progress = 0f;

        public void SetPath(List<Vector3> path)
        {
            this.path = path;
        }

        public void OnUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (path == null || path.Count == 0) return;
            if (curr >= path.Count) return;

            progress += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(path[curr], path[curr + 1], progress);

            if (progress >= 1f)
            {
                progress = 0f;
                curr++;
                if (curr < path.Count)
                {
                    transform.LookAt(path[curr]);
                }
            }

        }
    }
}
