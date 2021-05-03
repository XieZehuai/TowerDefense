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


    /// <summary>
    /// 关卡成功UI
    /// </summary>
    public partial class UIStageSuccess : UIBase<UIStageSuccessData>
    {
        protected override void OnInit()
        {
            continueBtn.onClick.AddListener(OnContinueBtnClick);
            exitBtn.onClick.AddListener(OnExitBtnClick);
        }

        protected override void OnOpen()
        {
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
            //PlayerManager.Instance.NextStage();
            //GameManager.Instance.LoadGameScene();
            GameManager.Instance.LoadNextStage();
        }

        private void OnExitBtnClick()
        {
            GameManager.Instance.LoadMainScene();
        }
    }


    public partial class UIStageSuccess
    {
        [SerializeField] private Transform background = default;
        [SerializeField] private Image[] starImages = default;
        [SerializeField] private Button continueBtn = default;
        [SerializeField] private Button exitBtn = default;
    }
}
