using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    [RequireComponent(typeof(Button))]
    public class UIStageBtn : PoolObject
    {
        [SerializeField] private Button button = default;
        [SerializeField] private Text text = default;
        [SerializeField] private Image[] starImages = default;

        private int stage;

        protected override void OnInstantiate()
        {
            button.onClick.AddListener(ChangeStage);
        }
        
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="stage">关卡数</param>
        /// <param name="starCount">关卡评分</param>
        public void SetData(int stage, int starCount)
        {
            this.stage = stage;
            text.text = $"第 {stage} 关";
            button.interactable = stage <= PlayerManager.Data.ReachStage;

            for (int i = 0; i < starImages.Length; i++)
            {
                starImages[i].color = i < starCount ? Color.yellow : Color.gray;
            }
        }

        public override void OnReclaim()
        {
            button.onClick.RemoveAllListeners();
        }

        private void ChangeStage()
        {
            PlayerManager.ChangeCurrentStage(stage);
            GameManager.Instance.LoadGameScene();
        }
    }
}
