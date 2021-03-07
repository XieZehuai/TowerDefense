using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class ObjectPool : MonoSingleton<ObjectPool>
    {
        private readonly Dictionary<string, Pool> pools = new Dictionary<string, Pool>(); // 储存所有类型的池子

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="tag">池子的标签</param>
        /// <param name="prefab">目标物体的预设</param>
        /// <param name="capacity">最大容量，默认1000</param>
        public void CreatePool(string tag, GameObject prefab, int capacity = 1000)
        {
            if (pools.ContainsKey(tag)) return;

            Pool pool = new Pool(tag, prefab, capacity);
            pools.Add(tag, pool);
        }

        /// <summary>
        /// 判断是否创建了带有目标标签的池子
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        public bool ContainsPool(string tag)
        {
            return pools.ContainsKey(tag);
        }

        /// <summary>
        /// 获取目标池子，如果没有目标池子，就会创建目标池子
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <returns>目标池子的引用</returns>
        public Pool GetOrCreatePool(string tag)
        {
            if (!ContainsPool(tag))
            {
                GameObject prefab = ResourceManager.Load<GameObject>(tag + "Prefab");
                CreatePool(tag, prefab);
            }

            return pools[tag];
        }

        /// <summary>
        /// 根据池子的标签获取对象
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public GameObject Spawn(string tag)
        {
            return Spawn(tag, Vector3.zero, Quaternion.identity, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public GameObject Spawn(string tag, Vector3 pos)
        {
            return Spawn(tag, pos, Quaternion.identity, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置以及旋转
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <param name="rot">对象的旋转</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public GameObject Spawn(string tag, Vector3 pos, Quaternion rot)
        {
            return Spawn(tag, pos, rot, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置以及旋转以及父物体
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <param name="rot">对象的旋转</param>
        /// <param name="parent">对象的父物体</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public GameObject Spawn(string tag, Vector3 pos, Quaternion rot, Transform parent)
        {
            if (!ContainsPool(tag))
            {
                GameObject prefab = ResourceManager.Load<GameObject>(tag + "Prefab");
                CreatePool(tag, prefab);
            }

            return pools[tag].Spawn(pos, rot, parent);
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="tag">对象所在池子的标签</param>
        /// <param name="obj">要回收的对象</param>
        public void Unspawn(string tag, GameObject obj)
        {
            if (!ContainsPool(tag))
            {
                Debug.LogError("没有目标池子");
                return;
            }

            pools[tag].Unspawn(obj);
        }

        /// <summary>
        /// 延迟一段时间回收对象
        /// </summary>
        /// <param name="tag">对象所在池子的标签</param>
        /// <param name="obj">要回收的对象</param>
        /// <param name="delay">延迟时间</param>
        public void DelayUnspawn(string tag, GameObject obj, float delay)
        {
            this.Invoke(() => Unspawn(tag, obj), delay);
        }

        /// <summary>
        /// 根据条件延迟回收对象
        /// </summary>
        /// <param name="tag">对象所在池子的标签</param>
        /// <param name="obj">要回收的对象</param>
        /// <param name="condition">延迟条件</param>
        public void DelayUnspawn(string tag, GameObject obj, YieldInstruction condition)
        {
            this.Invoke(() => Unspawn(tag, obj), condition);
        }

        /// <summary>
        /// 清空目标池子
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        public void ClearPool(string tag)
        {
            if (!ContainsPool(tag)) return;

            pools[tag].Clear();
        }

        /// <summary>
        /// 清空所有池子
        /// </summary>
        public void ClearAllPool()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
        }
    }


    /// <summary>
    /// 池子，管理某一类对象
    /// </summary>
    public class Pool
    {
        public string tag; // 池子的标签
        public GameObject prefab; // 物体的预设
        public readonly int capacity; // 最大容量
        public HashSet<GameObject> activeList = new HashSet<GameObject>(); // 保存所有激活状态的对象（不可用）
        public HashSet<GameObject> inactiveList = new HashSet<GameObject>(); // 保存所有非激活状态的对象（可用）

        public Pool(string tag, GameObject prefab, int capacity)
        {
            this.tag = tag;
            this.prefab = prefab;
            this.capacity = capacity;
        }

        public GameObject Spawn()
        {
            return Spawn(Vector3.zero, Quaternion.identity, null);
        }

        public GameObject Spawn(Vector3 pos)
        {
            return Spawn(pos, Quaternion.identity, null);
        }

        public GameObject Spawn(Vector3 pos, Quaternion rot)
        {
            return Spawn(pos, rot, null);
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
}
