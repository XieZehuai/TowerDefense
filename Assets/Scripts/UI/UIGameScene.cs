using System;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class UIGameSceneData : UIDataBase
    {
        public int maxHp;
        public int coins;
        public int maxWaveCount;
    }


    public partial class UIGameScene : UIBase
    {
        private bool firstWave;

        protected override void OnInit(UIDataBase uiData)
        {
            data = uiData as UIGameSceneData ?? new UIGameSceneData();

            OnReplay(new OnReplay());

            spawnBtn.onClick.AddListener(OnSpawnBtnClick);
            pauseBtn.onClick.AddListener(OnPauseBtnClick);
            TypeEventSystem.Register<UpdateHp>(UpdateHp);
            TypeEventSystem.Register<UpdateCoins>(UpdateCoins);
            TypeEventSystem.Register<UpdateWaveCount>(UpdateWave);
            TypeEventSystem.Register<UpdateNextWaveCountdown>(UpdateNextWaveCountdown);
            TypeEventSystem.Register<OnReplay>(OnReplay);
        }

        protected override void OnClose()
        {
            spawnBtn.onClick.RemoveAllListeners();
            pauseBtn.onClick.RemoveAllListeners();
            TypeEventSystem.UnRegister<UpdateHp>(UpdateHp);
            TypeEventSystem.UnRegister<UpdateCoins>(UpdateCoins);
            TypeEventSystem.UnRegister<UpdateWaveCount>(UpdateWave);
            TypeEventSystem.UnRegister<UpdateNextWaveCountdown>(UpdateNextWaveCountdown);
            TypeEventSystem.UnRegister<OnReplay>(OnReplay);
        }

        private void OnSpawnBtnClick()
        {
            if (firstWave)
            {
                firstWave = false;
                TypeEventSystem.Send(new StartGame());
            }
            else
            {
                TypeEventSystem.Send(new NextWave());
            }

            spawnBtn.gameObject.SetActive(false);
        }

        private void OnPauseBtnClick()
        {
            TypeEventSystem.Send(new PauseGame());
            UIManager.Instance.Open<UIPausePanel>(layer: UILayer.Foreground);
        }

        private void UpdateHp(UpdateHp context)
        {
            hpSlider.value = context.hp;
            hpText.text = context.hp + "/" + data.maxHp;
        }

        private void UpdateCoins(UpdateCoins context)
        {
            coinsText.text = context.coins.ToString();
        }

        private void UpdateWave(UpdateWaveCount context)
        {
            spawnBtn.gameObject.SetActive(false);
            waveText.text = context.waveCount + "/" + data.maxWaveCount;
        }

        private void UpdateNextWaveCountdown(UpdateNextWaveCountdown context)
        {
            if (!spawnBtn.gameObject.activeSelf)
            {
                spawnBtn.gameObject.SetActive(true);
            }

            spawnText.text = $"开始({context.countdown.ToString("0.0")}S)";
        }

        private void OnReplay(OnReplay context)
        {
            firstWave = true;
            spawnBtn.gameObject.SetActive(true);
            spawnText.text = "开始";
            hpSlider.maxValue = data.maxHp;
            hpSlider.value = data.maxHp;
            hpText.text = data.maxHp + "/" + data.maxHp;
            coinsText.text = data.coins.ToString();
            waveText.text = "1/" + data.maxWaveCount;
        }
    }

    public partial class UIGameScene
    {
        private UIGameSceneData data;

        [SerializeField] private Slider hpSlider = default;
        [SerializeField] private Text hpText = default;
        [SerializeField] private Text coinsText = default;

        [SerializeField] private Button pauseBtn = default;
        [SerializeField] private Text waveText = default;
        [SerializeField] private Button spawnBtn = default;
        [SerializeField] private Text spawnText = default;
    }
}
