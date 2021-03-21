using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// MonoBehaviour单例
    /// </summary>
    /// <typeparam name="T">必须传入继承自这个类的类本身</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;

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
