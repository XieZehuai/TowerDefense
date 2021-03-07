using System;
using UnityEngine;

namespace TowerDefense
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
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

        private static T instance;

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(gameObject);

            OnInit();
        }

        /// <summary>
        /// 在单例初始化时调用
        /// </summary>
        protected virtual void OnInit()
        {
        }
    }
}
