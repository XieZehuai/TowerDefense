using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public enum UILayer
    {
        Background,
        Common,
        Foreground,
    }


    public class UIManager : MonoSingleton<UIManager>
    {
        private Dictionary<Type, UIBase> uiDic = new Dictionary<Type, UIBase>();

        [SerializeField] private Transform backgroundLayer = default;
        [SerializeField] private Transform commonLayer = default;
        [SerializeField] private Transform foregroundLayer = default;

        /// <summary>
        /// 根据类型打开UI
        /// </summary>
        /// <typeparam name="T">UI的类型，必须继承自Panel</typeparam>
        /// <param name="data">要传入的数据，必须继承自PanelData，可以为空</param>
        /// <returns>UI对象</returns>
        public T Open<T>(UIDataBase data = null, UILayer layer = UILayer.Common) where T : UIBase
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
                case UILayer.Background: parent = backgroundLayer; return;
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
        /// <returns>成功获取到返回对应的实力，失败返回null</returns>
        public T Get<T>() where T : UIBase
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
        public void Hide<T>() where T : UIBase
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
        public void Close<T>() where T : UIBase
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
            foreach (UIBase ui in uiDic.Values)
            {
                ui.Close();
                Destroy(ui.gameObject);
            }

            uiDic.Clear();
        }
    }
}
