using UnityEngine;
using CoreBreach.Interfaces;

namespace CoreBreach.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 50f;
        private float currentHealth;

        private Rigidbody rb;
        private Vector2 moveInput;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            currentHealth = maxHealth;
        }

        private void Update()
        {
            // Basit Input alımı
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
        }

        private void FixedUpdate()
        {
            // Fizik tabanlı hareket
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            if (currentHealth <= 0) Die();
        }

        public void Die()
        {
            Debug.Log("Player Died!");
            gameObject.SetActive(false);
        }
    }
}