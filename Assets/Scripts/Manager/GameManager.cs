using UnityEngine.SceneManagement;

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
