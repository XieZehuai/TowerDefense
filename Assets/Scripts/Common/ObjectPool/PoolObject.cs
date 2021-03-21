using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 可用对象池管理的对象
    /// </summary>
    public class PoolObject : MonoBehaviour
    {
        public string Tag { get; private set; }

        private bool isDestroy = false;

        public void Init(string tag, bool destroyOnLoad = true)
        {
            Tag = tag;

            if (!destroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Destroy()
        {
            if (isDestroy)
            {
                Debug.LogError("物体已经被回收" + Tag);
                return;
            }

            isDestroy = true;
            ObjectPool.Clear(Tag, this);
        }

        /// <summary>
        /// 从对象池中取出时调用
        /// </summary>
        public virtual void OnSpawn() { }

        /// <summary>
        /// 回收进对象池时调用
        /// </summary>
        public virtual void OnUnspanw() { }

        /// <summary>
        /// 从对象池中移除时调用
        /// </summary>
        public virtual void OnReclaim() { }

        private void OnDestroy()
        {
            Destroy();
        }
    }
}
