using System;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    [RequireComponent(typeof(Button))]
    public class UIStageBtn : PoolObject
    {
        private Button button;
        private Text text;
        private int stage;

        public int Stage
        {
            get => stage;
            set
            {
                stage = value;
                text.text = $"第 {stage} 关";
                button.interactable = stage <= PlayerManager.Data.MaxStage;
            }
        }

        protected override void OnInstantiate()
        {
            button = GetComponent<Button>();
            text = GetComponentInChildren<Text>();

            button.onClick.AddListener(ChangeStage);
        }

        public override void OnReclaim()
        {
            button.onClick.RemoveAllListeners();
        }

        private void ChangeStage()
        {
            PlayerManager.ChangeStage(stage);
            GameManager.Instance.LoadGameScene();
        }
    }
}
