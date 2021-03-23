using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    public class Pool
    {
        public string tag; // 池子的标签
        public GameObject prefab; // 物体的预设
        public readonly int capacity; // 最大容量
        public HashSet<PoolObject> activeList = new HashSet<PoolObject>(); // 保存所有激活状态的对象（不可用）
        public HashSet<PoolObject> inactiveList = new HashSet<PoolObject>(); // 保存所有非激活状态的对象（可用）

        public Pool(string tag, GameObject prefab, int capacity)
        {
            this.tag = tag;
            this.prefab = prefab;
            this.capacity = capacity;
        }

        public T Spawn<T>() where T : PoolObject
        {
            return Spawn<T>(Vector3.zero, Quaternion.identity, Vector3.one, null);
        }

        public T Spawn<T>(Vector3 pos) where T : PoolObject
        {
            return Spawn<T>(pos, Quaternion.identity, Vector3.one, null);
        }

        public T Spawn<T>(Vector3 pos, Quaternion rot) where T : PoolObject
        {
            return Spawn<T>(pos, rot, Vector3.one, null);
        }

        public T Spawn<T>(Vector3 pos, Quaternion rot, Vector3 scale) where T : PoolObject
        {
            return Spawn<T>(pos, rot, scale, null);
        }

        public T Spawn<T>(Vector3 pos, Quaternion rot, Vector3 scale, Transform parent) where T : PoolObject
        {
            PoolObject obj = null;

            if (inactiveList.Count == 0 && activeList.Count < capacity)
            {
                obj = Object.Instantiate(prefab, pos, rot, parent).GetComponent<T>();
                obj.OnInstantiate(tag, true);
            }
            else if (inactiveList.Count > 0)
            {
                obj = inactiveList.First(); // 从可用列表中取出第一个
                inactiveList.Remove(obj); // 从可用列表中移除

                obj.gameObject.SetActive(true);
                Transform transform = obj.transform;
                transform.SetParent(parent);
                transform.localPosition = pos;
                transform.localRotation = rot;
                transform.localScale = scale;
            }

            if (obj != null)
            {
                obj.OnSpawn();
                activeList.Add(obj);
            }

            return obj as T;
        }

        public void Unspawn(PoolObject obj)
        {
            if (inactiveList.Contains(obj)) return;

            if (activeList.Contains(obj))
            {
                obj.OnUnspawn();
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(false);
                activeList.Remove(obj);
                inactiveList.Add(obj);
            }
            else
            {
                Debug.LogError("要回收的物体不属于当前池子" + obj.Tag);
            }
        }

        public void UnspawnAll()
        {
            foreach (var obj in activeList)
            {
                obj.OnUnspawn();
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(false);
                inactiveList.Add(obj);
            }

            activeList.Clear();
        }

        public void Clear(PoolObject obj)
        {
            if (activeList.Contains(obj))
            {
                activeList.Remove(obj);
            }
            else if (inactiveList.Contains(obj))
            {
                inactiveList.Remove(obj);
            }
            else
            {
                Debug.LogError("要销毁的对象不属于当前池子" + tag);
            }

            obj.OnReclaim();
            Object.Destroy(obj.gameObject);
        }

        public void ClearAll()
        {
            foreach (var item in activeList)
            {
                item.OnReclaim();
                Object.Destroy(item.gameObject);
            }

            foreach (var item in inactiveList)
            {
                item.OnReclaim();
                Object.Destroy(item.gameObject);
            }

            activeList.Clear();
            inactiveList.Clear();
        }
    }
}
