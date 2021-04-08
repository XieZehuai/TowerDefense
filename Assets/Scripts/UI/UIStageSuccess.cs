using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TowerDefense
{
    public partial class UIStageSuccess : UIBase
    {
        protected override void OnInit(UIDataBase uiData)
        {
            continueBtn.onClick.AddListener(OnContinueBtnClick);
            exitBtn.onClick.AddListener(OnExitBtnClick);
        }

        protected override void OnOpen(UIDataBase uiData)
        {
            background.localScale = Vector3.zero;
            background.DOScale(Vector3.one, 0.5f);
        }

        protected override void OnHide()
        {
            background.DOScale(Vector3.zero, 0.5f);
        }

        protected override void OnClose()
        {
            continueBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
            transform.DOKill();
        }

        private void OnContinueBtnClick()
        {
            PlayerManager.NextStage();
            GameManager.Instance.LoadGameScene();
        }

        private void OnExitBtnClick()
        {
            GameManager.Instance.LoadMainScene();
        }
    }


    public partial class UIStageSuccess
    {
        [SerializeField] private Transform background = default;

        [SerializeField] private Button continueBtn = default;
        [SerializeField] private Button exitBtn = default;
    }
}
