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
        private Vector3 direction = Vector3.zero;

        public void SetPath(List<Vector3> path)
        {
            if (path.Count <= 1) Debug.LogError("路径长度太短");

            this.path = path;
            transform.localPosition = path[0];
            direction = path[1] - path[0];
            direction = direction.normalized;
            transform.LookAt(transform.localPosition + direction);
            curr = 1;
        }

        public void OnUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (curr >= path.Count) return;

            transform.localPosition += direction * Time.deltaTime * data.speed;

            if (Vector3.Distance(transform.localPosition, path[curr]) < 0.1f)
            {
                curr++;
                if (curr < path.Count)
                {
                    direction = path[curr] - path[curr - 1];
                    direction = direction.normalized;
                    transform.LookAt(transform.localPosition + direction);
                }
            }

        }
    }
}
