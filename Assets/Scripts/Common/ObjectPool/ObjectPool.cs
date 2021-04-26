using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public static class ObjectPool
    {
        private static readonly Dictionary<string, Pool> pools = new Dictionary<string, Pool>(); // 储存所有类型的池子

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="tag">池子的标签</param>
        /// <param name="prefab">目标物体的预设</param>
        /// <param name="capacity">最大容量，默认1000</param>
        public static void CreatePool(string tag, GameObject prefab, int capacity = 1000)
        {
            if (pools.ContainsKey(tag)) return;

            Pool pool = new Pool(tag, prefab, capacity);
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
        public static Pool GetOrCreatePool(string tag)
        {
            if (!ContainsPool(tag))
            {
                //GameObject prefab = ResourceManager.Load<GameObject>(tag + "Prefab");
                GameObject prefab = ResourceManager.Load<GameObject>(tag);
                CreatePool(tag, prefab);
            }

            return pools[tag];
        }

        public static PoolObject Spawn(string tag, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null)
        {
            return Spawn<PoolObject>(tag, pos, rot, scale, parent);
        }

        /// <summary>
        /// 根据池子的标签获取对象
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public static T Spawn<T>(string tag) where T : PoolObject
        {
            return Spawn<T>(tag, Vector3.zero, Quaternion.identity, Vector3.one, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public static T Spawn<T>(string tag, Vector3 pos) where T : PoolObject
        {
            return Spawn<T>(tag, pos, Quaternion.identity, Vector3.one, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置以及旋转
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <param name="rot">对象的旋转</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public static T Spawn<T>(string tag, Vector3 pos, Quaternion rot) where T : PoolObject
        {
            return Spawn<T>(tag, pos, rot, Vector3.one, null);
        }

        /// <summary>
        /// 根据池子的标签获取对象，并设置对象的位置、旋转以及缩放
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        /// <param name="pos">对象的位置</param>
        /// <param name="rot">对象的旋转</param>
        /// <param name="scale">对象的父物体</param>
        /// <returns>目标池子里可用的对象，没有可用对象会返回null</returns>
        public static T Spawn<T>(string tag, Vector3 pos, Quaternion rot, Vector3 scale) where T : PoolObject
        {
            return Spawn<T>(tag, pos, rot, scale, null);
        }

        /// <summary>
        /// 根据标签获取对应对象，并设置对象的位置、旋转、缩放和父物体
        /// </summary>
        public static T Spawn<T>(string tag, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent) where T : PoolObject
        {
            if (!ContainsPool(tag))
            {
                //GameObject prefab = ResourceManager.Load<GameObject>(tag + "Prefab");
                GameObject prefab = ResourceManager.Load<GameObject>(tag);
                CreatePool(tag, prefab);
            }

            return pools[tag].Spawn<T>(pos, rot, scale, parent);
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">要回收的对象</param>
        public static void Unspawn(PoolObject obj)
        {
            Unspawn(obj.Tag, obj);
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="tag">对象所在池子标签</param>
        /// <param name="obj">要回收的对象</param>
        public static void Unspawn(string tag, PoolObject obj)
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
                Debug.LogWarning("没有目标池子" + tag);
                return;
            }

            pools[tag].UnspawnAll();
        }

        /// <summary>
        /// 回收所有池子的所有对象
        /// </summary>
        public static void UnspawnAllPools()
        {
            foreach (var pool in pools.Values)
            {
                pool.UnspawnAll();
            }
        }

        public static void Clear(PoolObject obj)
        {
            Clear(obj.Tag, obj);
        }

        public static void Clear(string tag, PoolObject obj)
        {
            if (!ContainsPool(tag))
            {
                Debug.LogError("没有目标池子");
                return;
            }

            pools[tag].Clear(obj);
        }

        /// <summary>
        /// 清空目标池子
        /// </summary>
        /// <param name="tag">目标池子的标签</param>
        public static void ClearPool(string tag)
        {
            if (!ContainsPool(tag)) return;

            pools[tag].ClearAll();
        }

        /// <summary>
        /// 清空所有池子
        /// </summary>
        public static void ClearAllPool()
        {
            foreach (var pool in pools.Values)
            {
                pool.ClearAll();
            }
        }
    }
}
