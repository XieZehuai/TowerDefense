using System;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// 设置界面UI
    /// </summary>
    public partial class UISettingPanel : UIBase
    {
        protected override void OnInit()
        {
            closeBtn.onClick.AddListener(OnCloseBtnClick);
            audioVolumeSlider.onValueChanged.AddListener(OnAudioSliderValueChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        }

        protected override void OnOpen()
        {
            audioVolumeSlider.value = AudioManager.Instance.AudioVolume;
            musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
        }

        protected override void OnClose()
        {
            closeBtn.onClick.RemoveAllListeners();
            audioVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
        }

        private void OnCloseBtnClick()
        {
            Hide();
        }

        private void OnAudioSliderValueChanged(float value)
        {
            AudioManager.Instance.SetAudioVolume(value);
        }

        private void OnMusicSliderValueChanged(float value)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }


	public partial class UISettingPanel
    {
		[SerializeField] private Button closeBtn = default;
		[SerializeField] private Slider audioVolumeSlider = default;
		[SerializeField] private Slider musicVolumeSlider = default;
    }
}
