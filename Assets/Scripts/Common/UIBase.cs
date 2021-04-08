using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense
{
    /// <summary>
    /// 传递给UI的参数的基类
    /// </summary>
    public abstract class UIDataBase
    {
    }


    /// <summary>
    /// 所有UI的基类
    /// </summary>
    public abstract class UIBase : MonoBehaviour
    {
        private static readonly List<RaycastResult> raycastResult = new List<RaycastResult>();

        public bool IsOpen { get; private set; }

        public void Init(UIDataBase uiData)
        {
            gameObject.SetActive(false);
            OnInit(uiData);
        }

        public void Open(UIDataBase uiData)
        {
            if (IsOpen) return;

            IsOpen = true;
            gameObject.SetActive(true);
            OnOpen(uiData);
        }

        public void Hide()
        {
            if (!IsOpen) return;

            OnHide();
            gameObject.SetActive(false);
            IsOpen = false;
        }

        public void Close()
        {
            if (IsOpen)
            {
                Hide();
            }

            OnClose();
        }

        /// <summary>
        /// 在UI被加载时调用
        /// </summary>
        /// <param name="uiData">要传递的参数</param>
        protected virtual void OnInit(UIDataBase uiData)
        {
        }

        /// <summary>
        /// 打开UI时调用
        /// </summary>
        /// <param name="uiData">要传递的参数</param>
        protected virtual void OnOpen(UIDataBase uiData)
        {
        }

        /// <summary>
        /// 隐藏UI时调用
        /// </summary>
        protected virtual void OnHide()
        {
        }

        /// <summary>
        /// 关闭UI时调用
        /// </summary>
        protected virtual void OnClose()
        {
        }
    }
}