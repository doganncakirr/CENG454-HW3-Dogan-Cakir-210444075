using UnityEngine;
using CoreBreach.Interfaces;
using CoreBreach.Systems;

namespace CoreBreach.Entities
{
    public class GameCore : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Start()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateCoreHealth(currentHealth, maxHealth);
            }
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            Debug.Log($"Core Damage! Health: {currentHealth}");

            UIManager.Instance.UpdateCoreHealth(currentHealth, maxHealth);
            
            if (currentHealth <= 0) Die();
        }

        public void Die()
        {
            Debug.Log("GAME OVER: The Core has been destroyed!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}