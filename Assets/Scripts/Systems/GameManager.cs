using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreBreach.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Win Condition")]
        [SerializeField] private float survivalDuration = 180f;

        private float elapsedSurvivalTime;
        private bool gameEnded;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            Time.timeScale = 1f;
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

        private void Start()
        {
            elapsedSurvivalTime = 0f;
            gameEnded = false;

            GameEvents.FireSurvivalTimeChanged(survivalDuration, survivalDuration);
        }

        private void Update()
        {
            if (gameEnded) return;

            elapsedSurvivalTime += Time.deltaTime;

            float remainingTime = Mathf.Max(0f, survivalDuration - elapsedSurvivalTime);
            GameEvents.FireSurvivalTimeChanged(remainingTime, survivalDuration);

            if (elapsedSurvivalTime >= survivalDuration)
            {
                WinGame();
            }
        }

        private void WinGame()
        {
            if (gameEnded) return;

            gameEnded = true;
            Debug.Log("VICTORY: The Core survived the breach!");

            GameEvents.FireGameWon();
            Time.timeScale = 0f;
        }

        public void GameOver()
        {
            if (gameEnded) return;

            gameEnded = true;
            Debug.Log("GAME OVER: The facility has fallen!");

            Time.timeScale = 0f;
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}