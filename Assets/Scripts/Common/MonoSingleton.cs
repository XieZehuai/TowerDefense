using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// MonoBehaviour单例基类
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;

        /// <summary>
        /// 单例，如果没有对应的实例，会自动创建一个
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    obj.AddComponent(typeof(T));
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(gameObject);

            this.Invoke(OnInit, null);
        }

        /// <summary>
        /// 在单例初始化时调用
        /// </summary>
        protected virtual void OnInit() { }
    }
}
