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
    [AddComponentMenu("UI/TowerTypeButton", 30)]
    public class TowerTypeButton : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
    {
        [Header("炮塔的ID")]
        [SerializeField] private int towerId;

        [Serializable]
        public class ButtonClickedEvent : UnityEvent<int>
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

        protected TowerTypeButton()
        {
        }

        private void Press()
        {
            if (IsActive() && IsInteractable())
            {
                UISystemProfilerApi.AddMarker("Button.onClick", this);
                onClick.Invoke(towerId);
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
