using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 选关界面UI
    /// </summary>
    public partial class UISelectStage : UIBase
    {
        private UIStageBtn[] btns;

        protected override void OnInit()
        {
            int stageCount = Mathf.Max(GameManager.Instance.MaxStageCount, PlayerManager.Instance.Data.MaxStage);
            btns = new UIStageBtn[stageCount];

            for (int i = 0; i < stageCount; i++)
            {
                UIStageBtn btn = ObjectPool.Spawn<UIStageBtn>(Res.UIStageBtnPrefab);
                btn.transform.SetParent(content);
                int stage = i + 1;
                btn.SetData(stage, PlayerManager.Instance.GetStageStar(stage));

                btns[i] = btn;
            }
        }

        protected override void OnClose()
        {
            for (int i = 0; i < btns.Length; i++)
            {
                ObjectPool.Unspawn(btns[i]);
            }

            btns = null;
        }
    }


    public partial class UISelectStage
    {
        [SerializeField] private Transform content = default;
    }
}
