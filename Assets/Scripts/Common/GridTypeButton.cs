using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// 自定义Button组件
    /// </summary>
    [AddComponentMenu("UI/GridTypeButton", 30)]
    public class GridTypeButton : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
    {
        [Header("点击按钮时选择的格子类型")]
        [SerializeField] private MapObjectType type = MapObjectType.Empty;

        [Serializable]
        public class ButtonClickedEvent : UnityEvent<MapObjectType>
        {
        }

        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent onClick = new ButtonClickedEvent();

        public ButtonClickedEvent OnClick
        {
            get
            {
                return onClick;
            }
            set
            {
                onClick = value;
            }
        }

        protected GridTypeButton()
        {
        }

        private void Press()
        {
            if (IsActive() && IsInteractable())
            {
                UISystemProfilerApi.AddMarker("Button.onClick", this);
                onClick.Invoke(type);
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Press();
            }
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();
            if (IsActive() && IsInteractable())
            {
                DoStateTransition(SelectionState.Pressed, false);
                StartCoroutine(OnFinishSubmit());
            }
        }

        private IEnumerator OnFinishSubmit()
        {
            float fadeTime = colors.fadeDuration;
            float elapsedTime = 0f;
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            DoStateTransition(currentSelectionState, false);
        }
    }
}
