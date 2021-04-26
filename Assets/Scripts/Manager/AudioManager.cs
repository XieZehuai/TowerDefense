using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class AudioManager : Singleton<AudioManager>
    {
        private readonly Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip>();

        [SerializeField] private AudioSource bgmPlayer = default;

        private bool audioOn;
        private bool musicOn;
        private float volume;

        public void TurnOnAudio()
        {

        }

        public void TurnOffAudio()
        {

        }

        public void ToggleAudio()
        {
            audioOn = !audioOn;
        }

        public void ToggleMusic()
        {
            musicOn = !musicOn;

            if (musicOn && !bgmPlayer.isPlaying)
            {
                bgmPlayer.Play();
            }
            else if (!musicOn && bgmPlayer.isPlaying)
            {
                bgmPlayer.Stop();
            }
        }

        public void SetVolume(float volume)
        {
            this.volume = Mathf.Clamp01(volume);
            bgmPlayer.volume = this.volume;
        }

        public void Play(Vector3 pos, string clipName)
        {
            if (!audioOn) return;

            if (HasAudioClip(clipName))
            {
                Play(pos, clipName, volume, audioClipDic[clipName].length, true, false);
            }
        }

        public void Play(Vector3 pos, string clipName, float duration)
        {
            if (!audioOn) return;

            if (HasAudioClip(clipName))
            {
                Play(pos, clipName, volume, duration, true, false);
            }
        }

        private void Play(Vector3 pos, string clipName, float volume, float duration, bool replay, bool loop)
        {
            ObjectPool.Spawn<AudioPlayer>(Res.AudioPlayerPrefab, pos).Play(audioClipDic[clipName], volume, duration, replay, loop);
        }

        private bool HasAudioClip(string clipName)
        {
            if (!audioClipDic.ContainsKey(clipName))
            {
                AudioClip clip = ResourceManager.Load<AudioClip>(clipName);

                if (clip == null)
                {
                    Debug.LogError("找不到音效文件：" + clipName);
                    return false;
                }

                audioClipDic.Add(clipName, clip);
            }

            return true;
        }
    }
}
