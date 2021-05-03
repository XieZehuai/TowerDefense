using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// 选关界面上的按钮
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIStageBtn : PoolObject
    {
        [SerializeField] private Button button = default;
        [SerializeField] private Text text = default;
        [SerializeField] private Image[] starImages = default;

        private int stage;

        protected override void OnInstantiate()
        {
            button.onClick.AddListener(SelectStage);
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
            button.interactable = stage <= PlayerManager.Instance.Data.reachStage;

            for (int i = 0; i < starImages.Length; i++)
            {
                starImages[i].color = i < starCount ? Color.yellow : Color.gray;
            }
        }

        public override void OnReclaim()
        {
            button.onClick.RemoveAllListeners();
        }

        private void SelectStage()
        {
            GameManager.Instance.LoadStage(stage);
        }
    }
}
