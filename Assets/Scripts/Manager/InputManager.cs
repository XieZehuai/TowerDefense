using UnityEngine;

namespace TowerDefense
{
    public class InputManager : SubStageManager
    {
        public MapObjectType GridType { get; set; } = MapObjectType.Wall; // 当前选择的格子类型

        public int TowerId { get; set; } = -1; // 当前选择的塔的ID

        private float undoTimer; // 连续撤销操作的计时器

        public InputManager(StageManager stageManager) : base(stageManager)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            ControlCamera(); // 控制相机运动

            if (manager.IsPreparing || manager.IsPlaying)
            {
                if (!UIManager.Instance.IsMouseOverUI)
                {
                    ChangeMap();
                    PlaceTower();
                }

                UndoChangeMap();
            }
        }

        #region 玩家鼠标控制事件，通过鼠标点击地图触发

        // 修改地图
        private void ChangeMap()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = Utils.GetMousePosition();
                manager.MapManager.ChangeGridType(pos, GridType, true);
            }
        }

        // 撤销对地图的修改
        private void UndoChangeMap()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Z))
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    manager.MapManager.Undo();
                    undoTimer = 0f;
                }
                else
                {
                    if (undoTimer < Utils.UNDO_PRESS_DURATION)
                    {
                        undoTimer += Time.deltaTime;
                    }
                    else
                    {
                        manager.MapManager.Undo();
                    }
                }
            }
        }

        // 摆放炮塔
        private void PlaceTower()
        {
            if (Input.GetMouseButtonDown(1) && TowerId != -1)
            {
                manager.TowerManager.PlaceTower(Utils.GetMousePosition(), TowerId);
                TowerId = -1;
            }
        }

        // 控制相机运动
        private void ControlCamera()
        {
            CameraController.Instance.Zoom(Input.GetAxis("Mouse ScrollWheel"));

            if (Input.GetMouseButton(2))
            {
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                CameraController.Instance.Rotate(x, y);
            }

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 movement = new Vector2(h, v).normalized;
            CameraController.Instance.Move(movement);
        }

        #endregion
    }
}