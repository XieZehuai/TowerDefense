using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TowerDefense
{
    public class UIStageFailedData : UIDataBase
    {
        public Action onReplayBtnClick;
    }


    public partial class UIStageFailed : UIBase
    {
        protected override void OnInit(UIDataBase uiData)
        {
            replayBtn.onClick.AddListener(OnReplayBtnClick);
            exitBtn.onClick.AddListener(OnExitBtnClick);
        }

        protected override void OnOpen(UIDataBase uiData)
        {
            data = uiData as UIStageFailedData ?? new UIStageFailedData();

            background.localScale = Vector3.zero;
            background.DOScale(Vector3.one, 0.5f);
        }

        protected override void OnHide()
        {
            data = null;
        }

        protected override void OnClose()
        {
            replayBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
            background.DOKill();
        }

        private void OnReplayBtnClick()
        {
            data.onReplayBtnClick?.Invoke();
            background.DOScale(Vector3.zero, 0.5f).OnComplete(Hide);
        }

        private void OnExitBtnClick()
        {
            GameManager.Instance.LoadMainScene();
        }
    }


    public partial class UIStageFailed
    {
        private UIStageFailedData data;

        [SerializeField] private Transform background = default;

        [SerializeField] private Button replayBtn = default;
        [SerializeField] private Button exitBtn = default;
    }
}
