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
			
			SetPosition();
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

		private void SetPosition()
		{
			Vector2 pos = CameraController.Instance.Camera.WorldToScreenPoint(data.position);
			background.position = pos;

			background.localScale = Vector3.zero;
			background.DOScale(Vector3.one, 0.5f);
		}
		
		private void OnUpgradeBtnClick()
		{
			data.onUpgradeBtnClick?.Invoke();
		}

		private void OnSellBtnClick()
		{
			data.onSellBtnClick?.Invoke();
		}

		private void OnCancelBtnClick()
		{
			data.onCancelBtnClick?.Invoke();
			Hide();
		}
	}
	
	
	public partial class  UITowerOption
	{
		private UITowerOptionData data;

		[SerializeField] private Transform background = default;
		[SerializeField] private Button upgradeBtn = default;
		[SerializeField] private Button sellBtn = default;
		[SerializeField] private Button cancelBtn = default;
	}
}
