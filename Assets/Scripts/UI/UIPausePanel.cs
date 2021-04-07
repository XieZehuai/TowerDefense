using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public partial class UIPausePanel : UIBase
    {
        protected override void OnInit(UIDataBase uiData)
        {
            resumeBtn.onClick.AddListener(OnResumeBtnClick);
            replayBtn.onClick.AddListener(OnReplayBtnClick);
            optionsBtn.onClick.AddListener(OnOptionsBtnClick);
            exitBtn.onClick.AddListener(OnExitBtnClick);
        }

        protected override void OnClose()
        {
            resumeBtn.onClick.RemoveAllListeners();
            replayBtn.onClick.RemoveAllListeners();
            optionsBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
        }

        private void OnResumeBtnClick()
        {
            TypeEventSystem.Send(new ContinueGame());
            Hide();
        }

        private void OnReplayBtnClick()
        {
            TypeEventSystem.Send(new ReplayGame());
            Hide();
        }

        private void OnOptionsBtnClick()
        {

        }

        private void OnExitBtnClick()
        {
            UIManager.Instance.CloseAll();
            GameManager.Instance.LoadMainScene();
        }
    }

    public partial class UIPausePanel
    {
        [SerializeField] private Button resumeBtn = default;
        [SerializeField] private Button replayBtn = default;
        [SerializeField] private Button optionsBtn = default;
        [SerializeField] private Button exitBtn = default;
        
        protected override OpenAnim Anim => OpenAnim.Scale;

        protected override float AnimDuration => 0.4f;
    }
}
