using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class UIGameSceneData : UIDataBase
    {
        public StageManager manager;
    }


    public partial class UIGameScene : UIBase
    {
        private StageManager manager;
        private int maxHp;
        private int coins;
        private int maxWaves;
        private bool isFirstWave;

        protected override void OnInit(UIDataBase uiData)
        {
            var data = uiData as UIGameSceneData ?? new UIGameSceneData();
            manager = data.manager;
            maxHp = manager.StageConfig.playerHp;
            coins = manager.StageConfig.coins;
            maxWaves = manager.StageConfig.waveDatas.Length;

            #region 注册按钮点击事件

            spawnBtn.onClick.AddListener(OnSpawnBtnClick);
            pauseBtn.onClick.AddListener(OnPauseBtnClick);

            for (int i = 0; i < gridTypeBtns.Length; i++)
            {
                gridTypeBtns[i].OnClick.AddListener(OnGridTypeBtnClick);
            }
            for (int i = 0; i < towerTypeBtns.Length; i++)
            {
                towerTypeBtns[i].OnClick.AddListener(OnTowerTypeBtnClick);
            }

            saveBtn.onClick.AddListener(OnSaveBtnClick);
            pathToggleBtn.onClick.AddListener(OnPathToggleBtnClick);
            speedUpBtn.onClick.AddListener(OnSpeedUpBtnClick);

            #endregion

            TypeEventSystem.Register<UpdateHp>(UpdateHp);
            TypeEventSystem.Register<UpdateCoins>(UpdateCoins);
            TypeEventSystem.Register<UpdateWaveCount>(UpdateWave);
            TypeEventSystem.Register<UpdateNextWaveCountdown>(UpdateNextWaveCountdown);
            TypeEventSystem.Register<OnReplay>(OnReplay);

            OnReplay();
        }

        protected override void OnClose()
        {
            manager = null;

            #region 注销按钮点击事件

            spawnBtn.onClick.RemoveAllListeners();
            pauseBtn.onClick.RemoveAllListeners();

            for (int i = 0; i < gridTypeBtns.Length; i++)
            {
                gridTypeBtns[i].OnClick.RemoveAllListeners();
            }
            for (int i = 0; i < towerTypeBtns.Length; i++)
            {
                towerTypeBtns[i].OnClick.RemoveAllListeners();
            }

            saveBtn.onClick.RemoveAllListeners();
            pathToggleBtn.onClick.RemoveAllListeners();
            speedUpBtn.onClick.RemoveAllListeners();

            #endregion

            TypeEventSystem.UnRegister<UpdateHp>(UpdateHp);
            TypeEventSystem.UnRegister<UpdateCoins>(UpdateCoins);
            TypeEventSystem.UnRegister<UpdateWaveCount>(UpdateWave);
            TypeEventSystem.UnRegister<UpdateNextWaveCountdown>(UpdateNextWaveCountdown);
            TypeEventSystem.UnRegister<OnReplay>(OnReplay);
        }


        #region Button点击事件
        private void OnSpawnBtnClick()
        {
            if (isFirstWave)
            {
                isFirstWave = false;
                manager.StartGame();
            }
            else
            {
                manager.EnemyManager.SpawnNextWave();
            }

            spawnBtn.gameObject.SetActive(false);
        }

        private void OnPauseBtnClick()
        {
            manager.Pause();
        }

        private void OnSpeedUpBtnClick()
        {
            manager.SpeedUp(!manager.IsSpeedUp);
            speedUpBtnText.text = manager.IsSpeedUp ? "x2" : "x1";
        }

        private void OnGridTypeBtnClick(MapObjectType type)
        {
            manager.InputManager.GridType = type;
        }

        private void OnTowerTypeBtnClick(int towerId)
        {
            manager.InputManager.TowerId = towerId;
        }

        private void OnSaveBtnClick()
        {
            manager.SaveMapData();
        }

        private void OnPathToggleBtnClick()
        {
            manager.PathIndicator.TogglePathIndicator();
        }
        #endregion


        #region UI更新事件

        private void UpdateHp(UpdateHp context)
        {
            hpSlider.value = context.hp;
            hpText.text = context.hp + "/" + maxHp;
        }

        private void UpdateCoins(UpdateCoins context)
        {
            coinsText.text = context.coins.ToString();
        }

        private void UpdateWave(UpdateWaveCount context)
        {
            spawnBtn.gameObject.SetActive(false);
            waveText.text = context.waveCount + "/" + maxWaves;
        }

        private void UpdateNextWaveCountdown(UpdateNextWaveCountdown context)
        {
            if (!spawnBtn.gameObject.activeSelf)
            {
                spawnBtn.gameObject.SetActive(true);
            }

            spawnText.text = $"开始({context.countdown:0.0}S)";
        }

        private void OnReplay(OnReplay context = default)
        {
            isFirstWave = true;
            spawnBtn.gameObject.SetActive(true);
            spawnText.text = "开始";
            hpSlider.maxValue = maxHp;
            hpSlider.value = maxHp;
            hpText.text = maxHp + "/" + maxHp;
            coinsText.text = coins.ToString();
            waveText.text = "1/" + maxWaves;
            speedUpBtnText.text = manager.IsSpeedUp ? "x2" : "x1";
        }
        #endregion
    }

    public partial class UIGameScene
    {
        [Header("左上角")]
        [SerializeField] private Slider hpSlider = default;
        [SerializeField] private Text hpText = default;
        [SerializeField] private Text coinsText = default;

        [Header("右上角")]
        [SerializeField] private Button pauseBtn = default;
        [SerializeField] private Button speedUpBtn = default;
        [SerializeField] private Text speedUpBtnText = default;
        [SerializeField] private Text waveText = default;
        [SerializeField] private Button spawnBtn = default;
        [SerializeField] private Text spawnText = default;

        [Header("格子类型选择按钮")]
        [SerializeField] private GridTypeButton[] gridTypeBtns = default;

        [SerializeField] private Button saveBtn = default;
        [SerializeField] private Button pathToggleBtn = default;

        [Header("底部炮塔类型选择按钮")]
        [SerializeField] private TowerTypeButton[] towerTypeBtns = default;
    }
}
