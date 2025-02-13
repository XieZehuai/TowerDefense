﻿using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class UIPausePanelData : UIDataBase
    {
        public StageManager manager;
    }


    /// <summary>
    /// 暂停界面UI
    /// </summary>
    public partial class UIPausePanel : UIBase<UIPausePanelData>
    {
        private StageManager manager;

        protected override void OnInit()
        {
            manager = data.manager;

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
            manager.Continue();
            Hide();
        }

        private void OnReplayBtnClick()
        {
            manager.Replay();
            Hide();
        }

        private void OnOptionsBtnClick()
        {
            UIManager.Instance.Open<UISettingPanel>(layer: UILayer.Foreground);
        }

        private void OnExitBtnClick()
        {
            GameManager.Instance.LoadMainScene();
        }
    }


    public partial class UIPausePanel
    {
        [SerializeField] private Button resumeBtn = default;
        [SerializeField] private Button replayBtn = default;
        [SerializeField] private Button optionsBtn = default;
        [SerializeField] private Button exitBtn = default;
    }
}
