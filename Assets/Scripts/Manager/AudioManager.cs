using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// 管理游戏内的所有音效
    /// </summary>
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private readonly Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip>(); // 保存加载的音效文件

        [SerializeField] private AudioSource bgmPlayer; // 背景音乐播放器

        /// <summary>
        /// 是否打开了游戏音效
        /// </summary>
        public bool IsAudioOn { get; private set; }

        /// <summary>
        /// 是否打开了背景音乐
        /// </summary>
        public bool IsMusicOn { get; private set; }

        /// <summary>
        /// 游戏音效的音量
        /// </summary>
        public float AudioVolume { get; private set; }

        /// <summary>
        /// 背景音乐的音量
        /// </summary>
        public float MusicVolume { get; private set; }

        protected override void OnInit()
        {
            // 获取音量设置
            float audioVolume = PlayerPrefs.GetFloat("AudioVolume", 1f);
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            // 设置音量
            SetAudioVolume(audioVolume);
            SetMusicVolume(musicVolume);
            // 打开音效
            TurnOnAudio();
            TurnOnMusic();
        }

        /// <summary>
        /// 打开游戏音效
        /// </summary>
        public void TurnOnAudio()
        {
            IsAudioOn = true;
        }

        /// <summary>
        /// 关闭游戏音效
        /// </summary>
        public void TurnOffAudio()
        {
            IsAudioOn = false;
            ObjectPool.UnspawnAll(Res.AudioPlayerPrefab);
        }

        /// <summary>
        /// 打开背景音乐
        /// </summary>
        public void TurnOnMusic()
        {
            IsMusicOn = true;

            if (!bgmPlayer.isPlaying)
            {
                bgmPlayer.Play();
            }
        }

        /// <summary>
        /// 关闭背景音乐
        /// </summary>
        public void TurnOffMusic()
        {
            IsMusicOn = false;

            if (bgmPlayer.isPlaying)
            {
                bgmPlayer.Stop();
            }
        }

        /// <summary>
        /// 设置游戏音效音量
        /// </summary>
        /// <param name="volume">音量大小，范围0 ~ 1</param>
        public void SetAudioVolume(float volume)
        {
            AudioVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("AudioVolume", AudioVolume);
        }

        /// <summary>
        /// 设置背景音乐音量
        /// </summary>
        /// <param name="volume">音量大小，范围0 ~ 1</param>
        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            bgmPlayer.volume = MusicVolume;
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="pos">播放位置</param>
        /// <param name="clipName">音效文件名</param>
        public void Play(Vector3 pos, string clipName)
        {
            if (!IsAudioOn) return;

            if (HasAudioClip(clipName))
            {
                Play(pos, clipName, AudioVolume, audioClipDic[clipName].length, true, false);
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="pos">播放位置</param>
        /// <param name="clipName">音效文件名</param>
        /// <param name="duration">播放时长</param>
        public void Play(Vector3 pos, string clipName, float duration)
        {
            if (!IsAudioOn) return;

            if (HasAudioClip(clipName))
            {
                Play(pos, clipName, AudioVolume, duration, true, false);
            }
        }

        // 播放音效
        private void Play(Vector3 pos, string clipName, float volume, float duration, bool replay, bool loop)
        {
            ObjectPool.Spawn<AudioPlayer>(Res.AudioPlayerPrefab, pos).Play(audioClipDic[clipName], volume, duration, replay, loop);
        }

        // 判断是否有音效文件
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
