using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class UITowerOptionData : UIDataBase
    {
        public Tower tower;
    }


    /// <summary>
    /// 炮塔选项UI
    /// </summary>
    public partial class UITowerOption : UIBase<UITowerOptionData>
    {
        private bool trackTower;

        protected override void OnInit()
        {
            upgradeBtn.onClick.AddListener(OnUpgradeBtnClick);
            sellBtn.onClick.AddListener(OnSellBtnClick);
            cancelBtn.onClick.AddListener(OnCancelBtnClick);

            TypeEventSystem.Register<OnReplay>(OnReplay);
        }

        protected override void OnOpen()
        {
            SetPosition();
            InitPanelInfo();
            background.localScale = Vector3.zero;
            background.DOScale(Vector3.one, 0.2f);
            trackTower = true;
        }

        protected override void OnHide()
        {
            data = null;
            trackTower = false;
        }

        protected override void OnClose()
        {
            upgradeBtn.onClick.RemoveAllListeners();
            sellBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();

            TypeEventSystem.UnRegister<OnReplay>(OnReplay);
        }

        private void Update()
        {
            if (trackTower)
            {
                SetPosition();
            }
        }

        private void SetPosition()
        {
            Vector2 pos = CameraController.Instance.Camera.WorldToScreenPoint(data.tower.LocalPosition);
            background.position = pos;
        }

        private void InitPanelInfo()
        {
            Tower tower = data.tower;

            var result = tower.CanUpgrade();
            upgradeBtn.interactable = result.Item1;

            if (!result.Item1 && result.Item2 == -1)
            {
                upgradeBtnText.text = "满级";
            }
            else
            {
                upgradeBtnText.text = $"↑({result.Item2})";
            }

            sellBtnText.text = $"$({tower.GetSellPrice()})";
        }

        private void OnUpgradeBtnClick()
        {
            data.tower.Upgrade();
            data.tower.ShowAttackRange(data.tower.Data.LevelData.attackRange);
            InitPanelInfo();
        }

        private void OnSellBtnClick()
        {
            data.tower.Sell();
            Hide();
        }

        private void OnCancelBtnClick()
        {
            data.tower.HideAttackRange();
            background.DOScale(Vector3.zero, 0.2f).OnComplete(Hide);
        }

        private void OnReplay(OnReplay context)
        {
            Hide();
        }
    }


    public partial class UITowerOption
    {
        [SerializeField] private Transform background = default;
        [SerializeField] private Button upgradeBtn = default;
        [SerializeField] private Button sellBtn = default;
        [SerializeField] private Button cancelBtn = default;
        [SerializeField] private Text upgradeBtnText = default;
        [SerializeField] private Text sellBtnText = default;
    }
}
