using System;
using UnityEngine;

namespace TowerDefense
{
    public abstract class UIDataBase
    {
    }


    public abstract class UIBase : MonoBehaviour
    {
        public bool IsOpen { get; private set; } = false;

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

        protected virtual void OnInit(UIDataBase uiData) { }

        protected virtual void OnOpen(UIDataBase uiData) { }

        protected virtual void OnHide() { }

        protected virtual void OnClose() { }
    }
}