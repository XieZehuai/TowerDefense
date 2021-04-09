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
                UIStageBtn btn = ObjectPool.Spawn<UIStageBtn>("UIStageBtn");
                btn.transform.SetParent(content);
                btn.Stage = i + 1;

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
