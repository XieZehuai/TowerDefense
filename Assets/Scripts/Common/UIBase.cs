using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense
{
    public abstract class UIDataBase
    {
    }


    /// <summary>
    /// UI的基类
    /// </summary>
    public abstract class UIBase : MonoBehaviour
    {
        private static readonly List<RaycastResult> raycastResult = new List<RaycastResult>();

        /// <summary>
        /// UI打开和关闭的动画
        /// </summary>
        protected enum OpenAnim
        {
            None,
            Scale,
        }


        public bool IsOpen { get; private set; }

        protected virtual OpenAnim Anim => OpenAnim.None;

        protected virtual Transform AnimTransform => transform.GetChild(0);

        protected virtual float AnimDuration => 0.5f;

        protected virtual bool HideWhenClickOtherPlace => false;

        private bool canHideSelf;

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

            if (Anim == OpenAnim.Scale)
            {
                AnimTransform.localScale = Vector3.zero;
                AnimTransform.DOScale(Vector3.one, AnimDuration).OnComplete(() => { canHideSelf = true; });
            }
            else
            {
                this.Invoke(() => canHideSelf = true, null);
            }
        }

        public void Hide()
        {
            if (!IsOpen) return;

            canHideSelf = false;

            if (Anim == OpenAnim.Scale)
            {
                AnimTransform.DOScale(Vector3.zero, AnimDuration).OnComplete(() =>
                {
                    OnHide();
                    gameObject.SetActive(false);
                    IsOpen = false;
                });
            }
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

        private void Update()
        {
            if (HideWhenClickOtherPlace && canHideSelf)
            {
                HideSelf();
            }

            if (IsOpen)
            {
                OnUpdate();
            }
        }

        private void HideSelf()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    PointerEventData eventData = new PointerEventData(EventSystem.current)
                    {
                        position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
                    };
                    EventSystem.current.RaycastAll(eventData, raycastResult);

                    if (raycastResult.Exists(item => item.gameObject == gameObject))
                    {
                        return;
                    }
                }
                
                Hide();
            }
        }

        protected virtual void OnUpdate()
        {
        }
    }
}