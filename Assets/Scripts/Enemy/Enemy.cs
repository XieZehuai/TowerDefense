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
        public EnemyData data;

        private List<Vector3> path;
        private int curr;
        private float distance = 0f;
        private float progress = 0f;

        public void SetPath(List<Vector3> path)
        {
            if (path.Count <= 1) Debug.LogError("路径长度太短");

            this.path = path;
            transform.localPosition = path[0];
            transform.localRotation = Quaternion.LookRotation(path[1] - path[0]);
            distance = Vector3.Distance(path[1], path[0]);
            curr = 1;
        }

        public bool OnUpdate()
        {
            return Move();
        }

        private bool Move()
        {
            if (curr >= path.Count) return false;

            progress += Time.deltaTime * data.speed;
            transform.localPosition = Vector3.Lerp(path[curr - 1], path[curr], progress / distance);

            if (progress >= distance)
            {
                curr++;
                progress = 0f;

                if (curr < path.Count)
                {
                    distance = Vector3.Distance(path[curr], path[curr - 1]);
                    transform.localRotation = Quaternion.LookRotation(path[curr] - path[curr - 1]);
                }
            }

            return true;
        }
    }
}
