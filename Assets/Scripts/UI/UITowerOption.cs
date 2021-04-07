using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class UITowerOptionData : UIDataBase
    {
        public Vector3 position; // 炮塔的位置

        public Action onUpgradeBtnClick;
        public Action onSellBtnClick;
        public Action onCancelBtnClick;

        public bool canUpgrade;
        public int upgradePrice;
        public int sellPrice;
    }


    public partial class UITowerOption : UIBase
    {
        protected override void OnInit(UIDataBase uiData)
        {
            data = uiData as UITowerOptionData ?? new UITowerOptionData();

            upgradeBtn.onClick.AddListener(OnUpgradeBtnClick);
            sellBtn.onClick.AddListener(OnSellBtnClick);
            cancelBtn.onClick.AddListener(OnCancelBtnClick);
        }

        protected override void OnOpen(UIDataBase uiData)
        {
            data = uiData as UITowerOptionData ?? new UITowerOptionData();

            InitPanelInfo();
        }

        protected override void OnHide()
        {
            data = null;
        }

        protected override void OnClose()
        {
            upgradeBtn.onClick.RemoveAllListeners();
            sellBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();
        }

        private void InitPanelInfo()
        {
            Vector2 pos = CameraController.Instance.Camera.WorldToScreenPoint(data.position);
            background.position = pos;

            upgradeBtn.interactable = data.canUpgrade;

            if (!data.canUpgrade && data.upgradePrice == -1)
            {
                upgradeBtnText.text = "满级";
            }
            else
            {
                upgradeBtnText.text = $"↑({data.upgradePrice})";
            }

            sellBtnText.text = $"$({data.sellPrice})";
        }

        private void OnUpgradeBtnClick()
        {
            data.onUpgradeBtnClick?.Invoke();
            Hide();
        }

        private void OnSellBtnClick()
        {
            data.onSellBtnClick?.Invoke();
            Hide();
        }

        private void OnCancelBtnClick()
        {
            data.onCancelBtnClick?.Invoke();
            Hide();
        }
    }


    public partial class UITowerOption
    {
        private UITowerOptionData data;

        [SerializeField] private Transform background = default;
        [SerializeField] private Button upgradeBtn = default;
        [SerializeField] private Button sellBtn = default;
        [SerializeField] private Button cancelBtn = default;
        [SerializeField] private Text upgradeBtnText = default;
        [SerializeField] private Text sellBtnText = default;

        protected override OpenAnim Anim => OpenAnim.Scale;

        protected override Transform AnimTransform => background;

        protected override float AnimDuration => 0.4f;

        protected override bool HideWhenClickOtherPlace => true;
    }
}
