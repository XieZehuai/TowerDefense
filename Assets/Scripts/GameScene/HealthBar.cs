using UnityEngine;

namespace TowerDefense
{
    public class HealthBar : PoolObject
    {
        [SerializeField] private Transform health = default;

        private bool isFollowing = false;
        private Transform followTarget = null;
        private Vector3 offset;

        private void Update()
        {
            if (isFollowing)
            {
                transform.localPosition = followTarget.localPosition + offset; // 跟随目标移动
                transform.localRotation = CameraController.Instance.transform.localRotation; // 面向相机
            }
        }

        /// <summary>
        /// 设置血条的值
        /// </summary>
        /// <param name="value">范围0 ~ 1</param>
        public void SetValue(float value)
        {
            Vector3 scale = health.localScale;
            scale.x = Mathf.Clamp(value, 0f, 1f);
            health.localScale = scale;
        }

        public void Follow(Transform followTarget, Vector3 offset)
        {
            isFollowing = true;
            this.followTarget = followTarget;
            this.offset = offset;
        }

        public void NotFollow()
        {
            isFollowing = false;
            followTarget = null;
            offset = Vector3.zero;
        }

        public override void OnUnspawn()
        {
            NotFollow();
        }

        public override void OnReclaim()
        {
            NotFollow();
        }
    }
}
