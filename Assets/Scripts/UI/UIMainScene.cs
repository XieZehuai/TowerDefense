using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// 主界面UI
    /// </summary>
    public partial class UIMainScene : UIBase
    {
        protected override void OnInit()
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
            UIManager.Instance.Open<UISelectStage>(layer: UILayer.Foreground);
        }

        private void OnSettingBtnClick()
        {
            UIManager.Instance.Open<UISettingPanel>();
        }

        private void OnExitBtnClick()
        {
            GameManager.Instance.QuitGame();
        }
    }


    public partial class UIMainScene
    {
        [SerializeField] private Button startBtn = default;
        [SerializeField] private Button settingBtn = default;
        [SerializeField] private Button exitBtn = default;
    }
}
