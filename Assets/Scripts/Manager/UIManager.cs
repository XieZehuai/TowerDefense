using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense
{
    /// <summary>
    /// UI面板的显示层级
    /// </summary>
    public enum UILayer
    {
        Background,
        Common,
        Foreground,
    }


    /// <summary>
    /// UI管理器，管理游戏内所有继承自UIBehaviour的UI
    /// </summary>
    public class UIManager : MonoSingleton<UIManager>
    {
        private readonly Dictionary<Type, UIBehaviour> uiDic = new Dictionary<Type, UIBehaviour>();

        [SerializeField] private Transform backgroundLayer = default;
        [SerializeField] private Transform commonLayer = default;
        [SerializeField] private Transform foregroundLayer = default;

        /// <summary>
        /// 当前鼠标是否处于UI上
        /// </summary>
        public bool IsMouseOverUI { get; private set; }

        private void Update()
        {
            IsMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// 根据类型打开UI
        /// </summary>
        /// <typeparam name="T">UI的类型，必须继承自UIBehaviour</typeparam>
        /// <param name="data">要传入的数据，必须继承自UIDataBase，可以为空</param>
        /// <returns>打开后的UI的引用</returns>
        public T Open<T>(UIDataBase data = null, UILayer layer = UILayer.Common) where T : UIBehaviour
        {
            Type type = typeof(T);

            if (!uiDic.ContainsKey(type))
            {
                T ui = ResourceManager.Load<T>(type.Name);
                ui = Instantiate(ui);
                SetLayer(ui.transform, layer);
                ui.Init(data);
                uiDic.Add(type, ui);
            }

            uiDic[type].Open(data);

            return uiDic[type] as T;
        }

        // 设置UI的层级
        private void SetLayer(Transform trans, UILayer layer)
        {
            Transform parent = commonLayer;

            switch (layer)
            {
                case UILayer.Background: parent = backgroundLayer; break;
                case UILayer.Common: parent = commonLayer; break;
                case UILayer.Foreground: parent = foregroundLayer; break;
            }

            trans.SetParent(parent);
            trans.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            trans.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        }

        /// <summary>
        /// 获取UI实例
        /// </summary>
        /// <typeparam name="T">UI的类型</typeparam>
        /// <returns>获取成功返回对象的引用，失败返回null</returns>
        public T Get<T>() where T : UIBehaviour
        {
            Type type = typeof(T);

            if (!uiDic.ContainsKey(type))
            {
                Debug.LogError("找不到目标UI：" + type);
                return null;
            }

            return uiDic[type] as T;
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void Hide<T>() where T : UIBehaviour
        {
            Type type = typeof(T);

            if (!uiDic.ContainsKey(type))
            {
                Debug.LogError("找不到目标UI：" + type);
                return;
            }

            uiDic[type].Hide();
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public void Close<T>() where T : UIBehaviour
        {
            Type type = typeof(T);

            if (!uiDic.ContainsKey(type))
            {
                Debug.LogError("找不到目标UI：" + type);
                return;
            }

            uiDic[type].Close();
            Destroy(uiDic[type].gameObject);
            uiDic.Remove(type);
        }

        /// <summary>
        /// 关闭场景中所有UI
        /// </summary>
        public void CloseAll()
        {
            foreach (UIBehaviour ui in uiDic.Values)
            {
                ui.Close();
                Destroy(ui.gameObject);
            }

            uiDic.Clear();
        }
    }
}
