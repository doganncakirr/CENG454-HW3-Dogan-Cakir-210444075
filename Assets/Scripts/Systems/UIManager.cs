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

        [Header("Panels")]
        [SerializeField] private GameObject gameOverPanel;

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

        private void Start()
        {
            UpdateScore(0);
        }

        public void UpdatePlayerHealth(float currentHealth, float maxHealth)
        {
            if (playerHealthText != null)
            {
                playerHealthText.text = $"Player HP: {currentHealth}/{maxHealth}";
            }
        }

        public void UpdateCoreHealth(float currentHealth, float maxHealth)
        {
            if (coreHealthText != null)
            {
                coreHealthText.text = $"Core HP: {currentHealth}/{maxHealth}";
            }
        }

        public void AddScore(int points)
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

        public void ShowGameOverPanel()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}