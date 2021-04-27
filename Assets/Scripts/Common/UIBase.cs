using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// UI行为基类，定义UI拥有的基本方法
    /// </summary>
    public abstract class UIBehaviour : MonoBehaviour
    {
        /// <summary>
        /// UI是否已经打开
        /// </summary>
        public bool IsOpen { get; protected set; }

        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="uiData">参数</param>
        public abstract void Init(UIDataBase uiData);

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiData">参数</param>
        public abstract void Open(UIDataBase uiData);

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public abstract void Hide();

        /// <summary>
        /// 关闭UI
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 初始化UI时调用
        /// </summary>
        protected virtual void OnInit()
        {
        }

        /// <summary>
        /// 打开UI时调用
        /// </summary>
        protected virtual void OnOpen()
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


    /// <summary>
    /// 传递给UI的参数的基类
    /// </summary>
    public class UIDataBase
    {
    }


    /// <summary>
    /// 无自定义参数类型的UI的基类
    /// </summary>
    public abstract class UIBase : UIBase<UIDataBase>
    {
    }


    /// <summary>
    /// 自定义参数类型的UI的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UIBase<T> : UIBehaviour where T : UIDataBase, new()
    {
        protected T data; // 传递给UI的参数

        public override void Init(UIDataBase uiData)
        {
            data = uiData as T ?? new T();
            gameObject.SetActive(false);
            OnInit();
        }

        public override void Open(UIDataBase uiData)
        {
            if (IsOpen) return;

            data = uiData as T ?? new T();
            IsOpen = true;
            gameObject.SetActive(true);
            OnOpen();
        }

        public override void Hide()
        {
            if (!IsOpen) return;

            OnHide();
            gameObject.SetActive(false);
            IsOpen = false;
        }

        public override void Close()
        {
            if (IsOpen)
            {
                Hide();
            }

            OnClose();
        }
    }
}
