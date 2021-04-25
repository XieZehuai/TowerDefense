using UnityEngine;

namespace TowerDefense
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : PoolObject
    {
        private bool isPlaying;
        private AudioSource audioSource;

        protected override void OnInstantiate()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip audioClip, float duration)
        {
            Play(audioClip, duration, false, 1f, true);
        }

        public void Play(AudioClip audioClip, bool repeat, float volume, bool replay)
        {
            if (!replay && isPlaying) return;

            isPlaying = true;
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.loop = repeat;
            audioSource.Play();
        }

        public void Play(AudioClip audioClip, float duration, bool repeat, float volume, bool replay)
        {
            if (!replay && isPlaying) return;

            isPlaying = true;
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.loop = repeat;
            audioSource.Play();

            if (!repeat)
            {
                DelayUnspawn(duration);
            }
        }

        public void Stop()
        {
            if (!isPlaying) return;

            isPlaying = false;
            audioSource.Stop();
        }

        public override void OnUnspawn()
        {
            Stop();
        }
    }
}