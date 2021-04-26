using UnityEngine;

namespace TowerDefense
{
    public partial class UISelectStage : UIBase
    {
        private UIStageBtn[] btns;

        protected override void OnInit(UIDataBase uiData)
        {
            int stageCount = PlayerManager.Data.MaxStage;
            btns = new UIStageBtn[stageCount];

            Debug.Log(stageCount);
            for (int i = 0; i < stageCount; i++)
            {
                //UIStageBtn btn = ObjectPool.Spawn<UIStageBtn>("UIStageBtn");
                UIStageBtn btn = ObjectPool.Spawn<UIStageBtn>(Res.UIStageBtnPrefab);
                btn.transform.SetParent(content);
                int stage = i + 1;
                btn.SetData(stage, PlayerManager.GetStageStar(stage));

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
