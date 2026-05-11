using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreBreach.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        { 
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void GameOver()
        {
            Time.timeScale = 0f;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOverPanel();
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}