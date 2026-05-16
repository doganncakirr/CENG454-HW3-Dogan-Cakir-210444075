using UnityEngine;
using TMPro;

namespace CoreBreach.Systems
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private TextMeshProUGUI coreHealthText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Panels")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;

        private int currentScore = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerHealthChanged += UpdatePlayerHealth;
            GameEvents.OnCoreHealthChanged += UpdateCoreHealth;
            GameEvents.OnEnemyDied += AddScore;
            GameEvents.OnPlayerDied += ShowGameOverPanel;
            GameEvents.OnCoreDied += ShowGameOverPanel;
            GameEvents.OnGameWon += ShowVictoryPanel;
            GameEvents.OnSurvivalTimeChanged += UpdateTimer;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerHealthChanged -= UpdatePlayerHealth;
            GameEvents.OnCoreHealthChanged -= UpdateCoreHealth;
            GameEvents.OnEnemyDied -= AddScore;
            GameEvents.OnPlayerDied -= ShowGameOverPanel;
            GameEvents.OnCoreDied -= ShowGameOverPanel;
            GameEvents.OnGameWon -= ShowVictoryPanel;
            GameEvents.OnSurvivalTimeChanged -= UpdateTimer;
        }

        private void Start()
        {
            UpdateScore(0);

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }
        }

        private void UpdatePlayerHealth(float currentHealth, float maxHealth)
        {
            if (playerHealthText != null)
            {
                playerHealthText.text = $"Player HP: {currentHealth}/{maxHealth}";
            }
        }

        private void UpdateCoreHealth(float currentHealth, float maxHealth)
        {
            if (coreHealthText != null)
            {
                coreHealthText.text = $"Core HP: {currentHealth}/{maxHealth}";
            }
        }

        private void AddScore(int points)
        {
            currentScore += points;
            UpdateScore(currentScore);
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        private void UpdateTimer(float remainingTime, float totalTime)
        {
            if (timerText == null) return;

            int totalSeconds = Mathf.CeilToInt(remainingTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }

        private void ShowGameOverPanel()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void ShowVictoryPanel()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}