using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TowerDefense
{
    public class UIStageSuccessData : UIDataBase
    {
        public int stage;
        public int starCount;
    }


    public partial class UIStageSuccess : UIBase
    {
        protected override void OnInit(UIDataBase uiData)
        {
            continueBtn.onClick.AddListener(OnContinueBtnClick);
            exitBtn.onClick.AddListener(OnExitBtnClick);
        }

        protected override void OnOpen(UIDataBase uiData)
        {
            data = uiData as UIStageSuccessData ?? new UIStageSuccessData();

            for (int i = 0; i < starImages.Length; i++)
            {
                starImages[i].color = i < data.starCount ? Color.yellow : Color.gray;
            }

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
            background.DOKill();
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
        private UIStageSuccessData data;

        [SerializeField] private Transform background = default;
        [SerializeField] private Image[] starImages = default;
        [SerializeField] private Button continueBtn = default;
        [SerializeField] private Button exitBtn = default;
    }
}
