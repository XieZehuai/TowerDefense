using UnityEngine;

namespace TowerDefense
{
    public class Particle : PoolObject
    {
        private GameObject vfxObj;

        private bool isFollowing = false;
        private Transform followTarget = null;
        private Vector3 offset;

        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            vfxObj = transform.GetChild(0).gameObject;
            if (vfxObj == null)
            {
                Debug.LogError("没有对应的特效");
            }
        }

        private void Update()
        {
            if (isFollowing)
            {
                transform.localPosition = followTarget.localPosition + offset;
            }
        }

        public void Play()
        {
            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;
            vfxObj.SetActive(true);
        }

        public void Replay()
        {
            IsPlaying = true;
            vfxObj.SetActive(false);
            vfxObj.SetActive(true);
        }

        public void Stop()
        {
            if (IsPlaying)
            {
                return;
            }

            vfxObj.SetActive(false);
        }

        public Particle Follow(Transform followTarget, Vector3 offset)
        {
            isFollowing = true;
            this.followTarget = followTarget;
            this.offset = offset;

            return this;
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
