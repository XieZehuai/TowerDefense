using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    #region 泛型对象池，管理所有继承自Component的对象
    /// <summary>
    /// 泛型对象池
    /// </summary>
    public static class GenericObjectPool<T> where T : Component
    {
        private static readonly Dictionary<string, Pool<T>> pools = new Dictionary<string, Pool<T>>(); // 储存所有类型的池子

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="tag">池子的标签</param>
        /// <param name="prefab">目标物体的预设</param>
        /// <param name="capacity">最大容量，默认1000</param>
        public static void CreatePool(string tag, GameObject prefab, int capacity = 1000)
        {
            if (pools.ContainsKey(tag)) return;

            Pool<T> pool = new Pool<T>(tag, prefab, capacity);
            pools.Add(tag, pool);
        }

        /// <summary>
        /// 判断是否创建了带有目标标签的池子
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        public static bool ContainsPool(string tag)
        {
            return pools.ContainsKey(tag);
        }

        /// <summary>
        /// 获取目标池子，如果没有目标池子，就会创建目标池子
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <returns>目标池子的引用</returns>
        public static Pool<T> GetOrCreatePool(string tag)
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
        public static T Spawn(string tag)
        {
            return Spawn(tag, Vector3.zero, Quaternion.identity, Vector3.one, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public static T Spawn(string tag, Vector3 pos)
        {
            return Spawn(tag, pos, Quaternion.identity, Vector3.one, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置以及旋转
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <param name="rot">对象的旋转</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public static T Spawn(string tag, Vector3 pos, Quaternion rot)
        {
            return Spawn(tag, pos, rot, Vector3.one, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置、旋转以及缩放
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <param name="rot">对象的旋转</param>
        /// <param name="scale">对象的父物体</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public static T Spawn(string tag, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            return Spawn(tag, pos, rot, scale, null);
        }

        /// <summary>
        /// 根据标签获取对应对象，并设置对象的位置、旋转、缩放和父物体
        /// </summary>
        public static T Spawn(string tag, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent)
        {
            if (!ContainsPool(tag))
            {
                GameObject prefab = ResourceManager.Load<GameObject>(tag + "Prefab");
                CreatePool(tag, prefab);
            }

            return pools[tag].Spawn(pos, rot, scale, parent);
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="tag">对象所在池子的标签</param>
        /// <param name="obj">要回收的对象</param>
        public static void Unspawn(string tag, T obj)
        {
            if (!ContainsPool(tag))
            {
                Debug.LogError("没有目标池子" + tag);
                return;
            }

            pools[tag].Unspawn(obj);
        }

        /// <summary>
        /// 回收目标池子的所有对象
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        public static void UnspawnAll(string tag)
        {
            if (!ContainsPool(tag))
            {
                Debug.LogError("没有目标池子");
                return;
            }

            pools[tag].UnspawnAll();
        }

        /// <summary>
        /// 清空目标池子
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        public static void ClearPool(string tag)
        {
            if (!ContainsPool(tag)) return;

            pools[tag].Clear();
        }

        /// <summary>
        /// 清空所有池子
        /// </summary>
        public static void ClearAllPool()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
        }
    }
    #endregion


    #region 泛型池
    /// <summary>
    /// 泛型池，用于保存及管理某一类型的对象
    /// </summary>
    /// <typeparam name="T">存储对象的数据类型，必须继承自Component</typeparam>
    public class Pool<T> where T : Component
    {
        public string tag; // 池子的标签
        public GameObject prefab; // 物体的预设
        public readonly int capacity; // 最大容量
        public HashSet<T> activeList = new HashSet<T>(); // 保存所有激活状态的对象（不可用）
        public HashSet<T> inactiveList = new HashSet<T>(); // 保存所有非激活状态的对象（可用）

        public Pool(string tag, GameObject prefab, int capacity)
        {
            this.tag = tag;
            this.prefab = prefab;
            this.capacity = capacity;
        }

        public T Spawn()
        {
            return Spawn(Vector3.zero, Quaternion.identity, Vector3.one, null);
        }

        public T Spawn(Vector3 pos)
        {
            return Spawn(pos, Quaternion.identity, Vector3.one, null);
        }

        public T Spawn(Vector3 pos, Quaternion rot)
        {
            return Spawn(pos, rot, Vector3.one, null);
        }

        public T Spawn(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            return Spawn(pos, rot, scale, null);
        }

        public T Spawn(Vector3 pos, Quaternion rot, Vector3 scale, Transform parent)
        {
            T obj = null;

            if (inactiveList.Count == 0 && activeList.Count < capacity)
            {
                obj = Object.Instantiate(prefab, pos, rot, parent).GetComponent<T>();
            }
            else if (inactiveList.Count > 0)
            {
                obj = inactiveList.First(); // 从可用列表中取出第一个
                obj.gameObject.SetActive(true);
                obj.transform.SetParent(parent);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = rot;
                obj.transform.localScale = scale;
                inactiveList.Remove(obj);
            }

            if (obj != null) activeList.Add(obj);

            return obj;
        }

        public void Unspawn(T obj)
        {
            if (inactiveList.Contains(obj)) return;

            if (activeList.Contains(obj))
            {
                obj.gameObject.SetActive(false);
                activeList.Remove(obj);
                inactiveList.Add(obj);
            }
            else
            {
                Debug.LogError("要回收的物体不属于当前池子" + obj.name);
            }
        }

        public void UnspawnAll()
        {
            foreach (var obj in activeList)
            {
                obj.gameObject.SetActive(false);
                activeList.Remove(obj);
                inactiveList.Add(obj);
            }
        }

        public void Clear()
        {
            foreach (var item in activeList)
            {
                Object.Destroy(item.gameObject);
            }

            foreach (var item in inactiveList)
            {
                Object.Destroy(item.gameObject);
            }

            activeList.Clear();
            inactiveList.Clear();
        }
    }
    #endregion
}
