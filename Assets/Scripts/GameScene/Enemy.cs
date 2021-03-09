using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
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
        public float hp;
        public float speed;
        public ArmorType armorType;
    }
    
    
    public class Enemy : MonoBehaviour
    {
        public new string name;
        public float hp;
        public float speed;
        public ArmorType armorType;

        private List<Vector3> path;
        private int curr;
        private float distance;
        private float progress;
        private Vector3 height = new Vector3(0f, 0.5f, 0f);

        public Vector3 Position => transform.localPosition;

        public void SetPath(List<Vector3> path)
        {
            if (path.Count <= 1) Debug.LogError("路径长度太短");

            this.path = path;
            transform.localPosition = path[0] + height;
            transform.localRotation = Quaternion.LookRotation(path[1] - path[0]);
            distance = Vector3.Distance(path[1], path[0]);
            curr = 1;
        }

        public bool OnUpdate()
        {
            if (hp <= 0)
            {
                return false;
            }

            return Move();
        }

        public void GetDamage(float damage, AttackType attackType)
        {
            float actualDamage = GameManager.Instance.GetDamage(damage, attackType, armorType);
            hp -= actualDamage;
        }

        private bool Move()
        {
            if (curr >= path.Count) return false;

            progress += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(path[curr - 1], path[curr], progress / distance) + height;

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