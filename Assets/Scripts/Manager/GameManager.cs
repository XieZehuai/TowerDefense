using UnityEngine.SceneManagement;
using UnityEngine;

namespace TowerDefense
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public bool load = false;
        public string fileName = "MapData";
        public PathFindingStrategy pathFindingStrategy;

        protected override void OnInit()
        {
            UIManager.Instance.Open<UIMainScene>();
        }

        private void Start()
        {
            Debug.Log(ConfigManager.Instance.DamageConfig.GetArmorPriority(AttackType.Laser, ArmorType.Heavy));
            Debug.Log(ConfigManager.Instance.DamageConfig.GetArmorPriority(AttackType.Laser, ArmorType.Medium));
            Debug.Log(ConfigManager.Instance.DamageConfig.GetArmorPriority(AttackType.Laser, ArmorType.Light));
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene("GameScene");
            UIManager.Instance.Close<UIMainScene>();
        }

        public void LoadMainScene()
        {
            SceneManager.LoadScene("MainScene");
            UIManager.Instance.Open<UIMainScene>();
        }
    }
}
