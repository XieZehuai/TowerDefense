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
                transform.localPosition = followTarget.localPosition + offset;
            }

            Vector3 pos = Camera.main.WorldToScreenPoint(transform.localPosition);
            Debug.Log(pos);
            transform.LookAt(pos);
        }

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
