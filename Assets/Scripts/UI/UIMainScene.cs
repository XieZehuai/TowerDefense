using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TowerDefense
{
    public class UIMainSceneData : UIDataBase
    {
    }


    public partial class UIMainScene : UIBase
    {
        protected override void OnInit(UIDataBase uiData)
        {
            startBtn.onClick.AddListener(OnStartBtnClick);
            settingBtn.onClick.AddListener(OnSettingBtnClick);
            exitBtn.onClick.AddListener(OnExitBtnClick);
        }

        protected override void OnClose()
        {
            startBtn.onClick.RemoveAllListeners();
            settingBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
        }

        private void OnStartBtnClick()
        {
            GameManager.Instance.LoadGameScene();
        }

        private void OnSettingBtnClick()
        {
        }

        private void OnExitBtnClick()
        {
        }
    }


    public partial class UIMainScene
    {
        [SerializeField] private Button startBtn = default;
        [SerializeField] private Button settingBtn = default;
        [SerializeField] private Button exitBtn = default;
    }
}
