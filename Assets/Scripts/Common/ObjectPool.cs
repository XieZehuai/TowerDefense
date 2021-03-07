using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public readonly int capacity;
        public HashSet<GameObject> activeList = new HashSet<GameObject>();
        public HashSet<GameObject> inactiveList = new HashSet<GameObject>();

        public Pool(string tag, GameObject prefab, int capacity)
        {
            this.tag = tag;
            this.prefab = prefab;
            this.capacity = capacity;
        }

        public GameObject Spawn(Vector3 pos, Quaternion rot, Transform parent)
        {
            GameObject obj = null;

            if (inactiveList.Count == 0 && activeList.Count < capacity)
            {
                obj = GameObject.Instantiate(prefab, pos, rot, parent);
            }
            else if (inactiveList.Count > 0)
            {
                obj = inactiveList.First();
                obj.SetActive(true);
                obj.transform.SetParent(parent);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = rot;
                inactiveList.Remove(obj);
            }

            if (obj != null) activeList.Add(obj);

            return obj;
        }

        public void Unspawn(GameObject obj)
        {
            if (inactiveList.Contains(obj)) return;

            if (activeList.Contains(obj))
            {
                obj.SetActive(false);
                activeList.Remove(obj);
                inactiveList.Add(obj);
            }
            else
            {
                Debug.LogError("要回收的物体不属于当前池子" + obj.name);
            }
        }

        public void Clear()
        {
            foreach (var item in activeList)
            {
                GameObject.Destroy(item);
            }
            foreach (var item in inactiveList)
            {
                GameObject.Destroy(item);
            }

            activeList.Clear();
            inactiveList.Clear();
        }
    }


    public class ObjectPool : MonoSingleton<ObjectPool>
    {
        private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

        public void CreatePool(string tag, GameObject prefab, int capacity = 1000)
        {
            if (pools.ContainsKey(tag)) return;

            Pool pool = new Pool(tag, prefab, capacity);
            pools.Add(tag, pool);
        }

        public bool ContainsPool(string tag)
        {
            return pools.ContainsKey(tag);
        }

        public Pool GetPool(string tag)
        {
            if (ContainsPool(tag)) return pools[tag];

            return null;
        }

        public GameObject Spawn(string tag)
        {
            return Spawn(tag, Vector3.zero, Quaternion.identity, null);
        }

        public GameObject Spawn(string tag, Vector3 pos)
        {
            return Spawn(tag, pos, Quaternion.identity, null);
        }

        public GameObject Spawn(string tag, Vector3 pos, Quaternion rot)
        {
            return Spawn(tag, pos, rot, null);
        }

        public GameObject Spawn(string tag, Vector3 pos, Quaternion rot, Transform parent)
        {
            if (!ContainsPool(tag))
            {
                GameObject prefab = ResourceManager.Load<GameObject>(tag + "Prefab");
                CreatePool(tag, prefab);
            }

            return pools[tag].Spawn(pos, rot, parent);
        }

        public void Unspawn(string tag, GameObject obj)
        {
            if (!ContainsPool(tag))
            {
                Debug.LogError("没有目标池子");
                return;
            }

            pools[tag].Unspawn(obj);
        }

        public void DelayUnspawn(string tag, GameObject obj, float delay)
        {
            this.Invoke(() => Unspawn(tag, obj), delay);
        }

        public void DelayUnspawn(string tag, GameObject obj, YieldInstruction condition)
        {
            this.Invoke(() => Unspawn(tag, obj), condition);
        }

        public void ClearPool(string tag)
        {
            if (!ContainsPool(tag)) return;

            pools[tag].Clear();
        }

        public void ClearAllPool()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
        }
    }
}
