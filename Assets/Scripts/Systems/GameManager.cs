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

        private void OnEnable()
        {
            GameEvents.OnPlayerDied += GameOver;
            GameEvents.OnCoreDied += GameOver;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerDied -= GameOver;
            GameEvents.OnCoreDied -= GameOver;
        }

        public void GameOver()
        {
            Time.timeScale = 0f;
        }
        
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}