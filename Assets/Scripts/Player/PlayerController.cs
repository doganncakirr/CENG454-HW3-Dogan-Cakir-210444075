using UnityEngine;
using CoreBreach.Interfaces;
using CoreBreach.Systems;

namespace CoreBreach.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("Look Settings")]
        [SerializeField] private float mouseSensitivity = 200f; 
        [SerializeField] private Transform playerCamera;
        private float xRotation = 0f;

        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        [Header("Weapon Settings")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 0.15f;
        private float nextFireTime;

        private Rigidbody rb;
        private Vector2 moveInput;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            currentHealth = maxHealth;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
            }
        }

        private void Update()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);

            if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                Shoot();
            }
        }

        private void FixedUpdate()
        {
            Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        private void Shoot()
        {
            nextFireTime = Time.time + fireRate;

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            Vector3 targetPoint;
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(100f); 
            }

            Vector3 direction = targetPoint - firePoint.position;
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", firePoint.position, Quaternion.LookRotation(direction));
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
            
            if (currentHealth <= 0) Die();
        }

        public void Die()
        {
            Debug.Log("Player Died!");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}